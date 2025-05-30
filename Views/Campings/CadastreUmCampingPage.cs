﻿namespace MaCamp.Views.Campings
{
    public class CadastreUmCampingPage : ContentPage
    {
        public CadastreUmCampingPage()
        {
            Title = "Cadastre um camping";
            Padding = 0;

            Content = new WebView
            {
                Source = new HtmlWebViewSource
                {
                    // link antigo
                    // https://docs.google.com/forms/d/1ReCPcz25fjCSqB0uRH60W3NuCV8rnCtRzRbuUkf4CBE/viewform?edit_requested=true

                    Html = "<iframe src=\"https://docs.google.com/forms/d/e/1FAIpQLScYtVL4gNqVaBl5APaKKyVJ0cE3ORMWYGsc08KsIr2O_D3Vfg/viewform\" width=\"100%\" height=\"100%\" frameborder=\"0\" marginwidth=\"0\" marginheight=\"0\">Carregando…</iframe>"
                }
            };
        }
    }
}