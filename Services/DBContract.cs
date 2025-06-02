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
        private static SQLiteConnection? SqlConnection { get; set; }

        static DBContract()
        {
            SyncLock = new object();
        }

        public static void Initialize()
        {
            SqlConnection = GetConnection(AppConstants.SqliteFilename);

            if (SqlConnection != null)
            {
                SqlConnection.InsertOrReplace(new ChaveValor
                {
                    Chave = AppConstants.Filtro_ServicoSelecionados,
                    Valor = null
                });
                SqlConnection.InsertOrReplace(new ChaveValor
                {
                    Chave = AppConstants.Filtro_EstadoSelecionado,
                    Valor = null
                });
                SqlConnection.InsertOrReplace(new ChaveValor
                {
                    Chave = AppConstants.Filtro_CidadeSelecionada,
                    Valor = null
                });
                SqlConnection.InsertOrReplace(new ChaveValor
                {
                    Chave = AppConstants.Filtro_NomeCamping,
                    Valor = null
                });
            }
        }

        private static SQLiteConnection? GetConnection(string filename)
        {
            try
            {
                var path = Path.Combine(App.PATH, filename);
                var connection = new SQLiteConnection(path, SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.SharedCache | SQLiteOpenFlags.FullMutex);

                connection.CreateTables<Item, ItemIdentificador, ChaveValor, Cidade, Colaboracao>();

                return connection;
            }
            catch (Exception ex)
            {
                Workaround.ShowExceptionOnlyDevolpmentMode(nameof(DBContract), nameof(GetConnection), ex);
            }

            return null;
        }

        private static bool VerifyDatabaseIntegrity(string filename)
        {
            try
            {
                var path = Path.Combine(App.PATH, filename);
                var connection = new SQLiteConnection(path);
                var result = connection.ExecuteScalar<string>("PRAGMA integrity_check;");

                connection.Close();

                return result == "ok";
            }
            catch (Exception ex)
            {
                Workaround.ShowExceptionOnlyDevolpmentMode(nameof(DBContract), nameof(VerifyDatabaseIntegrity), ex);

                return false;
            }
        }

        private static bool Update(bool clean, Dictionary<Type, List<object>> dataDictionary, ProgressoVisual? progressoVisual = null)
        {
            var currentDatabasePath = Path.Combine(App.PATH, AppConstants.SqliteFilename);
            var temporaryDatabasePath = Path.Combine(App.PATH, AppConstants.SqliteTemporaryFilename);

            ProgressoVisual.AumentarTotal(progressoVisual, 8 + dataDictionary.Count);

            lock (SyncLock)
            {
                try
                {
                    ProgressoVisual.AumentarAtual(progressoVisual);

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

                    var temporaryConnection = GetConnection(AppConstants.SqliteTemporaryFilename);

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
                                temporaryConnection.DeleteAll(table);
                            }
                        }

                        temporaryConnection.RunInTransaction(() =>
                        {
                            foreach (var value in values)
                            {
                                temporaryConnection.InsertOrReplace(value);
                            }
                        });

                        ProgressoVisual.AumentarAtual(progressoVisual);
                    }

                    temporaryConnection.InsertOrReplace(new ChaveValor(AppConstants.Chave_DownloadCampingsCompleto, Convert.ToString(true), TipoChave.ControleInterno));
                    temporaryConnection.InsertOrReplace(new ChaveValor
                    {
                        Chave = AppConstants.Chave_DataUltimaAtualizacaoConteudo,
                        Valor = DateTime.Now.ToString("yyyy/MM/dd")
                    });

                    ProgressoVisual.AumentarAtual(progressoVisual);

                    var checkIntegrity = VerifyDatabaseIntegrity(AppConstants.SqliteTemporaryFilename);

                    if (!checkIntegrity)
                    {
                        throw new Exception("Integridade do banco atualizado falhou.");
                    }

                    ProgressoVisual.AumentarAtual(progressoVisual);

                    if (SqlConnection != null)
                    {
                        SqlConnection.Close();
                    }

                    if (File.Exists(currentDatabasePath))
                    {
                        File.Delete(currentDatabasePath);
                    }

                    File.Move(temporaryDatabasePath, currentDatabasePath);

                    SqlConnection = GetConnection(AppConstants.SqliteFilename);

                    ProgressoVisual.AumentarAtual(progressoVisual);

                    return true;
                }
                catch (Exception ex)
                {
                    Workaround.ShowExceptionOnlyDevolpmentMode(nameof(DBContract), nameof(Update), ex, false);

                    if (SqlConnection != null)
                    {
                        SqlConnection.Close();
                    }

                    if (File.Exists(currentDatabasePath))
                    {
                        File.Delete(currentDatabasePath);
                    }

                    SqlConnection = GetConnection(AppConstants.SqliteFilename);

                    return false;
                }
            }
        }

        public static bool Update<T>(bool clean, IEnumerable<T> listPrimaryData, ProgressoVisual? progressoVisual = null)
        {
            var dataDictionary = new Dictionary<Type, List<object>>
            {
                { typeof(T), listPrimaryData.OfType<object>().ToList() }
            };

            return Update(clean, dataDictionary, progressoVisual);
        }

        public static bool Update<T, TK>(bool clean, IEnumerable<T> listPrimaryData, IEnumerable<TK> listaSecondaryData, ProgressoVisual? progressoVisual = null)
        {
            var dataDictionary = new Dictionary<Type, List<object>>
            {
                { typeof(T), listPrimaryData.OfType<object>().ToList() },
                { typeof(TK), listaSecondaryData.OfType<object>().ToList() }
            };

            return Update(clean, dataDictionary, progressoVisual);
        }

        public static bool Update<T, TK, TR>(bool clean, IEnumerable<T> listPrimaryData, IEnumerable<TK> listaSecondaryData, IEnumerable<TR> listaTertiaryData, ProgressoVisual? progressoVisual = null)
        {
            var dataDictionary = new Dictionary<Type, List<object>>
            {
                { typeof(T), listPrimaryData.OfType<object>().ToList() },
                { typeof(TK), listaSecondaryData.OfType<object>().ToList() },
                { typeof(TR), listaTertiaryData.OfType<object>().ToList() }
            };

            return Update(clean, dataDictionary, progressoVisual);
        }

        public static bool Update<T>(T value)
        {
            lock (SyncLock)
            {
                try
                {
                    if (SqlConnection != null)
                    {
                        SqlConnection.InsertOrReplace(value);

                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Workaround.ShowExceptionOnlyDevolpmentMode(nameof(DBContract), nameof(Update), ex, false);
                }
            }

            return false;
        }

        public static bool UpdateKeyValue(string key, string? value, TipoChave type = default)
        {
            var keyValue = new ChaveValor
            {
                Chave = key,
                Valor = value,
                Tipo = type
            };

            return Update(keyValue);
        }

        public static T? Get<T>(Expression<Func<T, bool>>? predicate = null) where T : new()
        {
            lock (SyncLock)
            {
                try
                {
                    if (SqlConnection != null)
                    {
                        if (predicate != null)
                        {
                            return SqlConnection.Table<T>().FirstOrDefault(predicate);
                        }

                        return SqlConnection.Table<T>().FirstOrDefault();
                    }
                }
                catch (Exception ex)
                {
                    Workaround.ShowExceptionOnlyDevolpmentMode(nameof(DBContract), nameof(Get), ex);
                }
            }

            return default;
        }

        public static string? GetKeyValue(string key)
        {
            var keyValue = Get<ChaveValor>(x => x.Chave == key);

            if (keyValue != null)
            {
                return keyValue.Valor;
            }

            return null;
        }

        public static List<T> Query<T>(string query, params object[] args) where T : new()
        {
            lock (SyncLock)
            {
                try
                {
                    if (SqlConnection != null)
                    {
                        var list = SqlConnection.Query<T>(query, args);

                        return list;
                    }
                }
                catch (Exception ex)
                {
                    Workaround.ShowExceptionOnlyDevolpmentMode(nameof(DBContract), nameof(Query), ex);
                }
            }

            return new List<T>();
        }

        public static List<T> List<T>(Expression<Func<T, bool>>? predicate = null) where T : new()
        {
            lock (SyncLock)
            {
                try
                {
                    if (SqlConnection != null)
                    {
                        if (predicate != null)
                        {
                            return SqlConnection.Table<T>().Where(predicate).ToList();
                        }

                        return SqlConnection.Table<T>().ToList();
                    }
                }
                catch (Exception ex)
                {
                    Workaround.ShowExceptionOnlyDevolpmentMode(nameof(DBContract), nameof(List), ex);
                }
            }

            return new List<T>();
        }

        public static List<Item> ListCampings(string nomeDoCamping, string? cidade, string? estado)
        {
            lock (SyncLock)
            {
                try
                {
                    if (SqlConnection != null)
                    {
                        var nomeNormalizado = nomeDoCamping.RemoveDiacritics().ToLower();
                        var estadoNormalizado = estado?.RemoveDiacritics().ToLower();
                        var cidadeNormalizada = cidade?.RemoveDiacritics().ToLower();
                        var list = SqlConnection.Table<Item>().ToList();
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
                    Workaround.ShowExceptionOnlyDevolpmentMode(nameof(DBContract), nameof(ListCampings), ex);
                }
            }

            return new List<Item>();
        }

        public static List<int> ListIdCampingsComComodidades(bool possuiFiltroComodidades, string comodidades)
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
                    var list = Query<RetornoIdItem>(query);

                    return list.Select(x => x.IdItem).ToList();
                }
            }
            catch (Exception ex)
            {
                Workaround.ShowExceptionOnlyDevolpmentMode(nameof(DBContract), nameof(ListCampings), ex);
            }

            return new List<int>();
        }

        public static List<int> ListIdCampingsComCategorias(string categorias, bool possuiFiltroComodidades, List<int> idsCampingsComComodidades)
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
                var list = Query<RetornoIdItem>(query);

                return list.Select(x => x.IdItem).ToList();
            }
            catch (Exception ex)
            {
                Workaround.ShowExceptionOnlyDevolpmentMode(nameof(DBContract), nameof(ListCampings), ex);
            }

            return new List<int>();
        }
    }
}