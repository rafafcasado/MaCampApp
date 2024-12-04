
using Aspbrasil.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Aspbrasil.Views.Menu
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MasterPageItemViewCell : ViewCell
    {
        public MasterPageItemViewCell()
        {
            InitializeComponent();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            var itemMenu = BindingContext as ItemMenu;

            lbTexto.Text = itemMenu.Titulo;
            lbTexto.TextColor = Color.FromHex(itemMenu.HexCorTexto);
            imIcon.Source = itemMenu.IconSource;
            imIcon.IsVisible = itemMenu.ExibirIcone;
        }
    }
}