using MaCamp.Views.Menu;

namespace MaCamp.Views.CustomCells
{
    public partial class MasterPageSubItemViewCell : ViewCell
    {
        public MasterPageSubItemViewCell()
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