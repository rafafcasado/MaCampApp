using System.Text;
using MaCamp.Models;
using MaCamp.Utils;
using static MaCamp.Utils.Enumeradores;

namespace MaCamp.Services
{
    public static class CampingServices
    {
        public static async Task<bool> ExistemCampingsAsync()
        {
            var valorChaveDownloadConcluido = await DBContract.GetKeyValueAsync(AppConstants.Chave_DownloadCampingsCompleto);
            var downloadConcluido = !string.IsNullOrWhiteSpace(valorChaveDownloadConcluido) && Convert.ToBoolean(valorChaveDownloadConcluido);
            var item = await DBContract.GetAsync<Item>(x => x.IdPost == 0);
            var tem = downloadConcluido && item != null;

            return tem;
        }

        public static async Task BaixarCampingsAsync(bool forcarAtualizacao, ProgressoVisual? progressoVisual = null)
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
                var existemCampingsSalvos = await ExistemCampingsAsync();

                if (!existemCampingsSalvos || forcarAtualizacao)
                {
                    App.BAIXANDO_CAMPINGS = true;

                    ProgressoVisual.AumentarTotal(progressoVisual, 3);

                    await DBContract.UpdateKeyValueAsync(AppConstants.Chave_DownloadCampingsCompleto, "false", TipoChave.ControleInterno);

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

                    Parallel.ForEach(campings, x =>
                    {
                        x.Identificadores = identificadores.Where(y => y.IdItem == x.IdCamping).ToList();
                    });

                    await DBContract.UpdateAsync(true, campings, identificadores, progressoVisual);

                    ProgressoVisual.AumentarAtual(progressoVisual);

                    App.BAIXANDO_CAMPINGS = false;
                }
            }
        }

        public static async Task<List<Item>> CarregarCampingsAsync(bool utilizarFiltros = false)
        {
            await BaixarCampingsAsync(false);

            App.EXISTEM_CAMPINGS_DISPONIVEIS = await ExistemCampingsAsync();

            if (utilizarFiltros)
            {
                var campingsFiltrados = await CarregarCampingsFiltradosAsync();

                return campingsFiltrados;
            }

            var listaCampings = await DBContract.ListAsync<Item>(x => x.IdPost == 0);
            var campings = listaCampings.OrderBy(x => x.Nome).ToList();

            return campings;
        }

        private static async Task<List<Item>> CarregarCampingsFiltradosAsync()
        {
            var valorChaveEstadoSelecionado = await DBContract.GetKeyValueAsync(AppConstants.Filtro_EstadoSelecionado);
            var valorChaveCidadeSelecionada = await DBContract.GetKeyValueAsync(AppConstants.Filtro_CidadeSelecionada);
            var valorChaveLocalizacaoSelecionada = await DBContract.GetKeyValueAsync(AppConstants.Filtro_LocalizacaoSelecionada);
            var valorChaveBuscaCamping = await DBContract.GetKeyValueAsync(AppConstants.Filtro_NomeCamping);
            var usarLocalizacaoDoUsuario = valorChaveLocalizacaoSelecionada != null && Convert.ToBoolean(valorChaveLocalizacaoSelecionada);
            var valorFiltroEstabelecimentos = await DBContract.GetKeyValueAsync(AppConstants.Filtro_EstabelecimentoSelecionados);
            var valorFiltroComodidades = await DBContract.GetKeyValueAsync(AppConstants.Filtro_ServicoSelecionados) ?? string.Empty;
            var identificadoresEstabelecimento = "'" + valorFiltroEstabelecimentos?.Replace(",", "','") + "'";
            var identificadoresComodidades = "'" + valorFiltroComodidades.Replace(",", "','") + "'";
            var possuiFiltroCategoria = identificadoresEstabelecimento != "''";
            var possuiFiltroComodidades = identificadoresComodidades != "''";
            var idsCampingsQueAtendemCategoriaEComodidade = await BuscarIdsCampingsPorCategoriaEComodidadesAsync(identificadoresEstabelecimento, identificadoresComodidades, possuiFiltroCategoria, possuiFiltroComodidades);

            //Se possui filtro, e nenhum id de camping foi retornado nenhum camping atende
            if ((possuiFiltroCategoria || possuiFiltroComodidades) && idsCampingsQueAtendemCategoriaEComodidade.Count == 0)
            {
                return new List<Item>();
            }

            if (!string.IsNullOrWhiteSpace(valorChaveBuscaCamping))
            {
                var resultadoBuscaDeCampings = await DBContract.ListCampingsAsync(valorChaveBuscaCamping, valorChaveCidadeSelecionada, valorChaveEstadoSelecionado);

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
            var campings = await DBContract.QueryAsync<Item>(query);

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

        private static async Task<List<int>> BuscarIdsCampingsPorCategoriaEComodidadesAsync(string categorias, string comodidades, bool possuiFiltroCategoria, bool possuiFiltroComodidades)
        {
            var idsCampingsComComodidades = await DBContract.ListIdCampingsComComodidadesAsync(possuiFiltroComodidades, comodidades);

            // Se não possui filtro por categorias, ignora essa busca
            // Se nenhum camping atende ao filtro de comodidades também ignora essa busca
            if (possuiFiltroCategoria && (!possuiFiltroComodidades || (possuiFiltroComodidades && idsCampingsComComodidades.Count > 0)))
            {
                return await DBContract.ListIdCampingsComCategoriasAsync(categorias, possuiFiltroComodidades, idsCampingsComComodidades);
            }

            return idsCampingsComComodidades;
        }
    }
}