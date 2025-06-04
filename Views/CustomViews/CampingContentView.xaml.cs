using FFImageLoading.Maui;
using MaCamp.Models;
using MaCamp.Services;
using MaCamp.Utils;
using Microsoft.Maui.Controls.Shapes;
using Map = Microsoft.Maui.ApplicationModel.Map;

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
                imIconeTipo1.IsVisible = false;
            };

            imIconeTipo2.Error += delegate
            {
                imIconeTipo2.IsVisible = false;
            };

            imIconeTipo1.Success += delegate
            {
                imIconeTipo1.IsVisible = true;
            };

            imIconeTipo2.Success += delegate
            {
                imIconeTipo2.IsVisible = true;
            };

        }

        protected override void OnBindingContextChanged()
        {
            if (BindingContext is Item itemAtual)
            {
                ItemAtual = itemAtual;
                imItem.Source = string.IsNullOrEmpty(itemAtual.LinkUltimaFoto) ? "placeholder.jpg" : CampingServices.MontarUrlImagemTemporaria(itemAtual.LinkUltimaFoto);
                imDirecoes.IsVisible = itemAtual.IsValidLocation();

                CalcularDistancia();
                //ExibirEstrelas();
                ExibirTipos();

                base.OnBindingContextChanged();
            }
        }

        private void ExibirTipos()
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
                    sourceIconeTipo1 = tipo.Identificador?.Replace("`", string.Empty).Replace("(", string.Empty).Replace(")", string.Empty).Replace("çã", "ca").Replace("/", string.Empty).ToLower() + ".png";
                    textoTipo1 = tipo.NomeExibicao;
                }
                else
                {
                    sourceIconeTipo2 = tipo.Identificador?.Replace("`", string.Empty).Replace("(", string.Empty).Replace(")", string.Empty).Replace("çã", "ca").Replace("/", string.Empty).ToLower() + ".png";
                    textoTipo2 = tipo.NomeExibicao;
                }
            }

            imIconeTipo1.Source = sourceIconeTipo1;
            lbTipo1.Text = textoTipo1 ?? string.Empty;
            frTipo.IsVisible = !string.IsNullOrEmpty(textoTipo1);

            if (textoTipo2 != null)
            {
                frTipo2.IsVisible = true;
                imIconeTipo2.Source = sourceIconeTipo2;
                lbTipo2.Text = textoTipo2;
            }

            //cvTipo.Content = new TipoEstabelecimentoView(ItemAtual.Identificadores);
        }
        private void ExibirEstrelas()
        {
            if (ItemAtual != null)
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
        }

        private void CalcularDistancia()
        {
            var distance = ItemAtual.GetDistanceKilometersFromUser();

            if (distance is double distanceKilometers)
            {
                if (distanceKilometers < 1)
                {
                    // Menos de 1km, exibe em metros
                    var distanceMeters = Math.Round(distanceKilometers * 1000, 0);

                    lbDistancia.Text = $"{distanceMeters} m de distância";
                }
                else
                {
                    // 1km ou mais, exibe em km
                    var distanceRoundedKm = Math.Round(distanceKilometers, 2);

                    lbDistancia.Text = $"{distanceRoundedKm} km de distância";
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