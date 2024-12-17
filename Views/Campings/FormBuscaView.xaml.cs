using System.Diagnostics;
using System.Globalization;
using System.Text;
using MaCamp.AppSettings;
using MaCamp.Models;
using MaCamp.Models.DataAccess;
using Newtonsoft.Json;

namespace MaCamp.Views.Campings
{
    public partial class FormBuscaView : ContentView
    {
        private string ParametroTODAS { get; }
        private string ParametroTODOS { get; }
        //private string CampingsTodos { get; set; }
        private DBContract DB { get; }
        private string? EstadoSelecionado { get; set; }
        private string? CidadeSelecionada { get; set; }
        private string? NomeDoCamping { get; set; }

        public FormBuscaView()
        {
            InitializeComponent();

            ParametroTODAS = " - TODAS - ";
            ParametroTODOS = " - TODOS - ";
            //CampingsTodos = " ";
            DB = DBContract.Instance;

            Task.Run(() =>
            {
                DB.InserirOuSubstituirModelo(new ChaveValor
                {
                    Chave = "FILTROS_ESTABELECIMENTO_SELECIONADOS", Valor = ""
                });

                DB.InserirOuSubstituirModelo(new ChaveValor
                {
                    Chave = "FILTROS_SERVICO_SELECIONADOS", Valor = ""
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
            //var valorChaveUsarLocalizacaoUsuario = DB.ObterValorChave("FILTROS_LOCALIZACAO_SELECIONADA");
        }

        private async void CarregarCidadesEstados()
        {
            var EstadoBD = DB.ObterValorChave("FILTROS_ESTADO_SELECIONADO");
            var CIDADE_BD = DB.ObterValorChave("FILTROS_CIDADE_SELECIONADA");
            var NomeCampingBD = DB.ObterValorChave("FILTROS_NOME_DO_CAMPING");
            var cidadesWS = DB.ListarCidades();

            if (cidadesWS.Count == 0)
            {
                using var client = new HttpClient();

                try
                {
                    var jsonCidades = await client.GetStringAsync(AppConstants.Url_ListaCidades);
                    cidadesWS = JsonConvert.DeserializeObject<List<Cidade>>(jsonCidades)?.Where(x => x.Estado != null && !x.Estado.Contains("_")).ToList() ?? new List<Cidade>();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

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
            loader.IsVisible = loader.IsRunning = true;
            slUsarMinhaLocalizacao.IsVisible = false;

            try
            {
                App.LOCALIZACAO_USUARIO = await Geolocation.GetLocationAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            loader.IsVisible = loader.IsRunning = false;

            //AlterarBuscaLocalizacao(false);
            DB.InserirOuSubstituirModelo(new ChaveValor
            {
                Chave = "FILTROS_LOCALIZACAO_SELECIONADA",
                Valor = "true"
            });

            DB.InserirOuSubstituirModelo(new ChaveValor
            {
                Chave = "FILTROS_ESTADO_SELECIONADO",
                Valor = null
            });

            DB.InserirOuSubstituirModelo(new ChaveValor
            {
                Chave = "FILTROS_CIDADE_SELECIONADA",
                Valor = null
            });

            DB.InserirOuSubstituirModelo(new ChaveValor
            {
                Chave = "FILTROS_NOME_DO_CAMPING",
                Valor = ""
            });

            //Plugin.GoogleAnalytics.GoogleAnalytics.Current.Tracker.SendEvent("Usar minha localização", "Buscando campings próximos");

            //MessagingCenter.Send(Application.Current, AppConstants.MessagingCenter_BuscaRealizada);
        }

        private void btBuscar_Clicked(object sender, EventArgs e)
        {
            DB.InserirOuSubstituirModelo(new ChaveValor
            {
                Chave = "FILTROS_ESTADO_SELECIONADO",
                Valor = EstadoSelecionado == ParametroTODOS || EstadoSelecionado == string.Empty ? null : EstadoSelecionado
            });

            DB.InserirOuSubstituirModelo(new ChaveValor
            {
                Chave = "FILTROS_CIDADE_SELECIONADA",
                Valor = CidadeSelecionada == ParametroTODAS || CidadeSelecionada == string.Empty ? null : CidadeSelecionada
            });

            DB.InserirOuSubstituirModelo(new ChaveValor
            {
                Chave = "FILTROS_LOCALIZACAO_SELECIONADA",
                Valor = "false"
            });

            NomeDoCamping = RemoveDiacritics(etNomeDoCamping.Text);

            DB.InserirOuSubstituirModelo(new ChaveValor
            {
                Chave = "FILTROS_NOME_DO_CAMPING",
                Valor = NomeDoCamping == string.Empty ? null : NomeDoCamping
            });

            pkUF.SelectedItem = null;

            //MessagingCenter.Send(Application.Current, AppConstants.MessagingCenter_BuscaRealizada);
        }

        private string RemoveDiacritics(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return text;
            }

            var stFormD = text.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var caracter in stFormD)
            {
                var uc = CharUnicodeInfo.GetUnicodeCategory(caracter);

                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(caracter);
                }
            }

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}