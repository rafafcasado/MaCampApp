using MaCamp.AppSettings;
using MaCamp.Models;

namespace MaCamp.Views.CustomCells
{
    public partial class ItemViewCell : ViewCell
    {
        private Item? ItemAtual { get; set; }

        public ItemViewCell()
        {
            InitializeComponent();

            imItem.DownsampleWidth = App.SCREEN_WIDTH * 1.5;
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

                if (itemAtual.image != null && itemAtual.image.Contains(AppConstants.UrlDominioOficial))
                {
                    imItem.Source = Models.Services.CampingServices.MontarUrlImagemTemporaria(itemAtual.image);
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