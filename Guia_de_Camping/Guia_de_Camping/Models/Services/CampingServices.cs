using Aspbrasil.DataAccess;
using Aspbrasil.Models.DataAccess;
using Plugin.Connectivity;
using Plugin.Geolocator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Aspbrasil.Models.Services
{
    public static class CampingServices
    {
        public static bool ExistemCampingsBD()
        {
            DBContract DB = DBContract.NewInstance();
            string valorChaveDownloadConcluido = DB.ObterValorChave(AppSettings.AppConstants.CHAVE_DOWNLOAD_CAMPINGS_COMPLETO);
            bool downloadConcluido = string.IsNullOrWhiteSpace(valorChaveDownloadConcluido) ? false : Convert.ToBoolean(valorChaveDownloadConcluido);
            bool tem = downloadConcluido && DB.ObterItem(i => i.IdPost == 0) != null;
            return tem;
        }

        public static async Task BaixarCampings(bool forcarAtualizacao = false)
        {
            if (!CrossConnectivity.Current.IsConnected)
            {
                App.BAIXANDO_CAMPINGS = false;
                return;
            }

            // Se já estiver baixando os campings
            if (App.BAIXANDO_CAMPINGS)
            {
                // Espera até que o download seja concluído
                while (App.BAIXANDO_CAMPINGS)
                {
                    await Task.Delay(2000);
                }
            }
            else
            {
                bool existemCampingsSalvos = ExistemCampingsBD();
                if (!existemCampingsSalvos || forcarAtualizacao)
                {
                    App.BAIXANDO_CAMPINGS = true;

                    DBContract DB = DBContract.NewInstance();

                    DB.InserirOuSubstituirModelo(new ChaveValor(AppSettings.AppConstants.CHAVE_DOWNLOAD_CAMPINGS_COMPLETO, "false", TipoChave.ControleInterno));


                    List<Item> campings = new List<Item>();
                    List<ItemIdentificador> identificadores = new List<ItemIdentificador>();

                    List<Task> chamadasWS = new List<Task>();
                    chamadasWS.Add(Task.Run(async () =>
                    {
                        campings = await new WebService<Item>().Get("https://guiadecampings.homologacao.net/api/Campings/GetCampings", 0, string.Empty, string.Empty);
                    }));
                    chamadasWS.Add(Task.Run(async () =>
                    {
                        identificadores = await new WebService<ItemIdentificador>().Get("https://guiadecampings.homologacao.net/api/Campings/GetIdentificadores", 1);
                    }));
                    Task.WaitAll(chamadasWS.ToArray());
                    //Armazenar ID's de todos os Campings Favoritados
                    List<int> IdsFavoritados = DB.BuscarIdsCampingsFavoritados();
                    DB.ApagarItens();
                    DB.InserirListaDeModelo(campings);
                    DB.ApagarItensIdentificadores();
                    DB.InserirListaDeModelo(identificadores);
                    DB.InserirOuSubstituirModelo(new ChaveValor(AppSettings.AppConstants.CHAVE_DOWNLOAD_CAMPINGS_COMPLETO, "true", TipoChave.ControleInterno));
                    DB.InserirOuSubstituirModelo(new ChaveValor { Chave = AppSettings.AppConstants.CHAVE_DATA_ULTIMA_ATUALIZACAO_CONTEUDO, Valor = DateTime.Now.ToString("yyyy/MM/dd") });
                    //Realizar update informando os ID's dos favoritos
                    DB.AtualizarIdsCampingsFavoritados(IdsFavoritados);
                    
                    App.BAIXANDO_CAMPINGS = false;
                }
            }
        }


        public static async Task<List<Item>> CarregarCampings(bool utilizarFiltros = false)
        {
            List<Item> campings = new List<Item>();
            List<ItemIdentificador> identificadores = new List<ItemIdentificador>();

            await BaixarCampings();

            App.EXISTEM_CAMPINGS_DISPONIVEIS = ExistemCampingsBD();

            if (utilizarFiltros)
            {
                campings = await CarregarCampingsFiltradosBD();
            }
            else
            {
                DBContract DB = DBContract.NewInstance();

                campings = DB.ListarItens(i => i.IdPost == 0).OrderBy(i => i.Nome).ToList();
            }
            return campings;
        }

        private static async Task<List<Item>> CarregarCampingsFiltradosBD()
        {
            DBContract DB = DBContract.NewInstance();

            string valorChaveEstadoSelecionado = DB.ObterValorChave("FILTROS_ESTADO_SELECIONADO");
            string valorChaveCidadeSelecionada = DB.ObterValorChave("FILTROS_CIDADE_SELECIONADA");
            string valorChaveLocalizacaoSelecionada = DB.ObterValorChave("FILTROS_LOCALIZACAO_SELECIONADA");
            string valorChaveBuscaCamping = DB.ObterValorChave("FILTROS_NOME_DO_CAMPING");

            bool usarLocalizacaoDoUsuario = valorChaveLocalizacaoSelecionada != null && Convert.ToBoolean(valorChaveLocalizacaoSelecionada);

            string valorFiltroEstabelecimentos = DB.ObterValorChave("FILTROS_ESTABELECIMENTO_SELECIONADOS");
            string valorFiltroComodidades = DB.ObterValorChave("FILTROS_SERVICO_SELECIONADOS");

            if (valorFiltroComodidades == null) { valorFiltroComodidades = string.Empty; }

            string identificadoresEstabelecimento = "'" + valorFiltroEstabelecimentos.Replace(",", "','") + "'";
            string identificadoresComodidades = "'" + valorFiltroComodidades.Replace(",", "','") + "'";

            bool possuiFiltroCategoria = identificadoresEstabelecimento != "''";
            bool possuiFiltroComodidades = identificadoresComodidades != "''";

            List<int> idsCampingsQueAtendemCategoriaEComodidade = BuscarIdsCampingsPorCategoriaEComodidades(identificadoresEstabelecimento, identificadoresComodidades, possuiFiltroCategoria, possuiFiltroComodidades);

            //Se possui filtro, e nenhum id de camping foi retornado
            if ((possuiFiltroCategoria || possuiFiltroComodidades) && idsCampingsQueAtendemCategoriaEComodidade.Count == 0)
            {
                //Nenhum camping atende
                return new List<Item>();
            }

            if (!string.IsNullOrWhiteSpace(valorChaveBuscaCamping))
            {
                List<Item> resultadoBuscaDeCampings = DB.BuscarCampings(valorChaveBuscaCamping, valorChaveCidadeSelecionada, valorChaveEstadoSelecionado);
                return resultadoBuscaDeCampings;
            }
            else { 

            StringBuilder sbQuery = new StringBuilder();
            sbQuery.Append($"SELECT * ");
            sbQuery.Append($" FROM {nameof(Item)} I ");
            sbQuery.Append($" WHERE ");
            sbQuery.Append($" I.{nameof(Item.IdPost)} = 0 ");

            if (idsCampingsQueAtendemCategoriaEComodidade.Count > 0)
            {
                sbQuery.Append($" AND I.{nameof(Item.IdCamping)} IN ({string.Join(",", idsCampingsQueAtendemCategoriaEComodidade)}) ");
            }

            if (!usarLocalizacaoDoUsuario && valorChaveEstadoSelecionado != null)
            {
                sbQuery.Append($" AND I.{nameof(Item.Estado)} = '{valorChaveEstadoSelecionado}' ");
                if (valorChaveCidadeSelecionada != null)
                    sbQuery.Append($" AND I.{nameof(Item.Cidade)} = '{valorChaveCidadeSelecionada.Replace("'", "''")}' ");
            }

            sbQuery.Append($" ORDER BY {nameof(Item.Ordem)},{nameof(Item.Nome)} ASC ");

            string query = sbQuery.ToString();
            List<Item> campings = DB.QueryItens(query).ToList();
            
            if (usarLocalizacaoDoUsuario)
            {
                App.LOCALIZACAO_USUARIO = await CrossGeolocator.Current.GetLastKnownLocationAsync();
                if (App.LOCALIZACAO_USUARIO == null) { App.LOCALIZACAO_USUARIO = await CrossGeolocator.Current.GetPositionAsync(); }
                campings = campings.Where(i => i.Latitude != 0 && i.Longitude != 0).OrderBy(i => i.DistanciaDoUsuario).Take(20).ToList();
            }

            return campings;
            }
        }

        public static string MontarUrlImagemTemporaria(string urlImagem)
        {
            string dominioTemporario = "https://macamptecnologia1.websiteseguro.com/";
            string dominioOficial = "https://macamp.com.br/";
            string imagem = null;
            imagem = dominioTemporario + "carregarImagem.aspx?src=" + urlImagem.Replace(dominioOficial, "");

            return imagem;
        }


        private static List<int> BuscarIdsCampingsPorCategoriaEComodidades(string categorias, string comodidades, bool possuiFiltroCategoria, bool possuiFiltroComodidades)
        {
            var db = DBContract.NewInstance();
            List<int> idsCampingsQueAtendemABusca = new List<int>();

            List<int> idsCampingsComComodidades = db.ListarIdsCampingsComComodidades(possuiFiltroComodidades, comodidades);

            // Se não possui filtro por categorias, ignora essa busca
            // Se nenhum camping atende ao filtro de comodidades também ignora essa busca
            if (possuiFiltroCategoria && (!possuiFiltroComodidades || (possuiFiltroComodidades && idsCampingsComComodidades.Count > 0)))
            {
                idsCampingsQueAtendemABusca = db.ListarIdsCampingsComCategorias(categorias, possuiFiltroComodidades, idsCampingsComComodidades);
            }
            else
            {
                idsCampingsQueAtendemABusca = idsCampingsComComodidades;
            }
            return idsCampingsQueAtendemABusca;
        }
       
    }
}
