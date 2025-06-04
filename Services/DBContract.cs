using System.Linq.Expressions;
using System.Text;
using MaCamp.Models;
using MaCamp.Utils;
using SQLite;
using static MaCamp.Utils.Enumeradores;

namespace MaCamp.Services
{
    public static class DBContract
    {
        private static object SyncLock { get; }
        public static SQLiteAsyncConnection? SqlConnection { get; set; }
        private static SemaphoreSlim SemaphoreSlim { get; }

        static DBContract()
        {
            SyncLock = new object();
            SemaphoreSlim = new SemaphoreSlim(1, 1);
        }

        public static async Task InitializeAsync()
        {
            try
            {
                await SemaphoreSlim.WaitAsync();

                SqlConnection = await GetConnectionAsync(AppConstants.SqliteFilename);

                if (SqlConnection != null)
                {
                    await UpdateKeyValue(AppConstants.Busca_InicialRealizada, null);
                    await UpdateKeyValue(AppConstants.Filtro_ServicoSelecionados, null);
                    await UpdateKeyValue(AppConstants.Filtro_EstadoSelecionado, null);
                    await UpdateKeyValue(AppConstants.Filtro_CidadeSelecionada, null);
                    await UpdateKeyValue(AppConstants.Filtro_NomeCamping, null);
                }
            }
            finally
            {
                SemaphoreSlim.Release();
            }
        }

        private static async Task<SQLiteAsyncConnection?> GetConnectionAsync(string filename)
        {
            try
            {
                var path = Path.Combine(App.PATH, filename);
                var connection = new SQLiteAsyncConnection(path, SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.SharedCache | SQLiteOpenFlags.FullMutex);

                await connection.CreateTablesAsync<Item, ItemIdentificador, ChaveValor, Cidade, Colaboracao>();

                return connection;
            }
            catch (Exception ex)
            {
                Workaround.ShowExceptionOnlyDevolpmentMode(nameof(DBContract), nameof(GetConnectionAsync), ex);
            }

            return null;
        }

        private static async Task<bool> VerifyDatabaseIntegrityAsync(string filename)
        {
            try
            {
                var path = Path.Combine(App.PATH, filename);
                var connection = new SQLiteAsyncConnection(path);
                var result = await connection.ExecuteScalarAsync<string>("PRAGMA integrity_check;");

                await connection.CloseAsync();

                return result == "ok";
            }
            catch (Exception ex)
            {
                Workaround.ShowExceptionOnlyDevolpmentMode(nameof(DBContract), nameof(VerifyDatabaseIntegrityAsync), ex);

                return false;
            }
        }

