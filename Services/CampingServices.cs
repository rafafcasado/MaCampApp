using System.Text;
using MaCamp.Services.DataAccess;
using MaCamp.Utils;

namespace MaCamp.Models.Services
{
    public static class CampingServices
    {
        public static bool ExistemCampingsBD()
        {
            var valorChaveDownloadConcluido = DBContract.Instance.ObterValorChave(AppConstants.Chave_DownloadCampingsCompleto);
            var downloadConcluido = !string.IsNullOrWhiteSpace(valorChaveDownloadConcluido) && Convert.ToBoolean(valorChaveDownloadConcluido);
            var tem = downloadConcluido && DBContract.Instance.ObterItem(i => i.IdPost == 0) != null;

            return tem;
        }

        public static async Task BaixarCampings(bool forcarAtualizacao = false)
        {
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
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
                var existemCampingsSalvos = ExistemCampingsBD();
                var identificadores = new List<ItemIdentificador>();

                if (!existemCampingsSalvos || forcarAtualizacao)
                {
                    App.BAIXANDO_CAMPINGS = true;

                    DBContract.Instance.InserirOuSubstituirModelo(new ChaveValor(AppConstants.Chave_DownloadCampingsCompleto, "false", Enumeradores.TipoChave.ControleInterno));

                    var campings = new List<Item>();
                    var chamadasWS = new List<Task>
                    {
                        Task.Run(async () =>
                        {
                            campings = await new WebService().GetListAsync<Item>(AppConstants.Url_ListaCampings, 0, string.Empty, string.Empty);
                        }),
                        Task.Run(async () =>
                        {
                            identificadores = await new WebService().GetListAsync<ItemIdentificador>(AppConstants.Url_ListaIdentificadores, 1);
                        })
                    };

                    Task.WaitAll(chamadasWS.ToArray());

                    //Armazenar ID's de todos os Campings Favoritados
                    var idsFavoritados = DBContract.Instance.BuscarIdsCampingsFavoritados();

                    DBContract.Instance.ApagarItens();
                    DBContract.Instance.InserirListaDeModelo(campings);
                    DBContract.Instance.ApagarItensIdentificadores();
                    DBContract.Instance.InserirListaDeModelo(identificadores);
                    DBContract.Instance.InserirOuSubstituirModelo(new ChaveValor(AppConstants.Chave_DownloadCampingsCompleto, "true", Enumeradores.TipoChave.ControleInterno));
                    DBContract.Instance.InserirOuSubstituirModelo(new ChaveValor
                    {
                        Chave = AppConstants.Chave_DataUltimaAtualizacaoConteudo,
                        Valor = DateTime.Now.ToString("yyyy/MM/dd")
                    });
                    //Realizar update informando os ID's dos favoritos
                    DBContract.Instance.AtualizarIdsCampingsFavoritados(idsFavoritados);
                    App.BAIXANDO_CAMPINGS = false;
                }
            }
        }

        public static async Task<List<Item>> CarregarCampings(bool utilizarFiltros = false)
        {
            await BaixarCampings();

            App.EXISTEM_CAMPINGS_DISPONIVEIS = ExistemCampingsBD();

            if (utilizarFiltros)
            {
                var campingsFiltrados = await CarregarCampingsFiltradosBD();

                return campingsFiltrados;
            }

            var campings = DBContract.Instance.ListarItens(x => x.IdPost == 0).OrderBy(x => x.Nome).ToList();

            return campings;
        }

