using System.Collections.ObjectModel;
using MaCamp.Models;
using MaCamp.Models.Anuncios;
using MaCamp.Services;
using MaCamp.Utils;
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
            var countAnuncio = 0;
            var configs = default(ConfiguracoesAnuncios?);

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                configs = await ConfiguracoesAnunciosServices.GetAsync(pagina == 1);

                if (configs != null)
                {
                    countAnuncio = configs.QuantidadeCardsListagem;
                }
            }

            var countAdMob = 14;
            var random = new Random();

            if (tipoListagem == TipoListagem.Camping)
            {
                var listaItensCampings = await CampingServices.CarregarCampingsAsync(utilizarFiltros);
                var idLocal = Itens.Count;
                var listaAnuncios = await AnunciosServices.GetListAsync(pagina == 1);
                var anuncios = listaAnuncios.Where(x => x.Tipo == TipoAnuncio.Nativo).ToList();
                var listaItensCampingsOrdenados = listaItensCampings.OrderBy(x => x.Identificadores.Min(y => y.TipoIdentificador)).ThenBy(x => x.Nome).ToList();

                listaItensCampingsOrdenados.ForEach(x =>
                {
                    x.IdLocal = ++idLocal;

                    Itens.Add(x);

                    if (configs != null)
                    {
                        if (countAnuncio == 1 && anuncios.Count > 0)
                        {
                            var anuncioEscolhido = anuncios[random.Next(anuncios.Count)];

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
                            DeveAbrirExternamente = true,
                            UrlExterna = string.Empty,
                            IdLocal = idLocal++,
                            EhAdMobRetangulo = true
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
                var listItens = await new WebService().GetListAsync<Item>(endpoint, pagina, tag, query);
                var idLocal = Itens.Count;

                await listItens.ForEachAsync(async x =>
                {
                    var savedItem = StorageHelper.GetItemById(x.IdPost);

                    if (savedItem != null)
                    {
                        x.Visualizado = savedItem.Visualizado;
                        x.Favoritado = savedItem.Favoritado;
                    }

                    x.IdLocal = ++idLocal;

                    Itens.Add(x);

                    if (configs != null)
                    {
                        if (countAnuncio == 1)
                        {
                            var listaAnuncios = await AnunciosServices.GetListAsync(pagina == 1);
                            var anuncios = listaAnuncios.Where(y => y.Tipo == TipoAnuncio.Nativo).ToList();

                            if (anuncios.Count > 0)
                            {
                                var anuncioEscolhido = anuncios[random.Next(anuncios.Count)];

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
                                DeveAbrirExternamente = true,
                                UrlExterna = string.Empty,
                                IdLocal = idLocal++,
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
    }
}