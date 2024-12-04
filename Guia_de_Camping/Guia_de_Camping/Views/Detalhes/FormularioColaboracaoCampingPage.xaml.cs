using System;
using System.Net.Http;
using System.Text;
using Aspbrasil.AppSettings;
using Aspbrasil.Models;
using Aspbrasil.Models.DataAccess;
using Aspbrasil.Views.Popups;
using Newtonsoft.Json;
using Rg.Plugins.Popup.Extensions;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Aspbrasil.Views.Detalhes
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FormularioColaboracaoCampingPage : ContentPage
    {
        string NomeDoCamping;
        int IdDoCamping;

        public FormularioColaboracaoCampingPage(string nomeDoCamping, int idDoCamping)
        {
            NomeDoCamping = nomeDoCamping;
            IdDoCamping = idDoCamping;

            InitializeComponent();

            try
            {
                DadosColaborador();
            }
            catch
            {
                etEmail.Text = string.Empty;
                etNome.Text = string.Empty;
                Equipamento.Text = string.Empty;
                Informacao.Text = string.Empty;
                etValorPagoPorDiaria.Text = string.Empty;
            }

            Plugin.GoogleAnalytics.GoogleAnalytics.Current.Tracker.SendView("Formulário de colaboração: " + nomeDoCamping);

        }

        private void DadosColaborador()
        {
            DBContract DB = new DBContract();
            etEmail.Text = DB.Consultar().Email;
            etNome.Text = DB.Consultar().Nome;
            Equipamento.Text = DB.Consultar().Equipamento;
        }

        private async void EnviarColaboracao(object sender, EventArgs e)
        {

            Plugin.GoogleAnalytics.GoogleAnalytics.Current.Tracker.SendEvent("Enviar Colaboração", NomeDoCamping);


            if (String.IsNullOrWhiteSpace(etNome.Text))
            {
                await DisplayAlert("Aviso", "Digite seu nome.", "OK");
                return;
            }

            if (String.IsNullOrWhiteSpace(Informacao.Text))
            {
                await DisplayAlert("Aviso", "Digite sua mensagem.", "OK");
                return;
            }

            bool valido = EmailValidation.ValidarEmail(etEmail.Text);

            if (valido)
            {
                Colaboracao colaboracao = new Colaboracao
                {
                    Nome = etNome.Text,
                    Email = etEmail.Text,
                    Camping = NomeDoCamping,
                    IDCamping = IdDoCamping,
                    Informacao = Informacao.Text,
                    ValorPagoDiaria = etValorPagoPorDiaria.Text,
                    Equipamento = Equipamento.Text
                };

                DBContract DB = new DBContract();
                DB.InserirOuSubstituirModelo(colaboracao);

                await Navigation.PushPopupAsync(new LoadingPopupPage(AppColors.COR_PRIMARIA));

                using (HttpClient client = new HttpClient())
                {
                    string jsonColaboracao = JsonConvert.SerializeObject(colaboracao);
                    StringContent content = new StringContent(jsonColaboracao, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync("https://guiadecampings.homologacao.net/API/Colaboracao/EnviarEmail", content);
                    await Navigation.PopPopupAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        bool alert = await DisplayAlert("Colaboração enviada!", "Envie suas fotos para adicionar ao APP MaCamp via whatsapp", "Enviar imagens agora", "Agora não");
                        if (alert == true)
                        {
                            await Launcher.OpenAsync("https://api.whatsapp.com/send?phone=5511950490907&text=ENVIE%20FOTOS%20DAS%20INSTALA%C3%87%C3%95ES%20DO%20CAMPING.%20Evite%20fotos%20de%20galera%2C%20selfies%20e%20de%20crian%C3%A7as.%20N%C3%83O%20ESQUE%C3%87A%20DE%20MENCIONAR%20SEU%20NOME%20E%20%20QUAL%20CAMPING!");
                        }
                        etNome.Text = string.Empty;
                        etEmail.Text = string.Empty;
                        etValorPagoPorDiaria.Text = string.Empty;
                        Equipamento.Text = string.Empty;
                        Informacao.Text = string.Empty;
                    }
                    else
                    {
                        await DisplayAlert("Aviso", "Colaboracao não enviada, tente novamente mais tarde.", "OK");
                    }
                }
            }
            else
            {
                await DisplayAlert("Aviso", "Preencha seu e-mail corretamente.", "OK");
            }
        }
    }
}