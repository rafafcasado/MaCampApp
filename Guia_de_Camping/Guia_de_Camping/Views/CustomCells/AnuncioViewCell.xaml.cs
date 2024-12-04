using Aspbrasil.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Aspbrasil.Views.CustomCells
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AnuncioViewCell : ViewCell
    {
        public AnuncioViewCell()
        {
            InitializeComponent();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            Item item = (Item)BindingContext;

            cvAnuncio.Content = new AnuncioView(item.Anuncio, App.SCREEN_WIDTH, App.SCREEN_WIDTH * 9 / 21);
        }
    }
}