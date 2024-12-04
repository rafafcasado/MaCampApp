using MaCamp.Models;

namespace MaCamp.Views.CustomCells
{
    public partial class AnuncioCardViewCell : ViewCell
    {
        public AnuncioCardViewCell()
        {
            InitializeComponent();
            imItem.DownsampleWidth = App.SCREEN_WIDTH * 1.5;
            imItem.HeightRequest = Convert.ToDouble(App.SCREEN_WIDTH * 9 / 16);
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (BindingContext is Item item)
            {
                imItem.Source = item.Anuncio?.UrlImagem;
            }
        }
    }
}