using System.Collections.ObjectModel;
using MaCamp.AppSettings;
using MaCamp.Models;
using MaCamp.Models.Anuncios;
using MaCamp.Models.DataAccess;
using MaCamp.Models.Services;

namespace MaCamp.ViewModels
{
    public class ListagemInfinitaVM
    {
        public DBContract DB { get; set; }
        public ObservableCollection<Item> Itens { get; set; }
        private WebService<Item> WebService { get; set; }

        public ListagemInfinitaVM()
        {
            DB = DBContract.Instance;
            Itens = new ObservableCollection<Item>();
            WebService = new WebService<Item>();
        }

        public async Task Carregar(string endpoint, int pagina, string tag, string query = "", TipoListagem tipoListagem = TipoListagem.Noticias, bool utilizarFiltros = true)
        {
            var configs = default(ConfiguracoesAnuncios?);
            var countAnuncio = 0;

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                configs = await ConfiguracoesAnunciosDA.ObterConfigs(pagina == 1);

                if (configs != null)
                {
                    countAnuncio = configs.QuantidadeCardsListagem;
                }
            }

            var countAdMob = 14;
            var r = new Random();

            if (tipoListagem == TipoListagem.Camping)
            {
                var listaItensCampings = await ObterListaDeCampings(endpoint, pagina, tag, query, utilizarFiltros);
                var idLocal = Itens.Count;
                var anuncios = (await AnuncioDA.ObterAnuncios(pagina == 1)).Where(a => a.Tipo == TipoAnuncio.Nativo).ToList();

                listaItensCampings.ForEach(item =>
                {
                    item.IdLocal = ++idLocal;
                    Itens.Add(item);

                    if (configs != null)
                    {
                        if (countAnuncio == 1 && anuncios.Count > 0)
                        {
                            var anuncioEscolhido = anuncios[r.Next(anuncios.Count)];

                            if (anuncioEscolhido.UrlExterna != null)
                            {
                                Itens.Add(new Item
                                {
                                    DeveAbrirExternamente = true, UrlExterna = anuncioEscolhido.UrlExterna,
                                    IdLocal = idLocal++, EhAnuncio = true, Anuncio = anuncioEscolhido
                                });
                            }

                            countAnuncio = configs.QuantidadeCardsListagem;
                        }
                        else
                        {
                            countAnuncio--;
                        }
                    }

                    if (countAdMob == 1)
                    {
                        Itens.Add(new Item
                        {
                            DeveAbrirExternamente = true, UrlExterna = "", IdLocal = idLocal++, EhAdMobRetangulo = true
                        });

                        countAdMob = 14;
                    }
                    else
                    {
                        countAdMob--;
                    }
                });
            }
            else
            {
                var listItens = await WebService.Get(endpoint, pagina, tag, query);
                var idLocal = Itens.Count;

                await listItens.ForEachAsync(async item =>
                {
                    var itemBD = DB.ObterItem(i => i.IdPost == item.IdPost);

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
                            var listaAnuncios = await AnuncioDA.ObterAnuncios(pagina == 1);
                            var anuncios = listaAnuncios.Where(a => a.Tipo == TipoAnuncio.Nativo).ToList();

                            if (anuncios.Count > 0)
                            {
                                var anuncioEscolhido = anuncios[r.Next(anuncios.Count)];

                                if (anuncioEscolhido.UrlExterna != null)
                                {
                                    Itens.Add(new Item
                                    {
                                        DeveAbrirExternamente = true,
                                        UrlExterna = anuncioEscolhido.UrlExterna,
                                        IdLocal = idLocal++,
                                        EhAnuncio = true,
                                        Anuncio = anuncioEscolhido
                                    });
                                }
                            }

                            countAnuncio = configs.QuantidadeCardsListagem;
                        }
                        else
                        {
                            countAnuncio--;
                        }

                        if (countAdMob == 1)
                        {
                            Itens.Add(new Item
                            {
                                DeveAbrirExternamente = true, UrlExterna = "", IdLocal = idLocal++,
                                EhAdMobRetangulo = true
                            });

                            countAdMob = 14;
                        }
                        else
                        {
                            countAdMob--;
                        }
                    }
                });
            }
        }

        private async Task<List<Item>> ObterListaDeCampings(string endpoint, int pagina, string tag, string query, bool filtrar = true)
        {
            var campings = await CampingServices.CarregarCampings(filtrar);

            return campings;
        }

        private double CalcularDistancia(double latitudeItem, double longitudeItem)
        {
            if (App.LOCALIZACAO_USUARIO != null)
            {
                var constant = 0.0174532925199433;
                var latitude = App.LOCALIZACAO_USUARIO.Latitude * constant;
                var longitude = App.LOCALIZACAO_USUARIO.Longitude * constant;
                var num = latitudeItem * constant;
                var longitude1 = longitudeItem * constant;
                var num1 = longitude1 - longitude;
                var num2 = num - latitude;
                var num3 = Math.Pow(Math.Sin(num2 / 2), 2) + Math.Cos(latitude) * Math.Cos(num) * Math.Pow(Math.Sin(num1 / 2), 2);
                var num4 = 2 * Math.Atan2(Math.Sqrt(num3), Math.Sqrt(1 - num3));
                var num5 = 6376500 * num4;

                return num5;
            }

            return 0;
        }
    }
}