        private static async Task<bool> UpdateAsync(bool clean, Dictionary<Type, List<object>> dataDictionary, ProgressoVisual? progressoVisual = null)
        {
            ProgressoVisual.AumentarTotal(progressoVisual, 2 + dataDictionary.Count);

            if (SqlConnection != null)
            {
                try
                {
                    ProgressoVisual.AumentarAtual(progressoVisual);

                    foreach (var (key, values) in dataDictionary)
                    {
                        if (clean)
                        {
                            var table = SqlConnection.TableMappings.FirstOrDefault(x => x.MappedType == key);

                            if (table != null)
                            {
                                await SqlConnection.DeleteAllAsync(table);
                            }
                        }

                        await SqlConnection.RunInTransactionAsync(x =>
                        {
                            foreach (var value in values)
                            {
                                x.InsertOrReplace(value);
                            }
                        });

                        ProgressoVisual.AumentarAtual(progressoVisual);
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    Workaround.ShowExceptionOnlyDevolpmentMode(nameof(DBContract), nameof(UpdateAsync), ex, false);
                }
            }

            return false;
        }

        public static async Task<bool> UpdateAsync<T>(bool clean, IEnumerable<T> listPrimaryData, ProgressoVisual? progressoVisual = null)
        {
            var dataDictionary = new Dictionary<Type, List<object>>
            {
                { typeof(T), listPrimaryData.OfType<object>().ToList() }
            };

            return await UpdateAsync(clean, dataDictionary, progressoVisual);
        }

        public static async Task<bool> UpdateAsync<T, TK>(bool clean, IEnumerable<T> listPrimaryData, IEnumerable<TK> listaSecondaryData, ProgressoVisual? progressoVisual = null)
        {
            var dataDictionary = new Dictionary<Type, List<object>>
            {
                { typeof(T), listPrimaryData.OfType<object>().ToList() },
                { typeof(TK), listaSecondaryData.OfType<object>().ToList() }
            };

            return await UpdateAsync(clean, dataDictionary, progressoVisual);
        }

        public static async Task<bool> UpdateAsync<T, TK, TR>(bool clean, IEnumerable<T> listPrimaryData, IEnumerable<TK> listaSecondaryData, IEnumerable<TR> listaTertiaryData, ProgressoVisual? progressoVisual = null)
        {
            var dataDictionary = new Dictionary<Type, List<object>>
            {
                { typeof(T), listPrimaryData.OfType<object>().ToList() },
                { typeof(TK), listaSecondaryData.OfType<object>().ToList() },
                { typeof(TR), listaTertiaryData.OfType<object>().ToList() }
            };

            return await UpdateAsync(clean, dataDictionary, progressoVisual);
        }

        public static async Task<bool> UpdateAsync<T>(T value)
        {
            try
            {
                if (SqlConnection != null)
                {
                    await SqlConnection.InsertOrReplaceAsync(value);

                    return true;
                }
            }
            catch (Exception ex)
            {
                Workaround.ShowExceptionOnlyDevolpmentMode(nameof(DBContract), nameof(UpdateAsync), ex, false);
            }

            return false;
        }

        public static async Task<bool> UpdateKeyValue(string key, string? value, TipoChave type = default)
        {
            var keyValue = new ChaveValor
            {
                Chave = key,
                Valor = value,
                Tipo = type
            };

            return await UpdateAsync(keyValue);
        }

        public static async Task<T?> GetAsync<T>(Expression<Func<T, bool>>? predicate = null) where T : new()
        {
            try
            {
                if (SqlConnection != null)
                {
                    var table = SqlConnection.Table<T>();

                    if (predicate != null)
                    {
                        return await table.FirstOrDefaultAsync(predicate);
                    }

                    return await table.FirstOrDefaultAsync();
                }
            }
            catch (Exception ex)
            {
                Workaround.ShowExceptionOnlyDevolpmentMode(nameof(DBContract), nameof(GetAsync), ex);
            }

            return default;
        }

        public static async Task<string?> GetKeyValueAsync(string key)
        {
            var keyValue = await GetAsync<ChaveValor>(x => x.Chave == key);

            if (keyValue != null)
            {
                return keyValue.Valor;
            }

            return null;
        }

        public static async Task<List<T>> QueryAsync<T>(string query, params object[] args) where T : new()
        {
            try
            {
                if (SqlConnection != null)
                {
                    var list = await SqlConnection.QueryAsync<T>(query, args);

                    return list;
                }
            }
            catch (Exception ex)
            {
                Workaround.ShowExceptionOnlyDevolpmentMode(nameof(DBContract), nameof(QueryAsync), ex);
            }

            return new List<T>();
        }

        public static async Task<List<T>> ListAsync<T>(Expression<Func<T, bool>>? predicate = null) where T : new()
        {
            try
            {
                if (SqlConnection != null)
                {
                    var table = SqlConnection.Table<T>();

                    if (predicate != null)
                    {
                        return await table.Where(predicate).ToListAsync();
                    }

                    return await table.ToListAsync();
                }
            }
            catch (Exception ex)
            {
                Workaround.ShowExceptionOnlyDevolpmentMode(nameof(DBContract), nameof(ListAsync), ex);
            }

            return new List<T>();
        }

        public static async Task<List<Item>> ListCampingsAsync(string nomeDoCamping, string? cidade, string? estado)
        {
            try
            {
                if (SqlConnection != null)
                {
                    var nomeNormalizado = nomeDoCamping.RemoveDiacritics().ToLower();
                    var estadoNormalizado = estado?.RemoveDiacritics().ToLower();
                    var cidadeNormalizada = cidade?.RemoveDiacritics().ToLower();
                    var list = await SqlConnection.Table<Item>().ToListAsync();
                    var query = list.AsQueryable().Where(x =>
                        (string.IsNullOrEmpty(estadoNormalizado) || x.Estado != null && x.Estado.RemoveDiacritics().ToLower().Contains(estadoNormalizado)) &&
                        (string.IsNullOrEmpty(cidadeNormalizada) || x.Cidade != null && x.Cidade.RemoveDiacritics().ToLower().Contains(cidadeNormalizada)) &&
                        (string.IsNullOrEmpty(nomeNormalizado) ||
                            x.Nome != null && x.Nome.RemoveDiacritics().ToLower().Contains(nomeNormalizado) ||
                            x.Cidade != null && x.Cidade.RemoveDiacritics().ToLower().Contains(nomeNormalizado)
                        //precisa converter base64 para string, não compensa
                        //x.Descricao != null && x.Descricao.RemoveDiacritics().ToLower().Contains(nomeNormalizado)
                        )
                    );

                    return query.ToList();
                }
            }
            catch (Exception ex)
            {
                Workaround.ShowExceptionOnlyDevolpmentMode(nameof(DBContract), nameof(ListCampingsAsync), ex);
            }

            return new List<Item>();
        }

        public static async Task<List<int>> ListIdCampingsComComodidadesAsync(bool possuiFiltroComodidades, string comodidades)
        {
            try
            {
                if (possuiFiltroComodidades)
                {
                    var qtdFiltrosComodidades = comodidades.Split(',').Length;
                    var sbQueryComodidades = new StringBuilder();

                    sbQueryComodidades.Append($"SELECT II.{nameof(ItemIdentificador.IdItem)} ");
                    sbQueryComodidades.Append($" FROM {nameof(ItemIdentificador)} II ");
                    sbQueryComodidades.Append($" WHERE ");

                    if (comodidades == "'PossuiPraiaProxima'")
                    {
                        sbQueryComodidades.Append($" II.{nameof(ItemIdentificador.Identificador)} IN ({comodidades}) AND {nameof(ItemIdentificador.Opcao)} = 2 ");
                    }
                    else
                    {
                        sbQueryComodidades.Append($" II.{nameof(ItemIdentificador.Identificador)} IN ({comodidades}) AND {nameof(ItemIdentificador.Opcao)} = 1 ");
                    }

                    sbQueryComodidades.Append($" GROUP BY II.{nameof(ItemIdentificador.IdItem)} ");

                    if (qtdFiltrosComodidades > 0)
                    {
                        sbQueryComodidades.Append($" Having Count(II.{nameof(ItemIdentificador.IdItem)}) >= {qtdFiltrosComodidades}");
                    }

                    var query = sbQueryComodidades.ToString();
                    var list = await QueryAsync<RetornoIdItem>(query);

                    return list.Select(x => x.IdItem).ToList();
                }
            }
            catch (Exception ex)
            {
                Workaround.ShowExceptionOnlyDevolpmentMode(nameof(DBContract), nameof(ListCampingsAsync), ex);
            }

            return new List<int>();
        }

        public static async Task<List<int>> ListIdCampingsComCategoriasAsync(string categorias, bool possuiFiltroComodidades, List<int> idsCampingsComComodidades)
        {
            try
            {
                var queryIdsInCampingsComComodidade = string.Empty;

                if (possuiFiltroComodidades && idsCampingsComComodidades.Count > 0)
                {
                    queryIdsInCampingsComComodidade = $" (II.{nameof(ItemIdentificador.IdItem)} IN ({string.Join(",", idsCampingsComComodidades)}) ) AND ";
                }

                var sbQueryCategorias = new StringBuilder();

                sbQueryCategorias.Append($"SELECT II.{nameof(ItemIdentificador.IdItem)} ");
                sbQueryCategorias.Append($" FROM {nameof(ItemIdentificador)} II ");
                sbQueryCategorias.Append($" WHERE ");

                var complementoDaQuery = string.Empty;

                if (categorias.Length == 104)
                {
                    var identificadoresQueNaoEstaoNosFiltros = "'CampingEmFuncionamento'" + ",'CampingEmReformas'" + ",'CampingemSituaçãoIncerta'" + ",'CampingInformal'" + ",'CampingParaGruposouEventos'" + ",'Destaque'" + ",'Empresa'" + ",'EstacionamentoUrbanoouPraça'" + ",'Hotel/Pousada/Hostel'" + ",'PostodeCombustível'" + ",'Quiosque/Restaurante/PontoTurístico'" + ",'RVPark(ExclusivoRV`s)'" + ",'RVPark(SóMensalistas)'";
                    complementoDaQuery = $" OR II.{nameof(ItemIdentificador.Identificador)} IN ({identificadoresQueNaoEstaoNosFiltros})  ";
                }
                else
                {
                    if (categorias.Contains("Campings"))
                    {
                        var identificadoresCampings = "'CampingEmFuncionamento'" + ",'CampingEmReformas'" + ",'CampingemSituaçãoIncerta'" + ",'CampingInformal'" + ",'CampingParaGruposouEventos'" + ",'Destaque'";
                        complementoDaQuery += $" OR II.{nameof(ItemIdentificador.Identificador)} IN ({identificadoresCampings})  ";
                    }

                    if (categorias.Contains("PontodeApoioaRV`s"))
                    {
                        var identificadoresRVs = "'RVPark(ExclusivoRV`s)'" + ",'RVPark(SóMensalistas)'";
                        complementoDaQuery += $" OR II.{nameof(ItemIdentificador.Identificador)} IN ({identificadoresRVs})  ";
                    }
                }

                sbQueryCategorias.Append($" {queryIdsInCampingsComComodidade} (II.{nameof(ItemIdentificador.Identificador)} IN ({categorias}){complementoDaQuery})");
                sbQueryCategorias.Append($" GROUP BY {nameof(ItemIdentificador.IdItem)} ");

                var query = sbQueryCategorias.ToString();
                var list = await QueryAsync<RetornoIdItem>(query);

                return list.Select(x => x.IdItem).ToList();
            }
            catch (Exception ex)
            {
                Workaround.ShowExceptionOnlyDevolpmentMode(nameof(DBContract), nameof(ListCampingsAsync), ex);
            }

            return new List<int>();
        }
    }
}