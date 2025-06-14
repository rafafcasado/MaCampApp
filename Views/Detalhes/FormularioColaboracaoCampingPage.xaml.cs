﻿using MaCamp.CustomControls;
using MaCamp.Models;
using MaCamp.Services;
using MaCamp.Utils;
using MaCamp.Views.Popups;
using RGPopup.Maui.Extensions;

namespace MaCamp.Views.Detalhes
{
    public partial class FormularioColaboracaoCampingPage : SmartContentPage
    {
        private string NomeDoCamping { get; }
        private int IdDoCamping { get; }

        public FormularioColaboracaoCampingPage(string nomeDoCamping, int idDoCamping)
        {
            InitializeComponent();

            NomeDoCamping = nomeDoCamping;
            IdDoCamping = idDoCamping;

            FirstAppeared += FormularioColaboracaoCampingPage_FirstAppeared;

            //Plugin.GoogleAnalytics.GoogleAnalytics.Current.Tracker.SendView("Formulário de colaboração: " + nomeDoCamping);
        }

        private async void FormularioColaboracaoCampingPage_FirstAppeared(object? sender, EventArgs e)
        {
            var colaboracao = await DBContract.GetAsync<Colaboracao>();

            etEmail.Text = colaboracao?.Email ?? string.Empty;
            etNome.Text = colaboracao?.Nome ?? string.Empty;
            Equipamento.Text = colaboracao?.Equipamento ?? string.Empty;
            etWhatsApp.Text = colaboracao?.WhatsApp ?? string.Empty;
        }

        private async void EnviarColaboracao(object sender, EventArgs e)
        {
            //Plugin.GoogleAnalytics.GoogleAnalytics.Current.Tracker.SendEvent("Enviar Colaboração", NomeDoCamping);

            if (string.IsNullOrEmpty(etNome.Text))
            {
                await DisplayAlert("Aviso", "Digite seu nome.", "OK");

                return;
            }

            if (string.IsNullOrEmpty(Informacao.Text))
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
                    WhatsApp = etWhatsApp.Text,
                    Camping = NomeDoCamping,
                    IDCamping = IdDoCamping,
                    Informacao = Informacao.Text,
                    ValorPagoDiaria = etValorPagoPorDiaria.Text,
                    Equipamento = Equipamento.Text
                };

                await DBContract.UpdateAsync(colaboracao);

                await Navigation.PushPopupAsync(new LoadingPopupPage(AppColors.CorPrimaria));

                var response = await AppNet.PostAsync(AppConstants.Url_EnviarEmail, colaboracao);

                await Navigation.PopPopupAsync();

                if (response)
                {
                    var alert = await DisplayAlert("Colaboração enviada!", "Envie suas fotos para adicionar ao APP MaCamp via whatsapp", "Enviar imagens agora", "Agora não");

                    if (alert)
                    {
                        await Launcher.OpenAsync("https://api.whatsapp.com/send?phone=5511950490907&text=ENVIE%20FOTOS%20DAS%20INSTALA%C3%87%C3%95ES%20DO%20CAMPING.%20Evite%20fotos%20de%20galera%2C%20selfies%20e%20de%20crian%C3%A7as.%20N%C3%83O%20ESQUE%C3%87A%20DE%20MENCIONAR%20SEU%20NOME%20E%20%20QUAL%20CAMPING!");
                    }

                    etNome.Text = string.Empty;
                    etEmail.Text = string.Empty;
                    etWhatsApp.Text = string.Empty;
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