using Aspbrasil.AppSettings;
using Aspbrasil.Models;
using Aspbrasil.Models.DataAccess;
using Newtonsoft.Json;
using Plugin.Geolocator;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Aspbrasil.Views.Campings
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FormBuscaView : ContentView
    {
        private string ParametroTODAS = " - TODAS - ";
        private string ParametroTODOS = " - TODOS - ";
        private string CampingsTodos = " ";
        private DBContract DB = DBContract.NewInstance();

        private string EstadoSelecionado;
        private string CidadeSelecionada;
        private string NomeDoCamping;

        public FormBuscaView()
        {
            InitializeComponent();

            Task.Run(() =>
            {
                DB.InserirOuSubstituirModelo(new ChaveValor { Chave = "FILTROS_ESTABELECIMENTO_SELECIONADOS", Valor = "" });
                DB.InserirOuSubstituirModelo(new ChaveValor { Chave = "FILTROS_SERVICO_SELECIONADOS", Valor = "" });

                CarregarCidadesEstados();
                CarregarLocalizacaoUsuario();
            });

            //TapGestureRecognizer buscarCidadeEstado = new TapGestureRecognizer();
            //buscarCidadeEstado.Tapped += (s, e) =>
            //{
            //    AlterarBuscaLocalizacao(true);
            //};
            //slBuscarCidadeEstado.GestureRecognizers.Add(buscarCidadeEstado);3s

            Plugin.GoogleAnalytics.GoogleAnalytics.Current.Tracker.SendView("Busca de Campings (Estado/Cidade)");

        }
        private void CarregarLocalizacaoUsuario()
        {
            string valorChaveUsarLocalizacaoUsuario = DB.ObterValorChave("FILTROS_LOCALIZACAO_SELECIONADA");
        }

        private void BuscaDeCampings()
        {


        }

        private async void CarregarCidadesEstados()
        {
            string EstadoBD = DB.ObterValorChave("FILTROS_ESTADO_SELECIONADO");
            string CIDADE_BD = DB.ObterValorChave("FILTROS_CIDADE_SELECIONADA");
            string NomeCampingBD = DB.ObterValorChave("FILTROS_NOME_DO_CAMPING");

            List<Cidade> cidadesWS = DB.ListarCidades();

            if (cidadesWS.Count == 0)
            {
                using (var client = new HttpClient())
                {
                    string url = "https://guiadecampings.homologacao.net/api/Cidades/GetCidades";
                    string jsonCidades = string.Empty;
                    try
                    {
                        jsonCidades = await client.GetStringAsync(url);
                        cidadesWS = JsonConvert.DeserializeObject<List<Cidade>>(jsonCidades).Where(x => !x.Estado.Contains("_")).ToList();
                    }
                    catch (Exception)
                    {

                    }
                    DB.InserirListaDeModelo(cidadesWS);
                }
            }

            var gruposCidadePorUF = cidadesWS.GroupBy(c => c.Estado);

            List<string> estados = new List<string>();
            estados.Add(ParametroTODOS);
            foreach (var grupoEstado in gruposCidadePorUF)
            {
                estados.Add(grupoEstado.Key);
            }

            Device.BeginInvokeOnMainThread(() =>
            {
                btBuscar.IsEnabled = true;
                pkUF.Title = "Selecione o Estado";
                pkUF.ItemsSource = estados;
                pkUF.SelectedIndexChanged += (s, e) =>
                {
                    EstadoSelecionado = ((s as Picker).SelectedItem as string);
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
                        cidadesDisponiveis.Insert(0, new Cidade { Nome = ParametroTODAS, Estado = EstadoSelecionado });
                        pkCidade.ItemsSource = cidadesDisponiveis;
                        pkCidade.ItemDisplayBinding = new Binding(nameof(Cidade.Nome));
                        pkCidade.Title = "Selecione a cidade";
                        pkCidade.IsEnabled = true;

                        pkCidade.SelectedIndexChanged += (senderCidade, eventCidade) =>
                        {
                            Cidade cidadeSelecionada = ((senderCidade as Picker).SelectedItem as Cidade);
                            if (cidadeSelecionada != null) { CidadeSelecionada = cidadeSelecionada.Nome; }
                            else { CidadeSelecionada = string.Empty; }
                        };
                        if (CIDADE_BD != null && CIDADE_BD != null)
                        {
                            pkCidade.SelectedItem = cidadesDisponiveis.Where(c => c.Nome == CIDADE_BD).FirstOrDefault();
                        }
                    }
                };
                pkUF.SelectedItem = EstadoBD;
                etNomeDoCamping.Text = NomeCampingBD;
            });
        }

        private async void UsarMinhaLocalizacao(object sender, EventArgs e)
        {
            loader.IsVisible = loader.IsRunning = true;
            slUsarMinhaLocalizacao.IsVisible = false;

            try
            {
                var locator = CrossGeolocator.Current;
                App.LOCALIZACAO_USUARIO = await locator.GetPositionAsync();
            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine(ex.Message); }

            loader.IsVisible = loader.IsRunning = false;
            //AlterarBuscaLocalizacao(false);

            DB.InserirOuSubstituirModelo(new ChaveValor { Chave = "FILTROS_LOCALIZACAO_SELECIONADA", Valor = "true" });
            DB.InserirOuSubstituirModelo(new ChaveValor { Chave = "FILTROS_ESTADO_SELECIONADO", Valor = null });
            DB.InserirOuSubstituirModelo(new ChaveValor { Chave = "FILTROS_CIDADE_SELECIONADA", Valor = null });
            DB.InserirOuSubstituirModelo(new ChaveValor { Chave = "FILTROS_NOME_DO_CAMPING", Valor = "" });

            Plugin.GoogleAnalytics.GoogleAnalytics.Current.Tracker.SendEvent("Usar minha localização", "Buscando campings próximos");

            MessagingCenter.Send(App.Current, AppConstants.MENSAGEM_BUSCA_REALIZADA);
        }

        private void btBuscar_Clicked(object sender, EventArgs e)
        {
            DB.InserirOuSubstituirModelo(new ChaveValor { Chave = "FILTROS_ESTADO_SELECIONADO", Valor = EstadoSelecionado == ParametroTODOS || EstadoSelecionado == string.Empty ? null : EstadoSelecionado });
            DB.InserirOuSubstituirModelo(new ChaveValor { Chave = "FILTROS_CIDADE_SELECIONADA", Valor = CidadeSelecionada == ParametroTODAS || CidadeSelecionada == string.Empty ? null : CidadeSelecionada });

            DB.InserirOuSubstituirModelo(new ChaveValor { Chave = "FILTROS_LOCALIZACAO_SELECIONADA", Valor = "false" });

            NomeDoCamping = RemoveDiacritics(etNomeDoCamping.Text);
            DB.InserirOuSubstituirModelo(new ChaveValor { Chave = "FILTROS_NOME_DO_CAMPING", Valor = NomeDoCamping == string.Empty ? null : NomeDoCamping });

            MessagingCenter.Send(App.Current, AppConstants.MENSAGEM_BUSCA_REALIZADA);
            pkUF.SelectedItem = null;
        }

        private string RemoveDiacritics(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return text;

            string stFormD = text.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();

            for (int ich = 0; ich < stFormD.Length; ich++)
            {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(stFormD[ich]);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(stFormD[ich]);
                }
            }
            return (sb.ToString().Normalize(NormalizationForm.FormC));
        }
    }
}