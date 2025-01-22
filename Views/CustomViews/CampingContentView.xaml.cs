using FFImageLoading.Maui;
using MaCamp.Models;
using MaCamp.Models.Services;
using MaCamp.Utils;
using Microsoft.Maui.Controls.Shapes;

namespace MaCamp.Views.CustomViews
{
    public partial class CampingContentView : ContentView
    {
        private Item? ItemAtual { get; set; }

        public CampingContentView()
        {
            InitializeComponent();

            var height = Convert.ToDouble(App.SCREEN_WIDTH * 9 / 16);

            imItem.DownsampleWidth = App.SCREEN_WIDTH * 1.5;
            imItem.HeightRequest = height;

            imItem.Clip = new RectangleGeometry
            {
                Rect = new Rect(0, 0, App.SCREEN_WIDTH - 20, height)
            };

            imIconeTipo1.Error += delegate
            {
                Dispatcher.Dispatch(() => imIconeTipo1.IsVisible = false);
            };

            imIconeTipo2.Error += delegate
            {
                Dispatcher.Dispatch(() => imIconeTipo2.IsVisible = false);
            };

            imIconeTipo1.Success += delegate
            {
                Dispatcher.Dispatch(() => imIconeTipo1.IsVisible = true);
            };

            imIconeTipo2.Success += delegate
            {
                Dispatcher.Dispatch(() => imIconeTipo2.IsVisible = true);
            };
        }

        protected override void OnBindingContextChanged()
        {
            if (BindingContext is Item itemAtual)
            {
                ItemAtual = itemAtual;
                imItem.Source = string.IsNullOrWhiteSpace(itemAtual.LinkUltimaFoto) ? "placeholder.jpg" : CampingServices.MontarUrlImagemTemporaria(itemAtual.LinkUltimaFoto);
                imDirecoes.IsVisible = itemAtual.Latitude != 0 && itemAtual.Longitude != 0;

                Task.Run(() => CalcularDistancia());
                Task.Run(() => ExibirEstrelasETipos());

                base.OnBindingContextChanged();
            }
        }

        private void ExibirEstrelasETipos()
        {
            var tipos = ItemAtual?.Identificadores.Where(x => x.Opcao == 0) ?? new List<ItemIdentificador>();
            var tipo1Foi = false;
            var sourceIconeTipo1 = default(string);
            var textoTipo1 = default(string);
            var sourceIconeTipo2 = default(string);
            var textoTipo2 = default(string);

            foreach (var tipo in tipos)
            {
                if (!tipo1Foi)
                {
                    tipo1Foi = true;
                    sourceIconeTipo1 = tipo.Identificador?.Replace("`", "").Replace("(", "").Replace(")", "").Replace("çã", "ca").Replace("/", "").ToLower() + ".png";
                    textoTipo1 = tipo.NomeExibicao;
                }
                else
                {
                    sourceIconeTipo2 = tipo.Identificador?.Replace("`", "").Replace("(", "").Replace(")", "").Replace("çã", "ca").Replace("/", "").ToLower() + ".png";
                    textoTipo2 = tipo.NomeExibicao;
                }
            }

            Dispatcher.Dispatch(() =>
            {
                imIconeTipo1.Source = sourceIconeTipo1;
                lbTipo1.Text = textoTipo1 ?? string.Empty;

                if (textoTipo2 != null)
                {
                    frTipo2.IsVisible = true;
                    imIconeTipo2.Source = sourceIconeTipo2;
                    lbTipo2.Text = textoTipo2;
                }

                if (ItemAtual != null && "não exibir" == "solicitado por Marcos")
                {
                    slEstrelas.IsVisible = ItemAtual.QuantidadeEstrelas > 0;

                    Enumerable.Range(0, 5).ForEach(x =>
                    {
                        var estrelaSelecionada = ItemAtual.QuantidadeEstrelas > x;
                        var imageSource = estrelaSelecionada ? "estrela_selecionada.png" : "estrela.png";
                        var estrela = new CachedImage
                        {
                            HeightRequest = 15,
                            WidthRequest = 15,
                            Source = imageSource
                        };

                        slEstrelas.Children.Add(estrela);
                    });
                }

                //cvTipo.Content = new TipoEstabelecimentoView(ItemAtual.Identificadores);
            });
        }

        private void CalcularDistancia()
        {
            if (ItemAtual != null)
            {
                var distancia = ItemAtual.DistanciaDoUsuario;

                if (distancia > 0)
                {
                    var d = distancia > 1000 ? Math.Round(distancia / 1000, 2) : Math.Round(distancia, 2);
                    var unidade = distancia > 1000 ? "km" : "m";

                    Dispatcher.Dispatch(() =>
                    {
                        lbDistancia.Text = d + unidade + " de distância";
                    });
                }
            }
        }

        public async void Compartilhar(object sender, EventArgs e)
        {
            if (ItemAtual != null)
            {
                await Share.RequestAsync(new ShareTextRequest
                {
                    Text = ItemAtual.Nome,
                    Uri = ItemAtual.UrlExterna
                });
            }
        }

        private async void AbrirDirecoes(object sender, EventArgs e)
        {
            if (ItemAtual?.Latitude != null && ItemAtual.Longitude != null)
            {
                await Map.OpenAsync(ItemAtual.Latitude.Value, ItemAtual.Longitude.Value, new MapLaunchOptions
                {
                    Name = ItemAtual.Nome
                });
            }
        }
    }
}