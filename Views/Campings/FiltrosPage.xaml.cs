using CommunityToolkit.Mvvm.Messaging;
using MaCamp.CustomControls;
using MaCamp.Utils;
using MaCamp.ViewModels;

namespace MaCamp.Views.Campings
{
    public partial class FiltrosPage : SmartContentPage
    {
        private List<string> ListaEstabelecimentosSelecionados { get; set; }
        private List<string> ListaComodidadesSelecionadas { get; set; }

        public FiltrosPage(bool busca = false)
        {
            InitializeComponent();

            ListaComodidadesSelecionadas = new List<string>();
            ListaEstabelecimentosSelecionados = new List<string>();
            BindingContext = new FiltrosViewModel();

            if (busca)
            {
                Title = "Busca";
                btFiltrar.Text = "BUSCAR";
            }
            else
            {
                Title = "Filtros";
            }

            FirstAppeared += FiltrosPage_FirstAppeared;
        }

        private void FiltrosPage_FirstAppeared(object? sender, EventArgs e)
        {
            if (BindingContext is FiltrosViewModel viewModel)
            {
                var listaEstabelecimentos = viewModel.ObterFiltrosEstabelecimento();
                var listaComodidades = viewModel.ObterFiltrosServicos();

                ListaEstabelecimentosSelecionados = listaEstabelecimentos;
                ListaComodidadesSelecionadas = listaComodidades;

                CarregarEstabelecimentos(listaEstabelecimentos);
                CarregarServicos(listaComodidades);
            }
        }

        private void CarregarEstabelecimentos(List<string> listaFiltros)
        {
            foreach (var filtro in listaFiltros)
            {
                switch (filtro)
                {
                    case "Campings":
                        DeselecionarView(slFiltroCamping);
                        break;
                    case "PontodeApoioaRV`s":
                        DeselecionarView(slFiltroApoioRVs);
                        break;
                    case "CampingSelvagem/WildCamping/Bushcfaft":
                        DeselecionarView(slFiltroWild);
                        break;
                    case "SemFunçãoCamping/ApoioouFechado":
                        DeselecionarView(slFiltroSemFuncao);
                        break;
                }
            }
        }

        private void CarregarServicos(List<string> listaFiltros)
        {
            foreach (var filtro in listaFiltros)
            {
                switch (filtro)
                {
                    case "AceitaBarracas":
                        DeselecionarView(slAceitaBarracas);
                        break;
                    case "AceitaRVs":
                        DeselecionarView(slAceitaRVs);
                        break;
                    case "PossuiChalesCabanasOuSuites":
                        DeselecionarView(slPossuiChales); break;
                    case "AceitaAnimais":
                        DeselecionarView(slAceitaAnimais);
                        break;
                    case "Restaurante":
                        DeselecionarView(slRestaurante);
                        break;
                    case "ChurrasqueiraCozinha":
                        DeselecionarView(slChurrasqueiraCozinha);
                        break;
                    case "TelefoneInternet":
                        DeselecionarView(slTelefoneInternet);
                        break;
                    case "Piscina":
                        DeselecionarView(slPiscina);
                        break;
                    case "Acessibilidade":
                        DeselecionarView(slAcessibilidade);
                        break;
                    case "Transporte":
                        DeselecionarView(slAcessoTransporteColetivo);
                        break;
                    case "Esgoto":
                        DeselecionarView(slLocalDetritos);
                        break;
                    case "PossuiEnergiaEletrica":
                        DeselecionarView(slEnergiaEletrica);
                        break;
                    case "PossuiPraiaProxima":
                        DeselecionarView(slPossuiPraiaProxima);
                        break;
                }
            }
        }

        private void FiltroEstabelecimentoTapped(object sender, EventArgs e)
        {
            if (sender is View view && BindingContext is FiltrosViewModel viewModel)
            {
                var filtro = view.ClassId;

                viewModel.AlternarEstabelecimento(ListaEstabelecimentosSelecionados, filtro);

                if (ListaEstabelecimentosSelecionados.Contains(filtro))
                {
                    SelecionarView(view);
                }
                else
                {
                    DeselecionarView(view);
                }
            }
        }

        private void ServicoTapped(object sender, EventArgs e)
        {
            if (sender is View view && BindingContext is FiltrosViewModel viewModel)
            {
                var servico = view.ClassId;

                viewModel.AlternarServico(ListaComodidadesSelecionadas, servico);

                if (ListaComodidadesSelecionadas.Contains(servico))
                {
                    SelecionarView(view);
                }
                else
                {
                    DeselecionarView(view);
                }
            }
        }

        private void SelecionarView(View view)
        {
            view.BackgroundColor = AppColors.CorDestaque;

            if (view is StackLayout layout)
            {
                var listChildrens = layout.Children.OfType<Label>().ToList();

                foreach (var child in listChildrens)
                {
                    child.TextColor = Colors.White;
                }
            }
        }

        private void DeselecionarView(View view)
        {
            view.BackgroundColor = Color.FromArgb("F5F5F5");

            if (view is StackLayout layout)
            {
                var listChildrens = layout.Children.OfType<Label>().ToList();

                foreach (var child in listChildrens)
                {
                    child.TextColor = Colors.Gray;
                }
            }
        }

        private async void Filtrar(object sender, EventArgs e)
        {
            if (BindingContext is FiltrosViewModel viewModel)
            {
                viewModel.SalvarFiltros(ListaEstabelecimentosSelecionados, ListaComodidadesSelecionadas);

                //Plugin.GoogleAnalytics.GoogleAnalytics.Current.Tracker.SendEvent("Filtro Estabelecimentos e Serviços", "Filtrar", valorEstabelecimentos + " - " + valorComodidades);
                WeakReferenceMessenger.Default.Send(string.Empty, AppConstants.WeakReferenceMessenger_BuscaRealizada);

                await Navigation.PopAsync();
            }
        }
    }
}