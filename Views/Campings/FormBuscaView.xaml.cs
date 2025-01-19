using System.Diagnostics;
using MaCamp.AppSettings;
using MaCamp.Models;
using MaCamp.Models.DataAccess;
using MaCamp.Utils;

namespace MaCamp.Views.Campings
{
    public partial class FormBuscaView : ContentView
    {
        private string ParametroTODAS { get; }
        private string ParametroTODOS { get; }
        private string CampingsTodos { get; set; }
        private DBContract DB { get; }
        private string? EstadoSelecionado { get; set; }
        private string? CidadeSelecionada { get; set; }
        private string? NomeDoCamping { get; set; }

        public FormBuscaView()
        {
            InitializeComponent();

            ParametroTODAS = " - TODAS - ";
            ParametroTODOS = " - TODOS - ";
            CampingsTodos = " ";
            DB = DBContract.Instance;

            Task.Run(() =>
            {
                DB.InserirOuSubstituirModelo(new ChaveValor
                {
                    Chave = AppConstants.Filtro_EstabelecimentoSelecionados,
                    Valor = ""
                });

                DB.InserirOuSubstituirModelo(new ChaveValor
                {
                    Chave = AppConstants.Filtro_ServicoSelecionados,
                    Valor = ""
                });

                CarregarCidadesEstados();
                CarregarLocalizacaoUsuario();
            });

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
            //var valorChaveUsarLocalizacaoUsuario = DB.ObterValorChave(AppConstants.Filtro_LocalizacaoSelecionada);
        }

        private async void CarregarCidadesEstados()
        {
            var EstadoBD = DB.ObterValorChave(AppConstants.Filtro_EstadoSelecionado);
            var CIDADE_BD = DB.ObterValorChave(AppConstants.Filtro_CidadeSelecionada);
            var NomeCampingBD = DB.ObterValorChave(AppConstants.Filtro_NomeCamping);
            var cidadesWS = DB.ListarCidades();

            if (cidadesWS.Count == 0)
            {
                cidadesWS = await AppNet.GetListAsync<Cidade>(AppConstants.Url_ListaCidades, x => x.Estado != null && !x.Estado.Contains("_"));

                DB.InserirListaDeModelo(cidadesWS);
            }

            var gruposCidadePorUF = cidadesWS.GroupBy(c => c.Estado).ToList();
            var estados = new List<string>
            {
                ParametroTODOS
            };

            estados.AddRange(gruposCidadePorUF.Where(x => x.Key != null).Select(x => x.Key).OfType<string>().ToList());

            Dispatcher.Dispatch(() =>
            {
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
                        var cidadesDisponiveis = cidadesWS.Where(c => c.Estado == EstadoSelecionado).ToList();

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
                            pkCidade.SelectedItem = cidadesDisponiveis.FirstOrDefault(c => c.Nome == CIDADE_BD);
                        }
                    }
                };

                pkCidade.SelectedIndexChanged += (sender, args) =>
                {
                    CidadeSelecionada = sender is Picker picker && picker.SelectedItem is Cidade selectedItem ? selectedItem.Nome : string.Empty;
                };

                pkUF.SelectedItem = EstadoBD;
                etNomeDoCamping.Text = NomeCampingBD ?? string.Empty;
            });
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
                Debug.WriteLine(ex.Message);
            }

            loader.IsVisible = false;
            loader.IsRunning = false;

            //AlterarBuscaLocalizacao(false);

            DB.InserirOuSubstituirModelo(new ChaveValor
            {
                Chave = AppConstants.Filtro_LocalizacaoSelecionada,
                Valor = "true"
            });
            DB.InserirOuSubstituirModelo(new ChaveValor
            {
                Chave = AppConstants.Filtro_EstadoSelecionado,
                Valor = null
            });
            DB.InserirOuSubstituirModelo(new ChaveValor
            {
                Chave = AppConstants.Filtro_CidadeSelecionada,
                Valor = null
            });
            DB.InserirOuSubstituirModelo(new ChaveValor
            {
                Chave = AppConstants.Filtro_NomeCamping,
                Valor = ""
            });

            //Plugin.GoogleAnalytics.GoogleAnalytics.Current.Tracker.SendEvent("Usar minha localização", "Buscando campings próximos");

            MessagingCenter.Send(Application.Current, AppConstants.MessagingCenter_BuscaRealizada);
        }

        private void btBuscar_Clicked(object sender, EventArgs e)
        {
            DB.InserirOuSubstituirModelo(new ChaveValor
            {
                Chave = AppConstants.Filtro_EstadoSelecionado,
                Valor = EstadoSelecionado == ParametroTODOS || EstadoSelecionado == string.Empty ? null : EstadoSelecionado
            });

            DB.InserirOuSubstituirModelo(new ChaveValor
            {
                Chave = AppConstants.Filtro_CidadeSelecionada,
                Valor = CidadeSelecionada == ParametroTODAS || CidadeSelecionada == string.Empty ? null : CidadeSelecionada
            });

            DB.InserirOuSubstituirModelo(new ChaveValor
            {
                Chave = AppConstants.Filtro_LocalizacaoSelecionada,
                Valor = "false"
            });

            NomeDoCamping = etNomeDoCamping.Text.RemoveDiacritics();

            DB.InserirOuSubstituirModelo(new ChaveValor
            {
                Chave = AppConstants.Filtro_NomeCamping,
                Valor = NomeDoCamping == string.Empty ? null : NomeDoCamping
            });

            pkUF.SelectedItem = null;

            MessagingCenter.Send(Application.Current, AppConstants.MessagingCenter_BuscaRealizada);
        }
    }
}