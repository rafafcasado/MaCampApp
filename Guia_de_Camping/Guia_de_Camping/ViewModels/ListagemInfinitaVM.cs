using Aspbrasil.DataAccess;
using Aspbrasil.Models;
using Aspbrasil.Models.DataAccess;
using Aspbrasil.Models.Services;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Aspbrasil.ViewModels
{
    public class ListagemInfinitaVM
    {
        public DBContract DB = DBContract.NewInstance();
        public ObservableCollection<Item> Itens = new ObservableCollection<Item>();
        private WebService<Item> _ws = new WebService<Item>();

        public async Task Carregar(string endpoint, int pagina, string tag, string query = "", TipoListagem tipoListagem = TipoListagem.Noticias, bool utilizarFiltros = true)
        {
            ConfiguracoesAnuncios configs = null;
            int countAnuncio = 0;
            if (CrossConnectivity.Current.IsConnected)
            {
                configs = await ConfiguracoesAnunciosDA.ObterConfigs(forcarAtualizacao: pagina == 1);
                countAnuncio = configs.QuantidadeCardsListagem;
            }

            int countAdMob = 14;

            List<Item> itens = new List<Item>();
            Random r = new Random();

            if (tipoListagem == TipoListagem.Camping)
            {
                itens = await ObterListaDeCampings(endpoint, pagina, tag, query, utilizarFiltros);
                int idLocal = Itens.Count;
                List<Anuncio> anuncios = (await AnuncioDA.ObterAnuncios(forcarAtualizacao: pagina == 1)).Where(a => a.Tipo == TipoAnuncio.Nativo).ToList();

                foreach (var item in itens)
                {
                    item.IdLocal = ++idLocal;
                    Itens.Add(item);

                    if (configs != null)
                    {
                        if (countAnuncio == 1 && anuncios.Count > 0)
                        {
                            Anuncio anuncioEscolhido = anuncios[r.Next(anuncios.Count)];
                            if (anuncioEscolhido != null)
                            {
                                Itens.Add(new Item { DeveAbrirExternamente = true, UrlExterna = anuncioEscolhido.URLExterna, IdLocal = idLocal++, EhAnuncio = true, Anuncio = anuncioEscolhido });
                            }
                            countAnuncio = configs.QuantidadeCardsListagem;
                        }
                        else { countAnuncio--; }
                    }

                    if (countAdMob == 1)
                    {
                        Itens.Add(new Item { DeveAbrirExternamente = true, UrlExterna = "", IdLocal = idLocal++, EhAdMobRetangulo = true });
                        countAdMob = 14;
                    }
                    else
                    {
                        countAdMob--;
                    }
                }
            }
            else
            {
                itens = await _ws.Get(endpoint, pagina, tag, query);

                int idLocal = Itens.Count;
                if (itens != null)
                {
                    Item itemBD = null;


                    foreach (var item in itens)
                    {
                        itemBD = DB.ObterItem(i => i.IdPost == item.IdPost);
                        if (itemBD != null)
                        {
                            item.Visualizado = itemBD.Visualizado;
                            item.Favoritado = itemBD.Favoritado;
                        }

                        item.IdLocal = ++idLocal;
                        Itens.Add(item);

                        if (configs != null)
                        {
                            if (countAnuncio == 1)
                            {
                                List<Anuncio> anuncios = (await AnuncioDA.ObterAnuncios(forcarAtualizacao: pagina == 1)).Where(a => a.Tipo == TipoAnuncio.Nativo).ToList();
                                if (anuncios.Count > 0)
                                {
                                    Anuncio anuncioEscolhido = anuncios[r.Next(anuncios.Count)];
                                    if (anuncioEscolhido != null)
                                    {
                                        Itens.Add(new Item { DeveAbrirExternamente = true, UrlExterna = anuncioEscolhido.URLExterna, IdLocal = idLocal++, EhAnuncio = true, Anuncio = anuncioEscolhido });
                                    }
                                }
                                countAnuncio = configs.QuantidadeCardsListagem;
                            }
                            else { countAnuncio--; }

                            if (countAdMob == 1)
                            {
                                Itens.Add(new Item { DeveAbrirExternamente = true, UrlExterna = "", IdLocal = idLocal++, EhAdMobRetangulo = true });
                                countAdMob = 14;
                            }
                            else
                            {
                                countAdMob--;
                            }
                        }
                    }
                }
            }
        }

        private async Task<List<Item>> ObterListaDeCampings(string endpoint, int pagina, string tag, string query, bool filtrar = true)
        {
            List<Item> campings = await CampingServices.CarregarCampings(filtrar);
            return campings;
        }

        private double CalcularDistancia(double latitudeItem, double longitudeItem)
        {
            double latitude = App.LOCALIZACAO_USUARIO.Latitude * 0.0174532925199433;
            double longitude = App.LOCALIZACAO_USUARIO.Longitude * 0.0174532925199433;
            double num = latitudeItem * 0.0174532925199433;
            double longitude1 = longitudeItem * 0.0174532925199433;
            double num1 = longitude1 - longitude;
            double num2 = num - latitude;
            double num3 = Math.Pow(Math.Sin(num2 / 2), 2) + Math.Cos(latitude) * Math.Cos(num) * Math.Pow(Math.Sin(num1 / 2), 2);
            double num4 = 2 * Math.Atan2(Math.Sqrt(num3), Math.Sqrt(1 - num3));
            double num5 = 6376500 * num4;

            return num5;
        }
    }
}
