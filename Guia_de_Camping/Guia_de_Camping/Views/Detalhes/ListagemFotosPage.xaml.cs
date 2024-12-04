using Aspbrasil.Models;
using FFImageLoading.Forms;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Aspbrasil.Views.Detalhes
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListagemFotosPage : ContentPage
    {
        public ListagemFotosPage(IEnumerable<string> urlsFotos)
        {
            InitializeComponent();

            Plugin.GoogleAnalytics.GoogleAnalytics.Current.Tracker.SendView("Listagem de fotos ");

            grContent.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            grContent.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });

            if (urlsFotos.Count() == 1)
            {
                grContent.RowDefinitions.Add(new RowDefinition { Height = App.SCREEN_HEIGHT / 3 });
            }
            else
            {
                for (int i = 0; i < urlsFotos.Count() / 2; i++)
                {
                    grContent.RowDefinitions.Add(new RowDefinition { Height = App.SCREEN_HEIGHT / 3 });
                }
            }

            int linha = 0, coluna = 0;
            foreach (var url in urlsFotos)
            {
                string urlTemp = Aspbrasil.Models.Services.CampingServices.MontarUrlImagemTemporaria(url);
                var imagem = new CachedImage { Source = urlTemp, HeightRequest = 240, DownsampleHeight = 300, Aspect = Aspect.AspectFill, LoadingPlaceholder = "placeholder.jpg" };
                TapGestureRecognizer abrirFoto = new TapGestureRecognizer();
                abrirFoto.Tapped += async (s, e) =>
                {
                    await Navigation.PushModalAsync(new VisualizacaoFotoPage(url));
                };
                imagem.GestureRecognizers.Add(abrirFoto);
                //flContent.Children.Add(imagem);
                grContent.Children.Add(imagem, coluna, linha);
                coluna++;

                if (coluna > 1) { linha++; coluna = 0; }
            }
        }
    }
}