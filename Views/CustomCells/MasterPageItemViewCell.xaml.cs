using MaCamp.Views.Menu;

namespace MaCamp.Views.CustomCells
{
    public partial class MasterPageItemViewCell : ViewCell
    {
        public MasterPageItemViewCell()
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
                imIcon.Source = itemMenu.IconSource;
                imIcon.IsVisible = itemMenu.ExibirIcone;
            }
        }
    }
}