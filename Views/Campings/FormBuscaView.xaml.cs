using CommunityToolkit.Mvvm.Messaging;
using MaCamp.CustomControls;
using MaCamp.Models;
using MaCamp.Utils;
using MaCamp.ViewModels;

namespace MaCamp.Views.Campings
{
    public partial class FormBuscaView : SmartContentView
    {
        private string? EstadoSelecionado { get; set; }
        private string? CidadeSelecionada { get; set; }
        private List<Cidade> ListaCidades { get; set; }
        private string? CidadeSalva { get; set; }

        public FormBuscaView()
        {
            InitializeComponent();

            ListaCidades = new List<Cidade>();
            BindingContext = new BuscaCampingsViewModel();

            FirstAppeared += FormBuscaView_FirstAppeared;

            //Plugin.GoogleAnalytics.GoogleAnalytics.Current.Tracker.SendView("Busca de Campings (Estado/Cidade)");
        }

        private async void FormBuscaView_FirstAppeared(object? sender, EventArgs e)
        {
            if (BindingContext is BuscaCampingsViewModel viewModel)
            {
                await viewModel.InicializarFiltrosAsync();

                var cidades = await viewModel.ObterListaCidadesAsync();
                var estados = viewModel.ObterListaEstados(cidades);
                var (estadoSalvo, cidadeSalva, nomeSalvo) = await viewModel.ObterFiltrosSalvosAsync();

                ListaCidades = cidades.OrderBy(x => x.Nome).ToList();
                CidadeSalva = cidadeSalva;

                pkUF.Title = "Selecione o Estado";
                pkUF.ItemsSource = estados.OrderBy(x => x).ToList();
                pkUF.SelectedItem = estadoSalvo;

                etNomeDoCamping.Text = nomeSalvo ?? string.Empty;
                btBuscar.IsEnabled = true;
            }
        }

        private void PkUF_OnSelectedIndexChanged(object? sender, EventArgs e)
        {
            if (BindingContext is BuscaCampingsViewModel viewModel && sender is Picker picker)
            {
                if (picker.SelectedItem is string estado)
                {
                    EstadoSelecionado = estado;

                    if (estado == viewModel.ParametroTODOS)
                    {
                        pkCidade.ItemsSource = null;
                        pkCidade.Title = " - ";
                        pkCidade.IsEnabled = false;
                        CidadeSelecionada = string.Empty;
                    }
                    else
                    {
                        var cidadesFiltradas = viewModel.FiltrarCidadesPorEstado(estado, ListaCidades);

                        pkCidade.ItemsSource = cidadesFiltradas;
                        pkCidade.ItemDisplayBinding = new Binding(nameof(Cidade.Nome));
                        pkCidade.Title = "Selecione a cidade";
                        pkCidade.IsEnabled = true;

                        if (!string.IsNullOrEmpty(CidadeSalva))
                        {
                            pkCidade.SelectedItem = cidadesFiltradas.FirstOrDefault(x => x.Nome == CidadeSalva);
                        }
                    }
                }
                else
                {
                    EstadoSelecionado = null;
                    pkCidade.ItemsSource = null;
                    pkCidade.Title = " - ";
                    pkCidade.IsEnabled = false;
                    CidadeSelecionada = string.Empty;
                }
            }
        }

        private void PkCidade_OnSelectedIndexChanged(object? sender, EventArgs e)
        {
            if (sender is Picker picker)
            {
                CidadeSelecionada = picker.SelectedItem is Cidade cidade ? cidade.Nome : string.Empty;
            }
        }

        private async void btBuscar_Clicked(object sender, EventArgs e)
        {
            if (BindingContext is BuscaCampingsViewModel viewModel)
            {
                await viewModel.SalvarFiltrosAsync(EstadoSelecionado, CidadeSelecionada, etNomeDoCamping.Text);

                pkUF.SelectedItem = null;
                WeakReferenceMessenger.Default.Send(string.Empty, AppConstants.WeakReferenceMessenger_BuscaRealizada);
            }
        }

        private async void TapGestureRecognizer_OnTapped(object? sender, TappedEventArgs e)
        {
            if (BindingContext is BuscaCampingsViewModel viewModel)
            {
                loader.IsVisible = true;
                loader.IsRunning = true;
                slUsarMinhaLocalizacao.IsVisible = false;

                await viewModel.UsarLocalizacaoAtualAsync();

                loader.IsVisible = false;
                loader.IsRunning = false;

                WeakReferenceMessenger.Default.Send(string.Empty, AppConstants.WeakReferenceMessenger_BuscaRealizada);
            }
        }
    }
}