﻿using System.Text;
using MaCamp.AppSettings;
using MaCamp.Models;
using MaCamp.Models.DataAccess;
using MaCamp.Views.Popups;
using Newtonsoft.Json;
using RGPopup.Maui.Extensions;

namespace MaCamp.Views.Detalhes
{
    public partial class FormularioColaboracaoCampingPage : ContentPage
    {
        private string NomeDoCamping { get; }
        private int IdDoCamping { get; }

        public FormularioColaboracaoCampingPage(string nomeDoCamping, int idDoCamping)
        {
            InitializeComponent();

            NomeDoCamping = nomeDoCamping;
            IdDoCamping = idDoCamping;

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

            //Plugin.GoogleAnalytics.GoogleAnalytics.Current.Tracker.SendView("Formulário de colaboração: " + nomeDoCamping);
        }

        private void DadosColaborador()
        {
            var DB = new DBContract();

            etEmail.Text = DB.Consultar().Email ?? string.Empty;
            etNome.Text = DB.Consultar().Nome ?? string.Empty;
            Equipamento.Text = DB.Consultar().Equipamento ?? string.Empty;
        }

        private async void EnviarColaboracao(object sender, EventArgs e)
        {
            //Plugin.GoogleAnalytics.GoogleAnalytics.Current.Tracker.SendEvent("Enviar Colaboração", NomeDoCamping);

            if (string.IsNullOrWhiteSpace(etNome.Text))
            {
                await DisplayAlert("Aviso", "Digite seu nome.", "OK");

                return;
            }

            if (string.IsNullOrWhiteSpace(Informacao.Text))
            {
                await DisplayAlert("Aviso", "Digite sua mensagem.", "OK");

                return;
            }

            var valido = EmailValidation.ValidarEmail(etEmail.Text);

            if (valido)
            {
                var colaboracao = new Colaboracao
                {
                    Nome = etNome.Text,
                    Email = etEmail.Text,
                    Camping = NomeDoCamping,
                    IDCamping = IdDoCamping,
                    Informacao = Informacao.Text,
                    ValorPagoDiaria = etValorPagoPorDiaria.Text,
                    Equipamento = Equipamento.Text
                };

                var DB = new DBContract();
                DB.InserirOuSubstituirModelo(colaboracao);
                await Navigation.PushPopupAsync(new LoadingPopupPage(AppColors.CorPrimaria));
                using var client = new HttpClient();
                var jsonColaboracao = JsonConvert.SerializeObject(colaboracao);
                var content = new StringContent(jsonColaboracao, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(AppConstants.Url_EnviarEmail, content);
                await Navigation.PopPopupAsync();

                if (response.IsSuccessStatusCode)
                {
                    var alert = await DisplayAlert("Colaboração enviada!", "Envie suas fotos para adicionar ao APP MaCamp via whatsapp", "Enviar imagens agora", "Agora não");

                    if (alert)
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
            else
            {
                await DisplayAlert("Aviso", "Preencha seu e-mail corretamente.", "OK");
            }
        }
    }
}