        private static async Task<List<Item>> CarregarCampingsFiltradosBD()
        {
            var valorChaveEstadoSelecionado = DBContract.Instance.ObterValorChave(AppConstants.Filtro_EstadoSelecionado);
            var valorChaveCidadeSelecionada = DBContract.Instance.ObterValorChave(AppConstants.Filtro_CidadeSelecionada);
            var valorChaveLocalizacaoSelecionada = DBContract.Instance.ObterValorChave(AppConstants.Filtro_LocalizacaoSelecionada);
            var valorChaveBuscaCamping = DBContract.Instance.ObterValorChave(AppConstants.Filtro_NomeCamping);
            var usarLocalizacaoDoUsuario = valorChaveLocalizacaoSelecionada != null && Convert.ToBoolean(valorChaveLocalizacaoSelecionada);
            var valorFiltroEstabelecimentos = DBContract.Instance.ObterValorChave(AppConstants.Filtro_EstabelecimentoSelecionados);
            var valorFiltroComodidades = DBContract.Instance.ObterValorChave(AppConstants.Filtro_ServicoSelecionados) ?? string.Empty;
            var identificadoresEstabelecimento = "'" + valorFiltroEstabelecimentos?.Replace(",", "','") + "'";
            var identificadoresComodidades = "'" + valorFiltroComodidades.Replace(",", "','") + "'";
            var possuiFiltroCategoria = identificadoresEstabelecimento != "''";
            var possuiFiltroComodidades = identificadoresComodidades != "''";
            var idsCampingsQueAtendemCategoriaEComodidade = BuscarIdsCampingsPorCategoriaEComodidades(identificadoresEstabelecimento, identificadoresComodidades, possuiFiltroCategoria, possuiFiltroComodidades);

            //Se possui filtro, e nenhum id de camping foi retornado nenhum camping atende
            if ((possuiFiltroCategoria || possuiFiltroComodidades) && idsCampingsQueAtendemCategoriaEComodidade.Count == 0)
            {
                return new List<Item>();
            }

            if (!string.IsNullOrWhiteSpace(valorChaveBuscaCamping))
            {
                var resultadoBuscaDeCampings = DBContract.Instance.BuscarCampings(valorChaveBuscaCamping, valorChaveCidadeSelecionada, valorChaveEstadoSelecionado);

                return resultadoBuscaDeCampings;
            }

            var sbQuery = new StringBuilder();

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
                {
                    sbQuery.Append($" AND I.{nameof(Item.Cidade)} = '{valorChaveCidadeSelecionada.Replace("'", "''")}' ");
                }
            }

            sbQuery.Append($" ORDER BY {nameof(Item.Ordem)},{nameof(Item.Nome)} ASC ");

            var query = sbQuery.ToString();
            var campings = DBContract.Instance.QueryItens(query).ToList();

            if (usarLocalizacaoDoUsuario)
            {
                App.LOCALIZACAO_USUARIO = await Geolocation.GetLastKnownLocationAsync();

                if (App.LOCALIZACAO_USUARIO == null)
                {
                    App.LOCALIZACAO_USUARIO = await Geolocation.GetLocationAsync();
                }

                campings = campings.Where(x => x.Latitude != 0 && x.Longitude != 0).OrderBy(x => x.DistanciaDoUsuario).Take(20).ToList();
            }

            return campings;
        }

        public static string MontarUrlImagemTemporaria(string urlImagem)
        {
            var imagem = AppConstants.Url_DominioTemporario + "carregarImagem.aspx?src=" + urlImagem.Replace(AppConstants.Url_DominioOficial, "");

            return imagem;
        }

        private static List<int> BuscarIdsCampingsPorCategoriaEComodidades(string categorias, string comodidades, bool possuiFiltroCategoria, bool possuiFiltroComodidades)
        {
            var idsCampingsComComodidades = DBContract.Instance.ListarIdsCampingsComComodidades(possuiFiltroComodidades, comodidades);

            // Se não possui filtro por categorias, ignora essa busca
            // Se nenhum camping atende ao filtro de comodidades também ignora essa busca
            if (possuiFiltroCategoria && (!possuiFiltroComodidades || (possuiFiltroComodidades && idsCampingsComComodidades.Count > 0)))
            {
                return DBContract.Instance.ListarIdsCampingsComCategorias(categorias, possuiFiltroComodidades, idsCampingsComComodidades);
            }

            return idsCampingsComComodidades;
        }
    }
}