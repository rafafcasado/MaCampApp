using System.Text;
using MaCamp.Models;
using MaCamp.Services.DataAccess;
using MaCamp.Utils;
using static MaCamp.Utils.Enumeradores;

namespace MaCamp.Services
{
    public static class CampingServices
    {
        public static bool ExistemCampingsBD()
        {
            var valorChaveDownloadConcluido = DBContract.GetKeyValue(AppConstants.Chave_DownloadCampingsCompleto);
            var downloadConcluido = !string.IsNullOrWhiteSpace(valorChaveDownloadConcluido) && Convert.ToBoolean(valorChaveDownloadConcluido);
            var tem = downloadConcluido && DBContract.Get<Item>(x => x.IdPost == 0) != null;

            return tem;
        }

        public static async Task BaixarCampings(bool forcarAtualizacao, ProgressoVisual? progressoVisual = null)
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

                if (!existemCampingsSalvos || forcarAtualizacao)
                {
                    App.BAIXANDO_CAMPINGS = true;

                    ProgressoVisual.AumentarTotal(progressoVisual, 3);

                    DBContract.UpdateKeyValue(AppConstants.Chave_DownloadCampingsCompleto, "false", TipoChave.ControleInterno);

                    var campings = new List<Item>();
                    var identificadores = new List<ItemIdentificador>();
                    var chamadasWS = new List<Task>
                    {
                        Task.Run(async () =>
                        {
                            campings = await new WebService().GetListAsync<Item>(AppConstants.Url_ListaCampings, 0);
                            ProgressoVisual.AumentarAtual(progressoVisual);
                        }),
                        Task.Run(async () =>
                        {
                            identificadores = await new WebService().GetListAsync<ItemIdentificador>(AppConstants.Url_ListaIdentificadores, 1);

                            Parallel.ForEach(identificadores, x =>
                            {
                                x.TipoIdentificador = Enum.TryParse<TipoIdentificador>(x.Identificador, out var tipoIdentificador) ? tipoIdentificador : null;
                            });

                            ProgressoVisual.AumentarAtual(progressoVisual);
                        })
                    };

                    await Task.WhenAll(chamadasWS);

                    Parallel.ForEach(campings, x => x.Identificadores = identificadores.Where(y => y.IdItem == x.IdCamping).ToList());

                    DBContract.Update(true, campings, identificadores, progressoVisual);

                    ProgressoVisual.AumentarAtual(progressoVisual);

                    App.BAIXANDO_CAMPINGS = false;
                }
            }
        }

        public static async Task<List<Item>> CarregarCampings(bool utilizarFiltros = false)
        {
            await BaixarCampings(false);

            App.EXISTEM_CAMPINGS_DISPONIVEIS = ExistemCampingsBD();

            if (utilizarFiltros)
            {
                var campingsFiltrados = await CarregarCampingsFiltradosBD();

                return campingsFiltrados;
            }

            var campings = DBContract.List<Item>(x => x.IdPost == 0).OrderBy(x => x.Nome).ToList();

            return campings;
        }

        private static async Task<List<Item>> CarregarCampingsFiltradosBD()
        {
            var valorChaveEstadoSelecionado = DBContract.GetKeyValue(AppConstants.Filtro_EstadoSelecionado);
            var valorChaveCidadeSelecionada = DBContract.GetKeyValue(AppConstants.Filtro_CidadeSelecionada);
            var valorChaveLocalizacaoSelecionada = DBContract.GetKeyValue(AppConstants.Filtro_LocalizacaoSelecionada);
            var valorChaveBuscaCamping = DBContract.GetKeyValue(AppConstants.Filtro_NomeCamping);
            var usarLocalizacaoDoUsuario = valorChaveLocalizacaoSelecionada != null && Convert.ToBoolean(valorChaveLocalizacaoSelecionada);
            var valorFiltroEstabelecimentos = DBContract.GetKeyValue(AppConstants.Filtro_EstabelecimentoSelecionados);
            var valorFiltroComodidades = DBContract.GetKeyValue(AppConstants.Filtro_ServicoSelecionados) ?? string.Empty;
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
                var resultadoBuscaDeCampings = DBContract.ListCampings(valorChaveBuscaCamping, valorChaveCidadeSelecionada, valorChaveEstadoSelecionado);

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
            var campings = DBContract.Query<Item>(query).ToList();

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
            var imagem = AppConstants.Url_DominioTemporario + "carregarImagem.aspx?src=" + urlImagem.Replace(AppConstants.Url_DominioOficial, string.Empty);

            return imagem;
        }

        private static List<int> BuscarIdsCampingsPorCategoriaEComodidades(string categorias, string comodidades, bool possuiFiltroCategoria, bool possuiFiltroComodidades)
        {
            var idsCampingsComComodidades = DBContract.ListIdCampingsComComodidades(possuiFiltroComodidades, comodidades);

            // Se não possui filtro por categorias, ignora essa busca
            // Se nenhum camping atende ao filtro de comodidades também ignora essa busca
            if (possuiFiltroCategoria && (!possuiFiltroComodidades || (possuiFiltroComodidades && idsCampingsComComodidades.Count > 0)))
            {
                return DBContract.ListIdCampingsComCategorias(categorias, possuiFiltroComodidades, idsCampingsComComodidades);
            }

            return idsCampingsComComodidades;
        }
    }
}