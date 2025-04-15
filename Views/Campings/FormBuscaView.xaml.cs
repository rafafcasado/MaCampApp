using CommunityToolkit.Mvvm.Messaging;
using MaCamp.Models;
using MaCamp.Services.DataAccess;
using MaCamp.Utils;

namespace MaCamp.Views.Campings
{
    public partial class FormBuscaView : ContentView
    {
        private string ParametroTODAS { get; }
        private string ParametroTODOS { get; }
        private string CampingsTodos { get; set; }
        private string? EstadoSelecionado { get; set; }
        private string? CidadeSelecionada { get; set; }
        private string? NomeDoCamping { get; set; }

        public FormBuscaView()
        {
            InitializeComponent();

            ParametroTODAS = " - TODAS - ";
            ParametroTODOS = " - TODOS - ";
            CampingsTodos = " ";

            DBContract.UpdateKeyValue(AppConstants.Filtro_EstabelecimentoSelecionados, string.Empty);
            DBContract.UpdateKeyValue(AppConstants.Filtro_ServicoSelecionados, string.Empty);

            CarregarCidadesEstados();
            CarregarLocalizacaoUsuario();

            //TapGestureRecognizer buscarCidadeEstado = new TapGestureRecognizer();
            //buscarCidadeEstado.Tapped += (s, e) =>
            //{
            //    AlterarBuscaLocalizacao(true);
            //};
            //slBuscarCidadeEstado.GestureRecognizers.Add(buscarCidadeEstado);3s

            //Plugin.GoogleAnalytics.GoogleAnalytics.Current.Tracker.SendView("Busca de Campings (Estado/Cidade)");
        }

        private void CarregarLocalizacaoUsuario()
        {
            //var valorChaveUsarLocalizacaoUsuario = DBContract.ObterValorChave(AppConstants.Filtro_LocalizacaoSelecionada);
        }

        private async void CarregarCidadesEstados()
        {
            var EstadoBD = DBContract.GetKeyValue(AppConstants.Filtro_EstadoSelecionado);
            var CIDADE_BD = DBContract.GetKeyValue(AppConstants.Filtro_CidadeSelecionada);
            var NomeCampingBD = DBContract.GetKeyValue(AppConstants.Filtro_NomeCamping);
            var cidadesWS = DBContract.List<Cidade>();

            if (cidadesWS.Count == 0)
            {
                await Workaround.TaskWork(async () =>
                {
                    cidadesWS = await AppNet.GetListAsync<Cidade>(AppConstants.Url_ListaCidades, x => x.Estado != null && !x.Estado.Contains("_"));
                });

                DBContract.Update(false, cidadesWS);
            }

            var gruposCidadePorUF = cidadesWS.GroupBy(x => x.Estado).ToList();
            var estados = new List<string>
            {
                ParametroTODOS
            };

            estados.AddRange(gruposCidadePorUF.Where(x => x.Key != null).Select(x => x.Key).OfType<string>().ToList());

            btBuscar.IsEnabled = true;
            pkUF.Title = "Selecione o Estado";
            pkUF.ItemsSource = estados;

            pkUF.SelectedIndexChanged += (sender, args) =>
            {
                if (sender is Picker picker && picker.SelectedItem is string selectedItem)
                {
                    EstadoSelecionado = selectedItem;
                }

                if (EstadoSelecionado == ParametroTODOS)
                {
                    pkCidade.ItemsSource = null;
                    pkCidade.Title = " - ";
                    pkCidade.IsEnabled = false;
                    CidadeSelecionada = string.Empty;
                }
                else
                {
                    var cidadesDisponiveis = cidadesWS.Where(x => x.Estado == EstadoSelecionado).ToList();

                    cidadesDisponiveis.Insert(0, new Cidade
                    {
                        Nome = ParametroTODAS,
                        Estado = EstadoSelecionado
                    });

                    pkCidade.ItemsSource = cidadesDisponiveis;
                    pkCidade.ItemDisplayBinding = new Binding(nameof(Cidade.Nome));
                    pkCidade.Title = "Selecione a cidade";
                    pkCidade.IsEnabled = true;

                    if (CIDADE_BD != null)
                    {
                        pkCidade.SelectedItem = cidadesDisponiveis.FirstOrDefault(x => x.Nome == CIDADE_BD);
                    }
                }
            };

            pkCidade.SelectedIndexChanged += (sender, args) =>
            {
                CidadeSelecionada = sender is Picker picker && picker.SelectedItem is Cidade selectedItem ? selectedItem.Nome : string.Empty;
            };

            pkUF.SelectedItem = EstadoBD;
            etNomeDoCamping.Text = NomeCampingBD ?? string.Empty;
        }

        private async void UsarMinhaLocalizacao(object sender, EventArgs e)
        {
            loader.IsVisible = true;
            loader.IsRunning = true;
            slUsarMinhaLocalizacao.IsVisible = false;

            try
            {
                App.LOCALIZACAO_USUARIO = await Geolocation.GetLocationAsync();
            }
            catch (Exception ex)
            {
                Workaround.ShowExceptionOnlyDevolpmentMode(nameof(FormBuscaView), nameof(UsarMinhaLocalizacao), ex);
            }

            loader.IsVisible = false;
            loader.IsRunning = false;

            //AlterarBuscaLocalizacao(false);

            DBContract.UpdateKeyValue(AppConstants.Filtro_LocalizacaoSelecionada, "true");
            DBContract.UpdateKeyValue(AppConstants.Filtro_EstadoSelecionado, null);
            DBContract.UpdateKeyValue(AppConstants.Filtro_CidadeSelecionada, null);
            DBContract.UpdateKeyValue(AppConstants.Filtro_NomeCamping, string.Empty);

            //Plugin.GoogleAnalytics.GoogleAnalytics.Current.Tracker.SendEvent("Usar minha localização", "Buscando campings próximos");

            WeakReferenceMessenger.Default.Send(string.Empty, AppConstants.WeakReferenceMessenger_BuscaRealizada);
        }

        private void btBuscar_Clicked(object sender, EventArgs e)
        {
            DBContract.UpdateKeyValue(AppConstants.Filtro_EstadoSelecionado, EstadoSelecionado == ParametroTODOS || EstadoSelecionado == string.Empty ? null : EstadoSelecionado);
            DBContract.UpdateKeyValue(AppConstants.Filtro_CidadeSelecionada, CidadeSelecionada == ParametroTODAS || CidadeSelecionada == string.Empty ? null : CidadeSelecionada);
            DBContract.UpdateKeyValue(AppConstants.Filtro_LocalizacaoSelecionada, "false");

            NomeDoCamping = etNomeDoCamping.Text.RemoveDiacritics();

            DBContract.UpdateKeyValue(AppConstants.Filtro_NomeCamping, NomeDoCamping == string.Empty ? null : NomeDoCamping);

            pkUF.SelectedItem = null;

            WeakReferenceMessenger.Default.Send(string.Empty, AppConstants.WeakReferenceMessenger_BuscaRealizada);
        }
    }
}