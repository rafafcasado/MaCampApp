using MaCamp.Views.Menu;

namespace MaCamp.Views.CustomViews
{
    public partial class MasterPageSubItemContentView : ContentView
    {
        public MasterPageSubItemContentView()
        {
            InitializeComponent();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (BindingContext is ItemMenu itemMenu)
            {
                lbTexto.Text = itemMenu.Titulo ?? string.Empty;
                lbTexto.TextColor = Color.FromArgb(itemMenu.HexCorTexto);
            }
        }
    }
}