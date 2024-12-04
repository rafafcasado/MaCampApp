using Aspbrasil.AppSettings;
using Aspbrasil.Models;
using FFImageLoading.Forms;
using System.Collections.Generic;
using System.Linq;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Aspbrasil.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TipoEstabelecimentoView : ContentView
    {
        public TipoEstabelecimentoView(List<ItemIdentificador> identificadores)
        {
            InitializeComponent();
            var tipos = identificadores.Where(i => i.Opcao == 0);

            int tamanhoIcone = Device.RuntimePlatform == Device.iOS ? 15 : 20;
            int tamanhoFonte = Device.RuntimePlatform == Device.iOS ? 10 : 13;

            int coluna = 0, linha = 0;
            foreach (var tipo in tipos)
            {
                Frame frTipo = new Frame { HasShadow = false, BackgroundColor = AppColors.COR_DESTAQUE, CornerRadius = 25, Padding = new Thickness(5), Margin = 5 };
                StackLayout slTipo = new StackLayout { Padding = 5, Spacing = 1, VerticalOptions = LayoutOptions.CenterAndExpand };
                CachedImage icone = new CachedImage { Source = tipo.Identificador.Replace("`", "").Replace("(", "").Replace(")", "").Replace("çã", "ca").Replace("/", "").ToLower() + ".png", WidthRequest = tamanhoIcone, HorizontalOptions = LayoutOptions.CenterAndExpand, VerticalOptions = LayoutOptions.Center };
                Label lbNome = new Label { Text = tipo.NomeExibicao, FontSize = tamanhoFonte, TextColor = Color.White, VerticalOptions = LayoutOptions.CenterAndExpand, HorizontalTextAlignment = TextAlignment.Center };

                slTipo.Children.Add(icone);
                slTipo.Children.Add(lbNome);
                frTipo.Content = slTipo;

                flTipos.Children.Add(frTipo, coluna, linha);
                coluna++;
                if (coluna == 2)
                {
                    linha++; coluna = 0;
                }
            }
        }
    }
}