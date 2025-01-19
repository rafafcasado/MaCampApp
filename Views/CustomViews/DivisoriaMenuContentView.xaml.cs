using MaCamp.Views.Menu;

namespace MaCamp.Views.CustomViews
{
    public partial class DivisoriaMenuContentView : ContentView
    {
        public DivisoriaMenuContentView()
        {
            InitializeComponent();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (BindingContext is ItemMenu itemMenu)
            {
                //lbTexto.Text = itemMenu.Titulo;
                //lbTexto.TextColor = Color.FromHex(itemMenu.HexCorTexto);
            }
        }
    }
}