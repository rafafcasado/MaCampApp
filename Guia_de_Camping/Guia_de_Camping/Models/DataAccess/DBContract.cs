using Aspbrasil.Dependencias;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using Xamarin.Forms;

namespace Aspbrasil.Models.DataAccess
{
    public class DBContract
    {
        private static DBContract dbContract;
        private static SQLiteConnection _sqlconnection;

        private static Mutex _mutex = new Mutex();

        public static DBContract NewInstance()
        {
            if (dbContract == null)
            {
                dbContract = new DBContract();
                Initialize();
            }
            return dbContract;
        }

        private static void Initialize()
        {
            _sqlconnection = DependencyService.Get<ISQLite>().ObterConexao();
            CreateTablesResult resultadoCriacaoTabelas = _sqlconnection.CreateTables<Item, ItemIdentificador, ChaveValor, Cidade, Colaboracao>();
        }

        #region Create
        /// <summary>
        /// Insere um objeto no BD.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Objeto inserido, com a Primary Key atualizada</returns>
        public object InserirModelo(object model)
        {
            _mutex.WaitOne();
            try
            {
                int qtdInserida = _sqlconnection.Insert(model);

                _mutex.ReleaseMutex();

                if (qtdInserida == 1)
                    return model;
                else
                    return null;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("<<Exceção>> " + e.ToString());
                _mutex.ReleaseMutex();
                return 0;
            }
        }

        public int InserirListaDeModelo(IEnumerable<object> model)
        {
            _mutex.WaitOne();
            try
            {
                _sqlconnection.BeginTransaction();
                int qtdInserida = _sqlconnection.InsertAll(model);
                _sqlconnection.Commit();
                _mutex.ReleaseMutex();
                return qtdInserida;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("<<Exceção>> " + e.ToString());
                _mutex.ReleaseMutex();
                return 0;
            }
        }
        public int InserirOuSubstituirModelo(object model)
        {
            _mutex.WaitOne();
            try
            {
                int result = _sqlconnection.InsertOrReplace(model);
                _mutex.ReleaseMutex();
                return result;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("<<Exceção>> " + e.ToString());
                _mutex.ReleaseMutex();
                return 0;
            }
        }
        #endregion

        #region Retrieve
        public Item ObterItem(Expression<Func<Item, bool>> where)
        {
            try
            {
                _mutex.WaitOne();
                var i = _sqlconnection.Table<Item>().FirstOrDefault(where);
                _mutex.ReleaseMutex();

                return i;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("<<Exceção>> " + e.ToString());
                _mutex.ReleaseMutex();
                return null;
            }
        }

        public string ObterValorChave(string chave)
        {
            try
            {
                _mutex.WaitOne();
                string valor = _sqlconnection.Table<ChaveValor>().FirstOrDefault(c => c.Chave == chave)?.Valor;
                _mutex.ReleaseMutex();
                return valor;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("<<Exceção>> " + e.ToString());
                _mutex.ReleaseMutex();
                return string.Empty;
            }
        }

        public class RetornoIdItem
        {
            public int IdItem { get; set; }
        }

        public List<int> ListarIdsCampingsComComodidades(bool possuiFiltroComodidades, string comodidades)
        {
            if (!possuiFiltroComodidades) { return new List<int>(); }

            int qtdFiltrosComodidades = comodidades.Split(',').Count();
            StringBuilder sbQueryComodidades = new StringBuilder();
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
            string query = sbQueryComodidades.ToString();
            return QueryIdsIdentificadorCampings(query).Select(i => i.IdItem).ToList();
        }


