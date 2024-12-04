using Aspbrasil.AppSettings;
using Aspbrasil.Models;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Aspbrasil.Views.Detalhes
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ComodidadesView : ContentView
    {
        public ComodidadesView(List<ItemIdentificador> itemIdentificadores)
        {
            InitializeComponent();

            var secoes = itemIdentificadores.Where(i => i.Opcao > 0).GroupBy(i => i.Secao);

            foreach (var secao in secoes)
            {
                Label lbTitulo = new Label { Text = secao.Key, TextColor = AppColors.COR_PRIMARIA, FontAttributes = FontAttributes.Bold, FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)), Margin = new Thickness(0, 10) };
                slContent.Children.Add(lbTitulo);

                var grupoItens = secao.GroupBy(i => i.Identificador);
                foreach (var grupoItem in grupoItens)
                {
                    var itens = grupoItem.ToList();

                    Grid grComodidades = new Grid() { ColumnSpacing = 15, Margin = new Thickness(0, 5) };
                    grComodidades.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                    grComodidades.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
                    for (int i = 0; i < itens.Count; i++)
                    {
                        grComodidades.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
                    }

                    var imIcone = new Image { Source = itens[0].Identificador.ToLower() + ".png", HeightRequest = 35, VerticalOptions = LayoutOptions.CenterAndExpand };
                    grComodidades.Children.Add(imIcone, 0, 0);
                    Grid.SetRowSpan(imIcone, itens.Count());

                    int linha = 0;
                    foreach (var item in itens)
                    {
                        grComodidades.Children.Add(new Label { FontAttributes = FontAttributes.Bold, Text = item.NomeExibicaoOpcao, FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)), VerticalOptions = LayoutOptions.CenterAndExpand }, 1, linha);
                        linha++;
                    }

                    slContent.Children.Add(grComodidades);

                    slContent.Children.Add(new BoxView { HeightRequest = 1, BackgroundColor = Color.FromHex("20000000"), Margin = new Thickness(0, 5) });
                }
            }
        }
    }
}