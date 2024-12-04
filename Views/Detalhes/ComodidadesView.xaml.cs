using MaCamp.AppSettings;
using MaCamp.Models;
using Grid = Microsoft.Maui.Controls.Grid;

namespace MaCamp.Views.Detalhes
{
    public partial class ComodidadesView : ContentView
    {
        public ComodidadesView(List<ItemIdentificador> itemIdentificadores)
        {
            InitializeComponent();
            var secoes = itemIdentificadores.Where(i => i.Opcao > 0).GroupBy(i => i.Secao);

            foreach (var secao in secoes)
            {
                var lbTitulo = new Label
                {
                    Text = secao.Key ?? string.Empty,
                    TextColor = AppColors.CorPrimaria,
                    FontAttributes = FontAttributes.Bold,
                    Margin = new Thickness(0, 10)
                };

                slContent.Children.Add(lbTitulo);
                var grupoItens = secao.GroupBy(i => i.Identificador);

                foreach (var grupoItem in grupoItens)
                {
                    var itens = grupoItem.ToList();

                    var grComodidades = new Grid()
                    {
                        ColumnSpacing = 15,
                        Margin = new Thickness(0, 5)
                    };

                    grComodidades.ColumnDefinitions.Add(new ColumnDefinition
                    {
                        Width = GridLength.Auto
                    });

                    grComodidades.ColumnDefinitions.Add(new ColumnDefinition
                    {
                        Width = GridLength.Star
                    });

                    for (var i = 0; i < itens.Count; i++)
                    {
                        grComodidades.RowDefinitions.Add(new RowDefinition
                        {
                            Height = GridLength.Star
                        });
                    }

                    var imIcone = new Image
                    {
                        Source = itens[0].Identificador?.ToLower() + ".png", HeightRequest = 35,
                        VerticalOptions = LayoutOptions.CenterAndExpand
                    };

                    grComodidades.Children.Add(imIcone);
                    Grid.SetRowSpan(imIcone, itens.Count);
                    var linha = 0;

                    foreach (var item in itens)
                    {
                        var label = new Label
                        {
                            FontAttributes = FontAttributes.Bold,
                            Text = item.NomeExibicaoOpcao ?? string.Empty,
                            VerticalOptions = LayoutOptions.CenterAndExpand
                        };

                        Grid.SetColumn(label, 1);
                        Grid.SetRow(label, linha);
                        linha++;
                    }

                    slContent.Children.Add(grComodidades);

                    slContent.Children.Add(new BoxView
                    {
                        HeightRequest = 1,
                        BackgroundColor = Color.FromArgb("20000000"),
                        Margin = new Thickness(0, 5)
                    });
                }
            }
        }
    }
}