        public List<int> ListarIdsCampingsComCategorias(string categorias, bool possuiFiltroComodidades, List<int> idsCampingsComComodidades)
        {
            string queryIdsInCampingsComComodidade = "";
            if (possuiFiltroComodidades && idsCampingsComComodidades.Count > 0)
            {
                queryIdsInCampingsComComodidade = $" (II.{nameof(ItemIdentificador.IdItem)} IN ({string.Join(",", idsCampingsComComodidades)}) ) AND ";
            }

            StringBuilder sbQueryCategorias = new StringBuilder();
            sbQueryCategorias.Append($"SELECT II.{nameof(ItemIdentificador.IdItem)} ");
            sbQueryCategorias.Append($" FROM {nameof(ItemIdentificador)} II ");
            sbQueryCategorias.Append($" WHERE ");

            string identificadoresQueNaoEstaoNosFiltros = string.Empty;
            string complementoDaQuery = string.Empty, complementoCampings = string.Empty;

            if (categorias.Count() == 104)
            {
                identificadoresQueNaoEstaoNosFiltros =
                    "'CampingEmFuncionamento'" +
                    ",'CampingEmReformas'" +
                    ",'CampingemSituaçãoIncerta'" +
                    ",'CampingInformal'" +
                   ",'CampingParaGruposouEventos'" +
                    ",'Destaque'" +
                    ",'Empresa'" +
                    ",'EstacionamentoUrbanoouPraça'" +
                    ",'Hotel/Pousada/Hostel'" +
                    ",'PostodeCombustível'" +
                    ",'Quiosque/Restaurante/PontoTurístico'" +
                    ",'RVPark(ExclusivoRV`s)'" +
                    ",'RVPark(SóMensalistas)'";
                complementoDaQuery = $" OR II.{nameof(ItemIdentificador.Identificador)} IN ({identificadoresQueNaoEstaoNosFiltros})  ";
            }
            else
            {
                if (categorias.Contains("Campings"))
                {
                    string identificadoresCampings =
                     "'CampingEmFuncionamento'" +
                     ",'CampingEmReformas'" +
                     ",'CampingemSituaçãoIncerta'" +
                     ",'CampingInformal'" +
                    ",'CampingParaGruposouEventos'" +
                     ",'Destaque'";
                    complementoDaQuery += $" OR II.{nameof(ItemIdentificador.Identificador)} IN ({identificadoresCampings})  ";
                }
                if (categorias.Contains("PontodeApoioaRV`s"))
                {
                    string identificadoresRVs =
                    "'RVPark(ExclusivoRV`s)'" +
                    ",'RVPark(SóMensalistas)'";
                    complementoDaQuery += $" OR II.{nameof(ItemIdentificador.Identificador)} IN ({identificadoresRVs})  ";
                }
            }

            sbQueryCategorias.Append($" {queryIdsInCampingsComComodidade} (II.{nameof(ItemIdentificador.Identificador)} IN ({categorias}){complementoDaQuery})");

            sbQueryCategorias.Append($" GROUP BY {nameof(ItemIdentificador.IdItem)} ");
            string query = sbQueryCategorias.ToString();
            return QueryIdsIdentificadorCampings(query).Select(i => i.IdItem).ToList();
        }

        public List<int> BuscarIdsCampingsFavoritados()
        {
            List<int> idsCampingsQueAtendemABusca = new List<int>();

            // Se não possui filtro por categorias, ignora essa busca
            // Se nenhum camping atende ao filtro de comodidades também ignora essa busca
            StringBuilder sbQuery = new StringBuilder();
            sbQuery.Append($"SELECT * ");
            sbQuery.Append($" FROM {nameof(Item)} ");
            sbQuery.Append($" WHERE ");
            sbQuery.Append($" {nameof(Item.Favoritado)} = 1 ");

            string query = sbQuery.ToString();
            idsCampingsQueAtendemABusca = QueryItens(query).Select(i => i.IdCamping).ToList();
            //List<Item> campings = QueryItens(query).ToList();

            return idsCampingsQueAtendemABusca;
        }

        public List<Item> ListarCampings()
        {
            List<Item> campings = _sqlconnection.Table<Item>().ToList();

            return campings;
        }

        public List<Item> BuscarCampings(string nomeDoCamping, string cidade, string estado)
        {
            List<Item> BuscaDeCampings;

            StringBuilder sbQuery = new StringBuilder();
            sbQuery.Append($"SELECT * ");
            sbQuery.Append($" FROM {nameof(Item)} ");
            sbQuery.Append($" WHERE ");
            sbQuery.Append($" {nameof(Item.Nome)}");

            if (!string.IsNullOrWhiteSpace(nomeDoCamping))
            { sbQuery.Append($" LIKE '%{nomeDoCamping}%'"); }

            if (!string.IsNullOrWhiteSpace(cidade))
            {
                sbQuery.Append($"and {nameof(Item.Cidade)} = '{cidade}'");
            }
            if (!string.IsNullOrWhiteSpace(estado))
            {
                sbQuery.Append($" and {nameof(Item.Estado)} = '{estado}'");
            }

            string query = sbQuery.ToString();
            BuscaDeCampings = QueryItens(query).ToList();
            //var resultado = campings.Where(i => i.Nome.Contains(nomeDoCamping)).ToList();

            DBContract DB = new DBContract();
            //DB.InserirOuSubstituirModelo(DB.InserirOuSubstituirModelo(new ChaveValor { Chave = "FILTROS_NOME_DO_CAMPING", Valor = "" }));

            return BuscaDeCampings;
        }

