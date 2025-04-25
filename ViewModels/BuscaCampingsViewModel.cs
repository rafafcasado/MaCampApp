using MaCamp.Models;
using MaCamp.Services;
using MaCamp.Utils;

namespace MaCamp.ViewModels
{
    public class BuscaCampingsViewModel
    {
        public string ParametroTODAS { get; }
        public string ParametroTODOS { get; }

        public BuscaCampingsViewModel()
        {
            ParametroTODAS = " - TODAS - ";
            ParametroTODOS = " - TODOS - ";
        }

        public async Task InicializarFiltrosAsync()
        {
            await DBContract.UpdateKeyValueAsync(AppConstants.Filtro_EstabelecimentoSelecionados, string.Empty);
            await DBContract.UpdateKeyValueAsync(AppConstants.Filtro_ServicoSelecionados, string.Empty);
        }

        public List<string> ObterListaEstados(List<Cidade> listaCidades)
        {
            var listEstados = new List<string>
            {
                ParametroTODOS
            };
            var listaCidadesFiltradas = listaCidades.Where(x => !string.IsNullOrWhiteSpace(x.Estado)).Select(x => x.Estado).Distinct().OfType<string>().ToList();

            listEstados.AddRange(listaCidadesFiltradas);

            return listEstados;
        }

        public async Task<List<Cidade>> ObterListaCidadesAsync()
        {
            var listaCidadesBD = await DBContract.ListAsync<Cidade>();

            if (listaCidadesBD.Count == 0)
            {
                var listaCidadesWS = await AppNet.GetListAsync<Cidade>(AppConstants.Url_ListaCidades, x => x.Estado != null && !x.Estado.Contains("_"));

                await DBContract.UpdateAsync(false, listaCidadesWS);

                return listaCidadesWS;
            }

            return listaCidadesBD;
        }

        public List<Cidade> FiltrarCidadesPorEstado(string estado, List<Cidade> listaCidades)
        {
            var cidades = listaCidades.Where(x => x.Estado == estado).ToList();

            cidades.Insert(0, new Cidade
            {
                Nome = ParametroTODAS,
                Estado = estado
            });

            return cidades;
        }

        public async Task SalvarFiltrosAsync(string? estado, string? cidade, string? nomeCamping)
        {
            var estadoSelecionado = estado == ParametroTODOS ? null : estado;
            var cidadeSelecionada = cidade == ParametroTODOS ? null : cidade;
            var nomeCampingSelecionado = string.IsNullOrWhiteSpace(nomeCamping) ? null : nomeCamping.RemoveDiacritics();

            await DBContract.UpdateKeyValueAsync(AppConstants.Filtro_EstadoSelecionado, estadoSelecionado);
            await DBContract.UpdateKeyValueAsync(AppConstants.Filtro_CidadeSelecionada, cidadeSelecionada);
            await DBContract.UpdateKeyValueAsync(AppConstants.Filtro_NomeCamping, nomeCampingSelecionado);
            await DBContract.UpdateKeyValueAsync(AppConstants.Filtro_LocalizacaoSelecionada, "false");
        }

        public async Task UsarLocalizacaoAtualAsync()
        {
            try
            {
                var permissionGranted = await Workaround.CheckPermissionAsync<Permissions.LocationWhenInUse>("Localização", "A permissão de localização será necessária para buscar o");

                if (permissionGranted)
                {
                    App.LOCALIZACAO_USUARIO = await Geolocation.GetLocationAsync();
                }

                await DBContract.UpdateKeyValueAsync(AppConstants.Filtro_LocalizacaoSelecionada, "true");
                await DBContract.UpdateKeyValueAsync(AppConstants.Filtro_EstadoSelecionado, null);
                await DBContract.UpdateKeyValueAsync(AppConstants.Filtro_CidadeSelecionada, null);
                await DBContract.UpdateKeyValueAsync(AppConstants.Filtro_NomeCamping, string.Empty);
            }
            catch (Exception ex)
            {
                Workaround.ShowExceptionOnlyDevolpmentMode(nameof(BuscaCampingsViewModel), nameof(UsarLocalizacaoAtualAsync), ex);
            }
        }

        public async Task<(string? estado, string? cidade, string? nome)> ObterFiltrosSalvosAsync()
        {
            var estado = await DBContract.GetKeyValueAsync(AppConstants.Filtro_EstadoSelecionado);
            var cidade = await DBContract.GetKeyValueAsync(AppConstants.Filtro_CidadeSelecionada);
            var nomeCamping = await DBContract.GetKeyValueAsync(AppConstants.Filtro_NomeCamping);

            return (estado, cidade, nomeCamping);
        }
    }
}
