using MaCamp.Models;

namespace MaCamp.Views.CustomCells
{
    public partial class CampingViewCell : ViewCell
    {
        private Item? ItemAtual;

        public CampingViewCell()
        {
            InitializeComponent();
            imItem.DownsampleWidth = App.SCREEN_WIDTH * 1.5;
            imItem.HeightRequest = Convert.ToDouble(App.SCREEN_WIDTH * 9 / 16);

            imIconeTipo1.Error += delegate
            {
                Dispatcher.Dispatch(() =>
                {
                    imIconeTipo1.IsVisible = false;
                });
            };

            imIconeTipo2.Error += delegate
            {
                Dispatcher.Dispatch(() =>
                {
                    imIconeTipo2.IsVisible = false;
                });
            };

            imIconeTipo1.Success += delegate
            {
                Dispatcher.Dispatch(() =>
                {
                    imIconeTipo1.IsVisible = true;
                });
            };

            imIconeTipo2.Success += delegate
            {
                Dispatcher.Dispatch(() =>
                {
                    imIconeTipo2.IsVisible = true;
                });
            };
        }

        protected override void OnBindingContextChanged()
        {
            ItemAtual = BindingContext as Item;

            if (ItemAtual == null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(ItemAtual.LinkUltimaFoto))
            {
                imItem.Source = "placeholder.jpg";
            }
            else
            {
                imItem.Source = Models.Services.CampingServices.MontarUrlImagemTemporaria(ItemAtual.LinkUltimaFoto);
            }

            if ((ItemAtual.Latitude != 0) & (ItemAtual.Longitude != 0))
            {
                imDirecoes.IsVisible = true;
            }
            else
            {
                imDirecoes.IsVisible = false;
            }

            Task.Run(() => CalcularDistancia());
            Task.Run(() => ExibirEstrelasETipos());

            base.OnBindingContextChanged();
        }

        private void ExibirEstrelasETipos()
        {
            var tipos = ItemAtual?.Identificadores.Where(i => i.Opcao == 0) ?? new List<ItemIdentificador>();
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

                if (ItemAtual != null)
                {
                    if (ItemAtual.QuantidadeEstrelas == -1)
                    {
                        slEstrelas.IsVisible = false;
                    }
                    else
                    {
                        slEstrelas.IsVisible = true;
                        estrela1.Source = "estrela.png";
                        estrela2.Source = "estrela.png";
                        estrela3.Source = "estrela.png";
                        estrela4.Source = "estrela.png";
                        estrela5.Source = "estrela.png";

                        if (ItemAtual.QuantidadeEstrelas > 0)
                        {
                            estrela1.Source = "estrela_selecionada.png";
                        }

                        if (ItemAtual.QuantidadeEstrelas > 1)
                        {
                            estrela2.Source = "estrela_selecionada.png";
                        }

                        if (ItemAtual.QuantidadeEstrelas > 2)
                        {
                            estrela3.Source = "estrela_selecionada.png";
                        }

                        if (ItemAtual.QuantidadeEstrelas > 3)
                        {
                            estrela4.Source = "estrela_selecionada.png";
                        }

                        if (ItemAtual.QuantidadeEstrelas > 4)
                        {
                            estrela5.Source = "estrela_selecionada.png";
                        }
                    }
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