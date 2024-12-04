using MaCamp.Models;

namespace MaCamp.Views.CustomCells
{
    public partial class AnuncioViewCell : ViewCell
    {
        public AnuncioViewCell()
        {
            InitializeComponent();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (BindingContext is Item item)
            {
                cvAnuncio.Content = new AnuncioView(item.Anuncio, App.SCREEN_WIDTH, App.SCREEN_WIDTH * 9 / 21);
            }
        }
    }
}