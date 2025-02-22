﻿using System.Diagnostics;
using System.Linq.Expressions;
using System.Text;
using MaCamp.Models;
using MaCamp.Utils;
using SQLite;

namespace MaCamp.Services.DataAccess
{
    public static class DBContract
    {
        public static SQLiteConnection? SqlConnection { get; set; }

        private static object Lock => new object();
        private static Mutex Mutex => new Mutex();

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
                    Debug.WriteLine("<<Exceção>> " + ex);

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
                    Debug.WriteLine("<<Exceção>> " + ex);

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
                    Debug.WriteLine("<<Exceção>> " + ex);

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
                    Debug.WriteLine("<<Exceção>> " + ex);

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

                    var valor = SqlConnection.Table<ChaveValor>().FirstOrDefault(c => c.Chave == chave)?.Valor;

                    return valor;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("<<Exceção>> " + ex);

                    return default;
                }
                //finally
                //{
                //    Mutex.ReleaseMutex();
                //}
            }
        }

        public class RetornoIdItem
        {
            public int IdItem { get; set; }
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

            if (categorias.Count() == 104)
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

                    var nomeNormalizado = nomeDoCamping?.RemoveDiacritics().ToLower();
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
                    Debug.WriteLine("<<Exceção>> " + ex);

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
                    Debug.WriteLine("<<Exceção>> " + ex);

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
                    Debug.WriteLine("<<Exceção>> " + ex);

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
                    Debug.WriteLine("<<Exceção>> " + ex);
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
                    Debug.WriteLine("<<Exceção>> " + ex);

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
                    Debug.WriteLine("<<Exceção>> " + ex);

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