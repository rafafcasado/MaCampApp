using MaCamp.Models;

namespace MaCamp.Views.CustomViews
{
    public partial class AnuncioContentView : ContentView
    {
        public AnuncioContentView()
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