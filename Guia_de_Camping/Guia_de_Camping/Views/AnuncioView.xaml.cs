using System;
using System.Collections.Generic;
using System.Linq;
using Aspbrasil.Models;
using Aspbrasil.Models.DataAccess;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Aspbrasil.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AnuncioView : ContentView
    {
        public AnuncioView()
        {
            InitializeComponent();
            ExibirAnuncio();
        }


        public AnuncioView(TipoAnuncio tipoAnuncio = TipoAnuncio.Banner)
        {
            InitializeComponent();
            ExibirAnuncio(tipoAnuncio);
        }


        public AnuncioView(Anuncio anuncio, int w, int h)
        {
            InitializeComponent();
            ExibirAnuncio(anuncioEscolhido: anuncio, width: w, height: h);
        }

        private async void ExibirAnuncio(TipoAnuncio tipoAnuncio = TipoAnuncio.Banner, Anuncio anuncioEscolhido = null, int width = 0, int height = 0)
        {
            if (anuncioEscolhido == null)
            {
                List<Anuncio> anuncios = (await AnuncioDA.ObterAnuncios()).Where(a => a.Tipo == tipoAnuncio).ToList();
                if (anuncios.Count == 0)
                {
                    imAnuncio.IsVisible = false;
                    return;
                }
                Random r = new Random();

                anuncioEscolhido = anuncios[r.Next(anuncios.Count)];
            }

            if (width == 0 || height == 0)
            {
                if (tipoAnuncio == TipoAnuncio.Popup)
                {
                    imAnuncio.WidthRequest = App.SCREEN_WIDTH;
                    imAnuncio.HeightRequest = App.SCREEN_HEIGHT * 0.8;
                }
                else if (tipoAnuncio == TipoAnuncio.Banner)
                {
                    imAnuncio.WidthRequest = App.SCREEN_WIDTH;
                    imAnuncio.HeightRequest = (App.SCREEN_WIDTH * 1.2 / 10);
                }
                else
                {
                    imAnuncio.WidthRequest = App.SCREEN_WIDTH;
                    imAnuncio.HeightRequest = (App.SCREEN_WIDTH * 9 / 21);
                }
            }
            else
            {
                imAnuncio.WidthRequest = width;
                imAnuncio.HeightRequest = height;
            }

            imAnuncio.Source = anuncioEscolhido.URLImagem;

            TapGestureRecognizer abrirAnuncio = new TapGestureRecognizer();
            abrirAnuncio.Tapped += async (s, e) => await Launcher.OpenAsync(anuncioEscolhido.URLExterna);
            GestureRecognizers.Add(abrirAnuncio);
        }
    }
}