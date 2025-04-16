using System.Linq.Expressions;
using System.Text;
using MaCamp.Models;
using MaCamp.Utils;
using SQLite;
using static MaCamp.Utils.Enumeradores;

namespace MaCamp.Services.DataAccess
{
    public static class DBContract
    {
        public static SQLiteAsyncConnection? SqlConnection { get; set; }

        private static object Lock { get; set; }

        static DBContract()
        {
            Lock = new object();
        }

        public static async Task InitializeAsync()
        {
            SqlConnection = await GetConnectionAsync(AppConstants.SqliteFilename);

            if (SqlConnection != null)
            {
                await SqlConnection.InsertOrReplaceAsync(new ChaveValor
                {
                    Chave = AppConstants.Filtro_ServicoSelecionados,
                    Valor = string.Empty
                });
                await SqlConnection.InsertOrReplaceAsync(new ChaveValor
                {
                    Chave = AppConstants.Filtro_NomeCamping,
                    Valor = string.Empty
                });
            }
        }

        private static async Task<SQLiteAsyncConnection?> GetConnectionAsync(string filename)
        {
            try
            {
                var path = Path.Combine(AppConstants.Path, filename);
                var connection = new SQLiteAsyncConnection(path, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);

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
                var path = Path.Combine(AppConstants.Path, filename);
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
            var backupDatabasePath = Path.Combine(AppConstants.Path, AppConstants.SqliteBackupFilename);
            var currentDatabasePath = Path.Combine(AppConstants.Path, AppConstants.SqliteFilename);
            var temporaryDatabasePath = Path.Combine(AppConstants.Path, AppConstants.SqliteTemporaryFilename);

            ProgressoVisual.AumentarTotal(progressoVisual, 8 + dataDictionary.Count);

            try
            {
                ProgressoVisual.AumentarAtual(progressoVisual);

                if (File.Exists(currentDatabasePath))
                {
                    File.Copy(currentDatabasePath, backupDatabasePath, true);
                }

                ProgressoVisual.AumentarAtual(progressoVisual);

                if (File.Exists(temporaryDatabasePath))
                {
                    File.Delete(temporaryDatabasePath);
                }

                ProgressoVisual.AumentarAtual(progressoVisual);

                if (!clean)
                {
                    File.Copy(currentDatabasePath, temporaryDatabasePath, true);
                }

                ProgressoVisual.AumentarAtual(progressoVisual);

                var temporaryConnection = await GetConnectionAsync(AppConstants.SqliteTemporaryFilename);

                if (temporaryConnection == null)
                {
                    throw new NullReferenceException("Conexão SQL não foi criada");
                }

                ProgressoVisual.AumentarAtual(progressoVisual);

                foreach (var (key, values) in dataDictionary)
                {
                    if (!clean)
                    {
                        var table = temporaryConnection.TableMappings.FirstOrDefault(x => x.MappedType == key);

                        if (table != null)
                        {
                            await temporaryConnection.DeleteAllAsync(table);
                        }
                    }

                    await temporaryConnection.RunInTransactionAsync(x =>
                    {
                        foreach (var value in values)
                        {
                            x.InsertOrReplace(value);
                        }
                    });

                    ProgressoVisual.AumentarAtual(progressoVisual);
                }

                await temporaryConnection.InsertOrReplaceAsync(new ChaveValor(AppConstants.Chave_DownloadCampingsCompleto, "true", TipoChave.ControleInterno));
                await temporaryConnection.InsertOrReplaceAsync(new ChaveValor
                {
                    Chave = AppConstants.Chave_DataUltimaAtualizacaoConteudo,
                    Valor = DateTime.Now.ToString("yyyy/MM/dd")
                });

                ProgressoVisual.AumentarAtual(progressoVisual);

                var checkIntegrity = await VerifyDatabaseIntegrityAsync(AppConstants.SqliteTemporaryFilename);

                if (!checkIntegrity)
                {
                    throw new Exception("Integridade do banco atualizado falhou.");
                }

                ProgressoVisual.AumentarAtual(progressoVisual);

                if (SqlConnection != null)
                {
                    await SqlConnection.CloseAsync();
                }

                if (File.Exists(currentDatabasePath))
                {
                    File.Delete(currentDatabasePath);
                }

                File.Move(temporaryDatabasePath, currentDatabasePath);

                SqlConnection = await GetConnectionAsync(AppConstants.SqliteFilename);

                ProgressoVisual.AumentarAtual(progressoVisual);

                return true;
            }
            catch (Exception ex)
            {
                Workaround.ShowExceptionOnlyDevolpmentMode(nameof(DBContract), nameof(UpdateAsync), ex);

                if (SqlConnection != null)
                {
                    await SqlConnection.CloseAsync();
                }

                if (File.Exists(currentDatabasePath))
                {
                    File.Delete(currentDatabasePath);
                }

                if (File.Exists(backupDatabasePath))
                {
                    File.Copy(backupDatabasePath, currentDatabasePath, true);
                }

                SqlConnection = await GetConnectionAsync(AppConstants.SqliteFilename);

                return false;
            }
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
                Workaround.ShowExceptionOnlyDevolpmentMode(nameof(DBContract), nameof(UpdateAsync), ex);
            }

            return false;
        }

        public static async Task<bool> UpdateKeyValueAsync(string key, string? value, TipoChave type = default)
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
                    if (predicate != null)
                    {
                        return await SqlConnection.Table<T>().FirstOrDefaultAsync(predicate);
                    }

                    return await SqlConnection.Table<T>().FirstOrDefaultAsync();
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
                return keyValue?.Valor;
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
                    if (predicate != null)
                    {
                        return await SqlConnection.Table<T>().Where(predicate).ToListAsync();
                    }

                    return await SqlConnection.Table<T>().ToListAsync();
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
                        (string.IsNullOrWhiteSpace(estadoNormalizado) || x.Estado != null && x.Estado.RemoveDiacritics().ToLower().Contains(estadoNormalizado)) &&
                        (string.IsNullOrWhiteSpace(cidadeNormalizada) || x.Cidade != null && x.Cidade.RemoveDiacritics().ToLower().Contains(cidadeNormalizada)) &&
                        (string.IsNullOrWhiteSpace(nomeNormalizado) ||
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