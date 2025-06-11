using System.Collections.ObjectModel;
using MaCamp.Models;
using MaCamp.Models.Anuncios;
using MaCamp.Services;
using static MaCamp.Utils.Enumeradores;

namespace MaCamp.ViewModels
{
    public class ListagemInfinitaViewModel
    {
        public ObservableCollection<Item> Itens { get; set; }

        public ListagemInfinitaViewModel()
        {
            Itens = new ObservableCollection<Item>();
        }

        public async Task CarregarAsync(string endpoint, int pagina, string? tag = null, string? query = null, TipoListagem tipoListagem = TipoListagem.Noticias, bool utilizarFiltros = true)
        {
            var countAdMob = 14;
            var countAnuncio = 0;
            var random = new Random();
            var configs = default(ConfiguracoesAnuncios?);

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                configs = await ConfiguracoesAnunciosServices.GetAsync(pagina == 1);

                if (configs != null)
                {
                    countAnuncio = configs.QuantidadeCardsListagem;
                }
            }

            if (tipoListagem == TipoListagem.Camping)
            {
                var listaItensCampings = await CampingServices.CarregarCampingsAsync(utilizarFiltros);
                var listaAnuncios = await AnunciosServices.GetListAsync(pagina == 1);
                var anuncios = listaAnuncios.Where(x => x.Tipo == TipoAnuncio.Nativo).ToList();
                var identificadorPadrao = Enum.GetValues<TipoIdentificador>().Max() + 1;
                var listaItensCampingsOrdenados = listaItensCampings.OrderBy(x => x.Identificadores.Min(y => y.TipoIdentificador) ?? identificadorPadrao).ThenBy(x => x.Nome).ToList();
                var idLocal = Itens.Count;

                foreach (var itemCamping in listaItensCampingsOrdenados)
                {
                    itemCamping.IdLocal = ++idLocal;

                    Itens.Add(itemCamping);

                    // Insere Anúncio Customizado
                    if (configs?.QuantidadeCardsListagem > 0 && countAnuncio == 1 && anuncios?.Count > 0)
                    {
                        var anuncioEscolhido = anuncios[random.Next(anuncios.Count)];

                        if (!string.IsNullOrEmpty(anuncioEscolhido.UrlExterna))
                        {
                            Itens.Add(new Item
                            {
                                IdLocal = ++idLocal,
                                EhAnuncio = true,
                                DeveAbrirExternamente = true,
                                UrlExterna = anuncioEscolhido.UrlExterna,
                                Anuncio = anuncioEscolhido
                            });
                        }

                        countAnuncio = configs.QuantidadeCardsListagem;
                    }
                    else
                    {
                        countAnuncio--;
                    }

                    // Insere Anúncio AdMob
                    if (countAdMob == 1)
                    {
                        Itens.Add(new Item
                        {
                            IdLocal = ++idLocal,
                            EhAdMobRetangulo = true,
                            DeveAbrirExternamente = true,
                            UrlExterna = string.Empty
                        });

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
                var listItens = await new WebService().GetListAsync<Item>(endpoint, pagina, tag, query);
                var listaAnuncios = new List<Anuncio>();
                var idLocal = Itens.Count;

                if (configs != null && pagina == 1)
                {
                    var resultadoAnuncios = await AnunciosServices.GetListAsync(true);

                    listaAnuncios = resultadoAnuncios.Where(a => a.Tipo == TipoAnuncio.Nativo).ToList();
                }

                foreach (var item in listItens)
                {
                    var savedItem = StorageHelper.GetItemById(item.IdPost);

                    if (savedItem != null)
                    {
                        item.Visualizado = savedItem.Visualizado;
                        item.Favoritado = savedItem.Favoritado;
                    }

                    item.IdLocal = ++idLocal;

                    Itens.Add(item);

                    // Insere anúncio nativo
                    if (configs != null && countAnuncio == 1 && listaAnuncios.Any())
                    {
                        var anuncioEscolhido = listaAnuncios[random.Next(listaAnuncios.Count)];

                        if (!string.IsNullOrEmpty(anuncioEscolhido.UrlExterna))
                        {
                            Itens.Add(new Item
                            {
                                IdLocal = ++idLocal,
                                EhAnuncio = true,
                                DeveAbrirExternamente = true,
                                UrlExterna = anuncioEscolhido.UrlExterna,
                                Anuncio = anuncioEscolhido
                            });
                        }

                        countAnuncio = configs.QuantidadeCardsListagem;
                    }
                    else
                    {
                        countAnuncio--;
                    }

                    // Insere anúncio AdMob
                    if (countAdMob == 1)
                    {
                        Itens.Add(new Item
                        {
                            IdLocal = ++idLocal,
                            EhAdMobRetangulo = true,
                            DeveAbrirExternamente = true,
                            UrlExterna = string.Empty
                        });

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