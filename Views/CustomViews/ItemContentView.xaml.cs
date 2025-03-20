using MaCamp.Models;
using MaCamp.Services;
using MaCamp.Utils;

namespace MaCamp.Views.CustomViews
{
    public partial class ItemContentView : ContentView
    {
        private Item? ItemAtual { get; set; }

        public ItemContentView()
        {
            InitializeComponent();

            //imItem.DownsampleWidth = App.SCREEN_WIDTH * 1.5;
            imItem.HeightRequest = Convert.ToDouble(App.SCREEN_WIDTH * 9 / 16);
            //imPlay.HeightRequest = App.SCREEN_HEIGHT / 10;
        }

        protected override void OnBindingContextChanged()
        {
            if (BindingContext is Item itemAtual)
            {
                ItemAtual = itemAtual;

                if (itemAtual.Tag == "app-eventos")
                {
                    testeLabel.IsVisible = false;
                }

                if (itemAtual.image != null && itemAtual.image.Contains(AppConstants.Url_DominioOficial))
                {
                    var urlImagem = CampingServices.MontarUrlImagemTemporaria(itemAtual.image);

                    imItem.Source = urlImagem;
                }

                grFoto.IsVisible = !string.IsNullOrWhiteSpace(itemAtual.image);

                //lbSubtitulo.Text = ItemAtual.Descricao.Substring(0, 100);

                //if (ItemAtual.hasVideo)
                //{
                //    imPlay.Source = "ic_play.png";
                //    cvVideo.IsVisible = true;
                //}
                //else { cvVideo.IsVisible = false; }
            }

            base.OnBindingContextChanged();
        }

        public async void Compartilhar(object sender, EventArgs e)
        {
            if (ItemAtual != null)
            {
                await Share.RequestAsync(new ShareTextRequest
                {
                    Text = ItemAtual.Titulo,
                    Uri = ItemAtual.UrlSite
                });
            }
        }
    }
}