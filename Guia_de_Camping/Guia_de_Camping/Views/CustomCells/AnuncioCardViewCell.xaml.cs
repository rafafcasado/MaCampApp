using Aspbrasil.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Aspbrasil.Views.CustomCells
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AnuncioCardViewCell : ViewCell
    {
        public AnuncioCardViewCell()
        {
            InitializeComponent();
            imItem.DownsampleWidth = App.SCREEN_WIDTH * 1.5;
            imItem.HeightRequest = App.SCREEN_WIDTH * 9 / 16;
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            Item item = (Item)BindingContext;

            imItem.Source = item.Anuncio.URLImagem;
        }
    }
}