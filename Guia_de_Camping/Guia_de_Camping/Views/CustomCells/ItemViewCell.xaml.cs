using Aspbrasil.Models;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Aspbrasil.Views.CustomCells
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ItemViewCell : ViewCell
    {
        Item ItemAtual;

        public ItemViewCell()
        {
            InitializeComponent();
            imItem.DownsampleWidth = App.SCREEN_WIDTH * 1.5;
            imItem.HeightRequest = App.SCREEN_WIDTH * 9 / 16;
            //imPlay.HeightRequest = App.SCREEN_HEIGHT / 10;
        }

        protected override void OnBindingContextChanged()
        {
            ItemAtual = BindingContext as Item;

            if(ItemAtual.Tag == "app-eventos")
            {
                testeLabel.IsVisible = false;
            }

            if (ItemAtual == null) { return; }

            if (ItemAtual.image.Contains("https://macamp.com.br")==true )
            {
                imItem.Source = Aspbrasil.Models.Services.CampingServices.MontarUrlImagemTemporaria(ItemAtual.image);
            }

            grFoto.IsVisible = !string.IsNullOrWhiteSpace(ItemAtual.image);
            
            //lbSubtitulo.Text = ItemAtual.Descricao.Substring(0, 100);

            //if (ItemAtual.hasVideo)
            //{
            //    imPlay.Source = "ic_play.png";
            //    cvVideo.IsVisible = true;
            //}
            //else { cvVideo.IsVisible = false; }

            base.OnBindingContextChanged();
        }

        public async void Compartilhar(object sender, EventArgs e)
        {
            if (ItemAtual != null)
            {
                await Plugin.Share.CrossShare.Current.Share(new Plugin.Share.Abstractions.ShareMessage { Text = ItemAtual.Titulo, Url = ItemAtual.UrlSite }, new Plugin.Share.Abstractions.ShareOptions { ChooserTitle = "Selecione" });
            }
        }
    }
}