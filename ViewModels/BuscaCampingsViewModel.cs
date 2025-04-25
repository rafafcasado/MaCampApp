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

        public void InicializarFiltros()
        {
            DBContract.UpdateKeyValue(AppConstants.Filtro_EstabelecimentoSelecionados, string.Empty);
            DBContract.UpdateKeyValue(AppConstants.Filtro_ServicoSelecionados, string.Empty);
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
            var listaCidadesBD = DBContract.List<Cidade>();

            if (listaCidadesBD.Count == 0)
            {
                var listaCidadesWS = await AppNet.GetListAsync<Cidade>(AppConstants.Url_ListaCidades, x => x.Estado != null && !x.Estado.Contains("_"));

                DBContract.Update(false, listaCidadesWS);

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

        public void SalvarFiltros(string? estado, string? cidade, string? nomeCamping)
        {
            var estadoSelecionado = estado == ParametroTODOS ? null : estado;
            var cidadeSelecionada = cidade == ParametroTODOS ? null : cidade;
            var nomeCampingSelecionado = string.IsNullOrWhiteSpace(nomeCamping) ? null : nomeCamping.RemoveDiacritics();

            DBContract.UpdateKeyValue(AppConstants.Filtro_EstadoSelecionado, estadoSelecionado);
            DBContract.UpdateKeyValue(AppConstants.Filtro_CidadeSelecionada, cidadeSelecionada);
            DBContract.UpdateKeyValue(AppConstants.Filtro_NomeCamping, nomeCampingSelecionado);
            DBContract.UpdateKeyValue(AppConstants.Filtro_LocalizacaoSelecionada, "false");
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

                DBContract.UpdateKeyValue(AppConstants.Filtro_LocalizacaoSelecionada, "true");
                DBContract.UpdateKeyValue(AppConstants.Filtro_EstadoSelecionado, null);
                DBContract.UpdateKeyValue(AppConstants.Filtro_CidadeSelecionada, null);
                DBContract.UpdateKeyValue(AppConstants.Filtro_NomeCamping, string.Empty);
            }
            catch (Exception ex)
            {
                Workaround.ShowExceptionOnlyDevolpmentMode(nameof(BuscaCampingsViewModel), nameof(UsarLocalizacaoAtualAsync), ex);
            }
        }

        public (string? estado, string? cidade, string? nome) ObterFiltrosSalvos()
        {
            var estado = DBContract.GetKeyValue(AppConstants.Filtro_EstadoSelecionado);
            var cidade = DBContract.GetKeyValue(AppConstants.Filtro_CidadeSelecionada);
            var nomeCamping = DBContract.GetKeyValue(AppConstants.Filtro_NomeCamping);

            return (estado, cidade, nomeCamping);
        }
    }
}
