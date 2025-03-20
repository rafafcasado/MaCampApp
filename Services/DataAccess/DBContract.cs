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
        private static Mutex Mutex { get; }
        private static object Lock { get; }
        private static SQLiteConnection? SqlConnection { get; set; }

        public class RetornoIdItem
        {
            public int IdItem { get; set; }
        }

        static DBContract()
        {
            Mutex = new Mutex();
            Lock = new object();
            SqlConnection = GetConnection(AppConstants.SqliteFilename);

            if (SqlConnection != null)
            {
                SqlConnection.InsertOrReplace(new ChaveValor
                {
                    Chave = AppConstants.Filtro_ServicoSelecionados,
                    Valor = string.Empty
                });
                SqlConnection.InsertOrReplace(new ChaveValor
                {
                    Chave = AppConstants.Filtro_NomeCamping,
                    Valor = string.Empty
                });
            }
        }

        private static SQLiteConnection? GetConnection(string filename)
        {
            try
            {
                var path = Path.Combine(AppConstants.Path, filename);
                var connection = new SQLiteConnection(path);

                connection.CreateTables<Item, ItemIdentificador, ChaveValor, Cidade, Colaboracao>();

                return connection;
            }
            catch (Exception ex)
            {
                Workaround.ShowExceptionOnlyDevolpmentMode(nameof(DBContract), nameof(GetConnection), ex);
            }

            return null;
        }

        public static async Task<bool> UpdateAsync<T>(bool clean, List<T> listPrimaryData, ProgressoVisual? progressoVisual = null)
        {
            var dataDictionary = new Dictionary<Type, List<object>>
            {
                { typeof(T), listPrimaryData.OfType<object>().ToList() }
            };

            return await UpdateAsync(clean, dataDictionary, progressoVisual);
        }

        public static async Task<bool> UpdateAsync<T, TK>(bool clean, List<T> listPrimaryData, List<TK> listaSecondaryData, ProgressoVisual? progressoVisual = null)
        {
            var dataDictionary = new Dictionary<Type, List<object>>
            {
                { typeof(T), listPrimaryData.OfType<object>().ToList() },
                { typeof(TK), listaSecondaryData.OfType<object>().ToList() }
            };

            return await UpdateAsync(clean, dataDictionary, progressoVisual);
        }

        public static async Task<bool> UpdateAsync<T, TK, TR>(bool clean, List<T> listPrimaryData, List<TK> listaSecondaryData, List<TR> listaTertiaryData, ProgressoVisual? progressoVisual = null)
        {
            var dataDictionary = new Dictionary<Type, List<object>>
            {
                { typeof(T), listPrimaryData.OfType<object>().ToList() },
                { typeof(TK), listaSecondaryData.OfType<object>().ToList() },
                { typeof(TR), listaTertiaryData.OfType<object>().ToList() }
            };

            return await UpdateAsync(clean, dataDictionary, progressoVisual);
        }

        private static async Task<bool> UpdateAsync(bool clean, Dictionary<Type, List<object>> dataDictionary, ProgressoVisual? progressoVisual = null)
        {
            var backupDatabasePath = Path.Combine(AppConstants.Path, AppConstants.SqliteBackupFilename);
            var currentDatabasePath = Path.Combine(AppConstants.Path, AppConstants.SqliteFilename);
            var temporaryDatabasePath = Path.Combine(AppConstants.Path, AppConstants.SqliteTemporaryFilename);

            ProgressoVisual.AumentarTotal(progressoVisual, 7 + dataDictionary.Count);

            try
            {
                await ProgressoVisual.AumentarAtualAsync(progressoVisual);

                if (File.Exists(currentDatabasePath))
                {
                    File.Copy(currentDatabasePath, backupDatabasePath, true);
                }

                await ProgressoVisual.AumentarAtualAsync(progressoVisual);

                if (clean)
                {
                    if (File.Exists(temporaryDatabasePath))
                    {
                        File.Delete(temporaryDatabasePath);
                    }
                }
                else
                {
                    File.Copy(currentDatabasePath, temporaryDatabasePath, true);
                }

                await ProgressoVisual.AumentarAtualAsync(progressoVisual);

                var temporaryConnection = GetConnection(AppConstants.SqliteTemporaryFilename);

                if (temporaryConnection == null)
                {
                    throw new NullReferenceException("Conexão SQL não foi criada");
                }

                await ProgressoVisual.AumentarAtualAsync(progressoVisual);

                foreach (var (key, value) in dataDictionary)
                {
                    var table = temporaryConnection.TableMappings.FirstOrDefault(x => x.MappedType == key);

                    if (table != null)
                    {
                        if (!clean)
                        {
                            temporaryConnection.DeleteAll(table);
                        }

                        temporaryConnection.InsertAll(value);
                    }

                    await ProgressoVisual.AumentarAtualAsync(progressoVisual);
                }

                temporaryConnection.InsertOrReplace(new ChaveValor(AppConstants.Chave_DownloadCampingsCompleto, "true", TipoChave.ControleInterno));

                temporaryConnection.InsertOrReplace(new ChaveValor
                {
                    Chave = AppConstants.Chave_DataUltimaAtualizacaoConteudo,
                    Valor = DateTime.Now.ToString("yyyy/MM/dd")
                });

                await ProgressoVisual.AumentarAtualAsync(progressoVisual);

                if (!VerifyDatabaseIntegrity(AppConstants.SqliteTemporaryFilename))
                {
                    throw new Exception("Integridade do banco atualizado falhou.");
                }

                await ProgressoVisual.AumentarAtualAsync(progressoVisual);

                lock (Lock)
                {
                    if (SqlConnection != null)
                    {
                        SqlConnection.Close();
                        SqlConnection.Dispose();
                    }

                    if (File.Exists(currentDatabasePath))
                    {
                        File.Delete(currentDatabasePath);
                    }

                    File.Move(temporaryDatabasePath, currentDatabasePath);

                    SqlConnection = GetConnection(AppConstants.SqliteFilename);
                }

                await ProgressoVisual.AumentarAtualAsync(progressoVisual);

                return true;
            }
            catch (Exception ex)
            {
                Workaround.ShowExceptionOnlyDevolpmentMode(nameof(DBContract), nameof(UpdateAsync), ex);

                lock (Lock)
                {
                    if (SqlConnection != null)
                    {
                        SqlConnection.Close();
                        SqlConnection.Dispose();
                    }

                    if (File.Exists(currentDatabasePath))
                    {
                        File.Delete(currentDatabasePath);
                    }

                    if (File.Exists(backupDatabasePath))
                    {
                        File.Copy(backupDatabasePath, currentDatabasePath, true);
                    }

                    SqlConnection = GetConnection(AppConstants.SqliteFilename);
                }

                return false;
            }
        }

        /// <summary>
        /// Verifica a integridade do banco de dados usando PRAGMA integrity_check.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns>Estado do banco de dados</returns>
        private static bool VerifyDatabaseIntegrity(string filename)
        {
            try
            {
                var path = Path.Combine(AppConstants.Path, filename);
                using var connection = new SQLiteConnection(path);
                var result = connection.ExecuteScalar<string>("PRAGMA integrity_check;");

                return result == "ok";
            }
            catch (Exception ex)
            {
                Workaround.ShowExceptionOnlyDevolpmentMode(nameof(DBContract), nameof(VerifyDatabaseIntegrity), ex);

                return false;
            }
        }

        /// <summary>
        /// Insere um objeto no BD.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Objeto inserido, com a Primary Key atualizada</returns>
        public static object? InserirModelo(object? model)
        {
            lock (Lock)
            {
                try
                {
                    //Mutex.WaitOne();

                    if (SqlConnection == null)
                    {
                        throw new NullReferenceException("Conexão SQL não foi criada");
                    }

                    var qtdInserida = SqlConnection.Insert(model);

                    return qtdInserida == 1 ? model : null;
                }
                catch (Exception ex)
                {
                    Workaround.ShowExceptionOnlyDevolpmentMode(nameof(DBContract), nameof(InserirModelo), ex);

                    return 0;
                }
                //finally
                //{
                //    Mutex.ReleaseMutex();
                //}
            }
        }

        public static int InserirListaDeModelo<T>(IEnumerable<T> model)
        {
            lock (Lock)
            {
                try
                {
                    //Mutex.WaitOne();

                    if (SqlConnection == null)
                    {
                        throw new NullReferenceException("Conexão SQL não foi criada");
                    }

                    SqlConnection.BeginTransaction();

                    var qtdInserida = SqlConnection.InsertAll(model);

                    SqlConnection.Commit();

                    return qtdInserida;
                }
                catch (Exception ex)
                {
                    Workaround.ShowExceptionOnlyDevolpmentMode(nameof(DBContract), nameof(InserirListaDeModelo), ex);

                    return 0;
                }
                //finally
                //{
                //    Mutex.ReleaseMutex();
                //}
            }
        }

        public static int InserirOuSubstituirModelo(object model)
        {
            lock (Lock)
            {
                try
                {
                    //Mutex.WaitOne();

                    if (SqlConnection == null)
                    {
                        throw new NullReferenceException("Conexão SQL não foi criada");
                    }

                    var result = SqlConnection.InsertOrReplace(model);

                    return result;
                }
                catch (Exception ex)
                {
                    Workaround.ShowExceptionOnlyDevolpmentMode(nameof(DBContract), nameof(InserirOuSubstituirModelo), ex);

                    return 0;
                }
                //finally
                //{
                //    Mutex.ReleaseMutex();
                //}
            }
        }

        public static Item? ObterItem(Expression<Func<Item, bool>> where)
        {
            lock (Lock)
            {
                try
                {
                    //Mutex.WaitOne();

                    if (SqlConnection == null)
                    {
                        throw new NullReferenceException("Conexão SQL não foi criada");
                    }

                    var item = SqlConnection.Table<Item>().FirstOrDefault(where);

                    return item;
                }
                catch (Exception ex)
                {
                    Workaround.ShowExceptionOnlyDevolpmentMode(nameof(DBContract), nameof(ObterItem), ex);

                    return null;
                }
                //finally
                //{
                //    Mutex.ReleaseMutex();
                //}
            }
        }

        public static string? ObterValorChave(string chave)
        {
            lock (Lock)
            {
                try
                {
                    //Mutex.WaitOne();

                    if (SqlConnection == null)
                    {
                        throw new NullReferenceException("Conexão SQL não foi criada");
                    }

                    var valor = SqlConnection.Table<ChaveValor>().FirstOrDefault(x => x.Chave == chave)?.Valor;

                    return valor;
                }
                catch (Exception ex)
                {
                    Workaround.ShowExceptionOnlyDevolpmentMode(nameof(DBContract), nameof(ObterValorChave), ex);

                    return default;
                }
                //finally
                //{
                //    Mutex.ReleaseMutex();
                //}
            }
        }

        public static List<int> ListarIdsCampingsComComodidades(bool possuiFiltroComodidades, string comodidades)
        {
            if (!possuiFiltroComodidades)
            {
                return new List<int>();
            }

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

            return QueryIdsIdentificadorCampings(query).Select(x => x.IdItem).ToList();
        }

        public static List<int> ListarIdsCampingsComCategorias(string categorias, bool possuiFiltroComodidades, List<int> idsCampingsComComodidades)
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

            return QueryIdsIdentificadorCampings(query).Select(x => x.IdItem).ToList();
        }

        public static List<Item> ListarCampings()
        {
            if (SqlConnection == null)
            {
                throw new NullReferenceException("Conexão SQL não foi criada");
            }

            var campings = SqlConnection.Table<Item>().ToList();

            return campings;
        }

        public static List<Item> BuscarCampings(string nomeDoCamping, string? cidade, string? estado)
        {
            lock (Lock)
            {
                try
                {
                    //Mutex.WaitOne();

                    if (SqlConnection == null)
                    {
                        throw new NullReferenceException("Conexão SQL não foi criada");
                    }

                    var nomeNormalizado = nomeDoCamping.RemoveDiacritics().ToLower();
                    var estadoNormalizado = estado?.RemoveDiacritics().ToLower();
                    var cidadeNormalizada = cidade?.RemoveDiacritics().ToLower();
                    var query = SqlConnection.Table<Item>().AsQueryable().Where(x =>
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
                catch (Exception ex)
                {
                    Workaround.ShowExceptionOnlyDevolpmentMode(nameof(DBContract), nameof(BuscarCampings), ex);

                    return new List<Item>();
                }
                //finally
                //{
                //    Mutex.ReleaseMutex();
                //}
            }
        }

        public static List<RetornoIdItem> QueryIdsIdentificadorCampings(string query)
        {
            lock (Lock)
            {
                try
                {
                    //Mutex.WaitOne();

                    if (SqlConnection == null)
                    {
                        throw new NullReferenceException("Conexão SQL não foi criada");
                    }

                    var itens = SqlConnection.Query<RetornoIdItem>(query);

                    return itens;
                }
                catch (Exception ex)
                {
                    Workaround.ShowExceptionOnlyDevolpmentMode(nameof(DBContract), nameof(QueryIdsIdentificadorCampings), ex);

                    return new List<RetornoIdItem>();
                }
                //finally
                //{
                //    Mutex.ReleaseMutex();
                //}
            }
        }

        public static List<Item> QueryItens(string query, params object[] args)
        {
            lock (Lock)
            {
                try
                {
                    //Mutex.WaitOne();

                    if (SqlConnection == null)
                    {
                        throw new NullReferenceException("Conexão SQL não foi criada");
                    }

                    var itens = SqlConnection.Query<Item>(query, args);

                    return itens;
                }
                catch (Exception ex)
                {
                    Workaround.ShowExceptionOnlyDevolpmentMode(nameof(DBContract), nameof(QueryItens), ex);

                    return new List<Item>();
                }
                //finally
                //{
                //    Mutex.ReleaseMutex();
                //}
            }
        }

        public static List<Item> FiltrarApenasCampingsDestaque(List<int> idsCampings)
        {
            lock (Lock)
            {
                var indiceAtual = 0;
                var tamanhoBloco = 100;
                var ids = new List<int>();
                var itensDestaque = new List<Item>();

                try
                {
                    //Mutex.WaitOne();

                    if (SqlConnection == null)
                    {
                        throw new NullReferenceException("Conexão SQL não foi criada");
                    }

                    var qtdIdsRestantes = idsCampings.Count;

                    while (qtdIdsRestantes > 0)
                    {
                        var idsBlocoAtual = idsCampings.Skip(indiceAtual * tamanhoBloco).Take(tamanhoBloco).ToList();
                        ids.AddRange(SqlConnection.Table<ItemIdentificador>().Where(x => x.Identificador == "Destaque" && idsBlocoAtual.Contains(x.IdItem)).Select(x => x.IdItem));
                        indiceAtual++;
                        qtdIdsRestantes -= tamanhoBloco;
                    }

                    itensDestaque = SqlConnection.Table<Item>().Where(x => ids.Contains(x.IdCamping)).OrderBy(x => x.Nome).ToList();
                }
                catch (Exception ex)
                {
                    Workaround.ShowExceptionOnlyDevolpmentMode(nameof(DBContract), nameof(FiltrarApenasCampingsDestaque), ex);
                }
                //finally
                //{
                //    Mutex.ReleaseMutex();
                //}

                return itensDestaque;
            }
        }

        public static List<Item> ListarItens(Expression<Func<Item, bool>>? where = null)
        {
            lock (Lock)
            {
                try
                {
                    //Mutex.WaitOne();

                    if (SqlConnection == null)
                    {
                        throw new NullReferenceException("Conexão SQL não foi criada");
                    }

                    var itens = where != null ? SqlConnection.Table<Item>().Where(where).ToList() : SqlConnection.Table<Item>().ToList();

                    itens.Where(x => x.type == "camping").ForEach(x => x.Identificadores = SqlConnection.Table<ItemIdentificador>().Where(y => y.IdItem == x.IdCamping).ToList());

                    return itens;
                }
                catch (Exception ex)
                {
                    Workaround.ShowExceptionOnlyDevolpmentMode(nameof(DBContract), nameof(ListarItens), ex);

                    return new List<Item>();
                }
                //finally
                //{
                //    Mutex.ReleaseMutex();
                //}
            }
        }

        public static List<ItemIdentificador> ListarItensIdentificadores(Expression<Func<ItemIdentificador, bool>> where)
        {
            lock (Lock)
            {
                try
                {
                    //Mutex.WaitOne();

                    if (SqlConnection == null)
                    {
                        throw new NullReferenceException("Conexão SQL não foi criada");
                    }

                    var listItemIdentificadores = SqlConnection.Table<ItemIdentificador>().Where(where).ToList();

                    return listItemIdentificadores;
                }
                catch (Exception ex)
                {
                    Workaround.ShowExceptionOnlyDevolpmentMode(nameof(DBContract), nameof(ListarItensIdentificadores), ex);

                    return new List<ItemIdentificador>();
                }
                //finally
                //{
                //    Mutex.ReleaseMutex();
                //}
            }
        }

        public static List<Cidade> ListarCidades()
        {
            if (SqlConnection == null)
            {
                throw new NullReferenceException("Conexão SQL não foi criada");
            }

            return SqlConnection.Table<Cidade>().ToList();
        }

        public static void ApagarItens()
        {
            lock (Lock)
            {
                if (SqlConnection == null)
                {
                    throw new NullReferenceException("Conexão SQL não foi criada");
                }

                //Mutex.WaitOne();
                SqlConnection.DeleteAll<Item>();
                //Mutex.ReleaseMutex();
            }
        }

        public static void ApagarItensIdentificadores()
        {
            lock (Lock)
            {
                if (SqlConnection == null)
                {
                    throw new NullReferenceException("Conexão SQL não foi criada");
                }

                //Mutex.WaitOne();
                SqlConnection.DeleteAll<ItemIdentificador>();
                //Mutex.ReleaseMutex();
            }
        }

        public static void ApagarCidades()
        {
            lock (Lock)
            {
                if (SqlConnection == null)
                {
                    throw new NullReferenceException("Conexão SQL não foi criada");
                }

                //Mutex.WaitOne();
                SqlConnection.DeleteAll<Cidade>();
                //Mutex.ReleaseMutex();
            }
        }

        public static Colaboracao Consultar()
        {
            if (SqlConnection == null)
            {
                throw new NullReferenceException("Conexão SQL não foi criada");
            }

            return SqlConnection.Table<Colaboracao>().FirstOrDefault();
        }
    }
}