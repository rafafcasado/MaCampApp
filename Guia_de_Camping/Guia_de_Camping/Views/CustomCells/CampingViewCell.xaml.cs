using Aspbrasil.Models;
using Plugin.ExternalMaps;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Aspbrasil.Views.CustomCells
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CampingViewCell : ViewCell
    {
        Item ItemAtual;

        public CampingViewCell()
        {
            InitializeComponent();
            imItem.DownsampleWidth = App.SCREEN_WIDTH * 1.5;
            imItem.HeightRequest = App.SCREEN_WIDTH * 9 / 16;

            imIconeTipo1.Error += (s, e) => { Device.BeginInvokeOnMainThread(() => { imIconeTipo1.IsVisible = false; }); };
            imIconeTipo2.Error += (s, e) => { Device.BeginInvokeOnMainThread(() => { imIconeTipo2.IsVisible = false; }); };

            imIconeTipo1.Success += (s, e) => { Device.BeginInvokeOnMainThread(() => { imIconeTipo1.IsVisible = true; }); };
            imIconeTipo2.Success += (s, e) => { Device.BeginInvokeOnMainThread(() => { imIconeTipo2.IsVisible = true; }); };
        }

        protected override void OnBindingContextChanged()
        {
            ItemAtual = BindingContext as Item;
            if (ItemAtual == null) { return; }

            if (string.IsNullOrWhiteSpace(ItemAtual.LinkUltimaFoto)) { imItem.Source = "placeholder.jpg"; }
            else { imItem.Source = Aspbrasil.Models.Services.CampingServices.MontarUrlImagemTemporaria(ItemAtual.LinkUltimaFoto); }

            if (ItemAtual.Latitude != 0 & ItemAtual.Longitude != 0)
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
            var tipos = ItemAtual.Identificadores.Where(i => i.Opcao == 0);

            bool tipo1Foi = false;
            string sourceIconeTipo1 = null;
            string textoTipo1 = null;

            string sourceIconeTipo2 = null;
            string textoTipo2 = null;
            foreach (var tipo in tipos)
            {
                if (!tipo1Foi)
                {
                    tipo1Foi = true;
                    sourceIconeTipo1 = tipo.Identificador.Replace("`", "").Replace("(", "").Replace(")", "").Replace("çã", "ca").Replace("/", "").ToLower() + ".png";
                    textoTipo1 = tipo.NomeExibicao;
                }
                else
                {
                    sourceIconeTipo2 = tipo.Identificador.Replace("`", "").Replace("(", "").Replace(")", "").Replace("çã", "ca").Replace("/", "").ToLower() + ".png";
                    textoTipo2 = tipo.NomeExibicao;
                }
            }

            Device.BeginInvokeOnMainThread(() =>
            {
                imIconeTipo1.Source = sourceIconeTipo1;
                lbTipo1.Text = textoTipo1;

                if (textoTipo2 != null)
                {
                    frTipo2.IsVisible = true;
                    imIconeTipo2.Source = sourceIconeTipo2;
                    lbTipo2.Text = textoTipo2;
                }

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
                    if (ItemAtual.QuantidadeEstrelas > 0) { estrela1.Source = "estrela_selecionada.png"; }
                    if (ItemAtual.QuantidadeEstrelas > 1) { estrela2.Source = "estrela_selecionada.png"; }
                    if (ItemAtual.QuantidadeEstrelas > 2) { estrela3.Source = "estrela_selecionada.png"; }
                    if (ItemAtual.QuantidadeEstrelas > 3) { estrela4.Source = "estrela_selecionada.png"; }
                    if (ItemAtual.QuantidadeEstrelas > 4) { estrela5.Source = "estrela_selecionada.png"; }
                }

                //cvTipo.Content = new TipoEstabelecimentoView(ItemAtual.Identificadores);
            });
        }

        private void CalcularDistancia()
        {
            double distancia = ItemAtual.DistanciaDoUsuario;
            if (distancia > 0)
            {
                double d = distancia > 1000 ? Math.Round(distancia / 1000, 2) : Math.Round(distancia, 2);
                string unidade = distancia > 1000 ? "km" : "m";

                Device.BeginInvokeOnMainThread(() =>
                {
                    lbDistancia.Text = d + unidade + " de distância";
                });
            }
        }

        public async void Compartilhar(object sender, EventArgs e)
        {
            if (ItemAtual != null)
            {
                await Plugin.Share.CrossShare.Current.Share(new Plugin.Share.Abstractions.ShareMessage { Text = ItemAtual.Nome, Url = ItemAtual.UrlExterna }, new Plugin.Share.Abstractions.ShareOptions { ChooserTitle = "Selecione" });
            }
        }

        private async void AbrirDirecoes(object sender, EventArgs e)
        {
            var success = await CrossExternalMaps.Current.NavigateTo(ItemAtual.Nome, ItemAtual.Latitude.Value, ItemAtual.Longitude.Value);
        }
    }
}