        public void AtualizarIdsCampingsFavoritados(List<int> campingsFavoritados)
        {
            string idCampings = "";
            idCampings = string.Join(",", campingsFavoritados);

            // Se não possui filtro por categorias, ignora essa busca
            // Se nenhum camping atende ao filtro de comodidades também ignora essa busca
            StringBuilder sbQuery = new StringBuilder();
            sbQuery.Append($"UPDATE {nameof(Item)} set {nameof(Item.Favoritado)} = 1");
            sbQuery.Append($" WHERE ");
            sbQuery.Append($" {nameof(Item.IdCamping)} in (");
            sbQuery.Append(idCampings);
            sbQuery.Append($")");

            string query = sbQuery.ToString();
            QueryItens(query);

        }



        public List<RetornoIdItem> QueryIdsIdentificadorCampings(string query)
        {
            try
            {
                _mutex.WaitOne();
                var itens = _sqlconnection.Query<RetornoIdItem>(query);
                _mutex.ReleaseMutex();
                return itens;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("<<Exceção>> " + e.ToString());
                _mutex.ReleaseMutex();
                return new List<RetornoIdItem>();
            }
        }

        public List<Item> QueryItens(string query, params object[] args)
        {
            //try
            //{
            _mutex.WaitOne();
            var itens = _sqlconnection.Query<Item>(query, args);
            _mutex.ReleaseMutex();
            return itens;
            //}
            //catch (Exception e)
            //{
            //    System.Diagnostics.Debug.WriteLine("<<Exceção>> " + e.ToString());
            //    _mutex.ReleaseMutex();
            //    return new List<Item>();
            //}
        }

        public List<Item> FiltrarApenasCampingsDestaque(List<int> idsCampings)
        {
            List<int> ids = new List<int>();
            List<Item> itensDestaque = new List<Item>();
            try
            {
                _mutex.WaitOne();

                int indiceAtual = 0, tamanhoBloco = 100;
                int qtdIdsRestantes = idsCampings.Count;
                List<int> idsBlocoAtual;
                while (qtdIdsRestantes > 0)
                {
                    idsBlocoAtual = new List<int>();
                    idsBlocoAtual = idsCampings.Skip(indiceAtual * tamanhoBloco).Take(tamanhoBloco).ToList();
                    ids.AddRange(_sqlconnection.Table<ItemIdentificador>().Where(i => i.Identificador == "Destaque" && idsBlocoAtual.Contains(i.IdItem)).Select(i => i.IdItem));
                    indiceAtual++;
                    qtdIdsRestantes -= tamanhoBloco;
                }

                _mutex.ReleaseMutex();
                itensDestaque = _sqlconnection.Table<Item>().Where(i => ids.Contains(i.IdCamping)).OrderBy(c => c.Nome).ToList();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("<<Exceção>> " + e.ToString());
                _mutex.ReleaseMutex();
            }
            return itensDestaque;
        }

        public List<Item> ListarItens(Expression<Func<Item, bool>> where)
        {
            try
            {
                _mutex.WaitOne();
                var itens = _sqlconnection.Table<Item>().Where(where).ToList();
                foreach (var item in itens)
                {
                    if (item.type == "camping")
                    {
                        item.Identificadores = _sqlconnection.Table<ItemIdentificador>().Where(i => i.IdItem == item.IdCamping).ToList();
                    }
                }
                _mutex.ReleaseMutex();

                return itens;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("<<Exceção>> " + e.ToString());
                _mutex.ReleaseMutex();
                return new List<Item>();
            }
        }

        public List<ItemIdentificador> ListarItensIdentificadores(Expression<Func<ItemIdentificador, bool>> where)
        {
            try
            {
                _mutex.WaitOne();
                var ii = _sqlconnection.Table<ItemIdentificador>().Where(where).ToList();
                _mutex.ReleaseMutex();
                return ii;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("<<Exceção>> " + e.ToString());
                _mutex.ReleaseMutex();
                return new List<ItemIdentificador>();
            }
        }

        public List<Cidade> ListarCidades()
        {
            return _sqlconnection.Table<Cidade>().ToList();
        }
        #endregion

        #region Delete
        public void ApagarItens()
        {
            _mutex.WaitOne();
            _sqlconnection.DeleteAll<Item>();
            _mutex.ReleaseMutex();
        }

        public void ApagarItensIdentificadores()
        {
            _mutex.WaitOne();
            _sqlconnection.DeleteAll<ItemIdentificador>();
            _mutex.ReleaseMutex();
        }

        public void ApagarCidades()
        {
            _mutex.WaitOne();
            _sqlconnection.DeleteAll<Cidade>();
            _mutex.ReleaseMutex();
        }
        #endregion

        public Colaboracao Consultar()
        {
            return _sqlconnection.Table<Colaboracao>().FirstOrDefault();
        }

    }
}