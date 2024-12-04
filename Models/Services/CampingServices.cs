using System.Text;
using MaCamp.AppSettings;
using MaCamp.Models.DataAccess;

namespace MaCamp.Models.Services
{
    public static class CampingServices
    {
        public static bool ExistemCampingsBD()
        {
            var db = DBContract.NewInstance();
            var valorChaveDownloadConcluido = db.ObterValorChave(AppConstants.ChaveDownloadCampingsCompleto);
            var downloadConcluido = !string.IsNullOrWhiteSpace(valorChaveDownloadConcluido) && Convert.ToBoolean(valorChaveDownloadConcluido);
            var tem = downloadConcluido && db.ObterItem(i => i.IdPost == 0) != null;

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
                    var DB = DBContract.NewInstance();
                    DB.InserirOuSubstituirModelo(new ChaveValor(AppConstants.ChaveDownloadCampingsCompleto, "false", TipoChave.ControleInterno));
                    var campings = new List<Item>();

                    var chamadasWS = new List<Task>
                    {
                        Task.Run(async () =>
                        {
                            campings = await new WebService<Item>().Get(AppConstants.UrlListaCampings, 0, string.Empty, string.Empty);
                        }),
                        Task.Run(async () =>
                        {
                            identificadores = await new WebService<ItemIdentificador>().Get(AppConstants.UrlListaIdentificadores, 1);
                        })
                    };

                    Task.WaitAll(chamadasWS.ToArray());

                    //Armazenar ID's de todos os Campings Favoritados
                    var idsFavoritados = DB.BuscarIdsCampingsFavoritados();

                    DB.ApagarItens();
                    DB.InserirListaDeModelo(campings);
                    DB.ApagarItensIdentificadores();
                    DB.InserirListaDeModelo(identificadores);
                    DB.InserirOuSubstituirModelo(new ChaveValor(AppConstants.ChaveDownloadCampingsCompleto, "true", TipoChave.ControleInterno));

                    DB.InserirOuSubstituirModelo(new ChaveValor
                    {
                        Chave = AppConstants.ChaveDataUltimaAtualizacaoConteudo,
                        Valor = DateTime.Now.ToString("yyyy/MM/dd")
                    });

                    //Realizar update informando os ID's dos favoritos
                    DB.AtualizarIdsCampingsFavoritados(idsFavoritados);
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

            var DB = DBContract.NewInstance();
            var campings = DB.ListarItens(i => i.IdPost == 0).OrderBy(i => i.Nome).ToList();

            return campings;
        }

        private static async Task<List<Item>> CarregarCampingsFiltradosBD()
        {
            var DB = DBContract.NewInstance();
            var valorChaveEstadoSelecionado = DB.ObterValorChave("FILTROS_ESTADO_SELECIONADO");
            var valorChaveCidadeSelecionada = DB.ObterValorChave("FILTROS_CIDADE_SELECIONADA");
            var valorChaveLocalizacaoSelecionada = DB.ObterValorChave("FILTROS_LOCALIZACAO_SELECIONADA");
            var valorChaveBuscaCamping = DB.ObterValorChave("FILTROS_NOME_DO_CAMPING");
            var usarLocalizacaoDoUsuario = valorChaveLocalizacaoSelecionada != null && Convert.ToBoolean(valorChaveLocalizacaoSelecionada);
            var valorFiltroEstabelecimentos = DB.ObterValorChave("FILTROS_ESTABELECIMENTO_SELECIONADOS");
            var valorFiltroComodidades = DB.ObterValorChave("FILTROS_SERVICO_SELECIONADOS") ?? string.Empty;
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
                var resultadoBuscaDeCampings = DB.BuscarCampings(valorChaveBuscaCamping, valorChaveCidadeSelecionada, valorChaveEstadoSelecionado);

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
            var campings = DB.QueryItens(query).ToList();

            if (usarLocalizacaoDoUsuario)
            {
                App.LOCALIZACAO_USUARIO = await Geolocation.GetLastKnownLocationAsync();

                if (App.LOCALIZACAO_USUARIO == null)
                {
                    App.LOCALIZACAO_USUARIO = await Geolocation.GetLocationAsync();
                }

                campings = campings.Where(i => i.Latitude != 0 && i.Longitude != 0).OrderBy(i => i.DistanciaDoUsuario).Take(20).ToList();
            }

            return campings;
        }

        public static string MontarUrlImagemTemporaria(string urlImagem)
        {
            var imagem = AppConstants.UrlDominioTemporario + "carregarImagem.aspx?src=" + urlImagem.Replace(AppConstants.UrlDominioOficial, "");

            return imagem;
        }

        private static List<int> BuscarIdsCampingsPorCategoriaEComodidades(string categorias, string comodidades, bool possuiFiltroCategoria, bool possuiFiltroComodidades)
        {
            var db = DBContract.NewInstance();
            var idsCampingsComComodidades = db.ListarIdsCampingsComComodidades(possuiFiltroComodidades, comodidades);

            // Se não possui filtro por categorias, ignora essa busca
            // Se nenhum camping atende ao filtro de comodidades também ignora essa busca
            if (possuiFiltroCategoria && (!possuiFiltroComodidades || (possuiFiltroComodidades && idsCampingsComComodidades.Count > 0)))
            {
                return db.ListarIdsCampingsComCategorias(categorias, possuiFiltroComodidades, idsCampingsComComodidades);
            }

            return idsCampingsComComodidades;
        }
    }
}