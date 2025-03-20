using MaCamp.Models.Anuncios;
using MaCamp.Services.DataAccess;
using static MaCamp.Utils.Enumeradores;

namespace MaCamp.Views
{
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

        public AnuncioView(Anuncio? anuncio, int w, int h)
        {
            InitializeComponent();

            ExibirAnuncio(anuncioEscolhido: anuncio, width: w, height: h);
        }

        private async void ExibirAnuncio(TipoAnuncio tipoAnuncio = TipoAnuncio.Banner, Anuncio? anuncioEscolhido = null, int width = 0, int height = 0)
        {
            if (anuncioEscolhido == null)
            {
                var listaAnuncios = await AnunciosServices.GetListAsync(false);
                var anuncios = listaAnuncios.Where(x => x.Tipo == tipoAnuncio).ToList();
                var random = new Random();

                if (anuncios.Count == 0)
                {
                    imAnuncio.IsVisible = false;

                    return;
                }

                anuncioEscolhido = anuncios[random.Next(anuncios.Count)];
            }

            if (width == 0 || height == 0)
            {
                switch (tipoAnuncio)
                {
                    case TipoAnuncio.Popup:
                        imAnuncio.WidthRequest = App.SCREEN_WIDTH;
                        imAnuncio.HeightRequest = App.SCREEN_HEIGHT * 0.8;
                        break;
                    case TipoAnuncio.Banner:
                        imAnuncio.WidthRequest = App.SCREEN_WIDTH;
                        imAnuncio.HeightRequest = App.SCREEN_WIDTH * 1.2 / 10;
                        break;
                    default:
                        imAnuncio.WidthRequest = App.SCREEN_WIDTH;
                        imAnuncio.HeightRequest = Convert.ToDouble(App.SCREEN_WIDTH * 9 / 21);
                        break;
                }
            }
            else
            {
                imAnuncio.WidthRequest = width;
                imAnuncio.HeightRequest = height;
            }

            var gestureRecognizer = new TapGestureRecognizer();

            imAnuncio.Source = anuncioEscolhido.UrlImagem;

            gestureRecognizer.Tapped += async delegate
            {
                if (anuncioEscolhido.UrlExterna != null)
                {
                    await Launcher.OpenAsync(anuncioEscolhido.UrlExterna);
                }
            };

            GestureRecognizers.Add(gestureRecognizer);
        }
    }
}