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

            imItem.HeightRequest = Convert.ToDouble(App.SCREEN_WIDTH * 9 / 16);
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

                if (itemAtual.URLImagem != null && itemAtual.URLImagem.Contains(AppConstants.Url_DominioOficial))
                {
                    var urlImagem = CampingServices.MontarUrlImagemTemporaria(itemAtual.URLImagem);

                    imItem.Source = urlImagem;
                }

                grFoto.IsVisible = !string.IsNullOrEmpty(itemAtual.URLImagem);
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