using Aspbrasil.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Aspbrasil.Views.Menu
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MasterPageSubItemViewCell : ViewCell
    {
        public MasterPageSubItemViewCell()
        {
            InitializeComponent();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            var itemMenu = BindingContext as ItemMenu;

            lbTexto.Text = itemMenu.Titulo;
            lbTexto.TextColor = Color.FromHex(itemMenu.HexCorTexto);
        }
    }
}