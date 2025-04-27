using System.Collections.ObjectModel;
using MaCamp.Views.Menu;
using static MaCamp.Utils.Enumeradores;

namespace MaCamp.ViewModels
{
    public class MasterPageViewModel
    {
        public ObservableCollection<ItemMenu> MenuItems { get; set; }

        public MasterPageViewModel()
        {
            MenuItems = new ObservableCollection<ItemMenu>(new[]
            {
                new ItemMenu
                {
                    Titulo = "Home",
                    TipoLayout = TipoLayoutMenu.Item,
                    TipoAcao = TipoAcaoMenu.Home,
                    TituloPagina = "Guia de Campings"
                },
                new ItemMenu
                {
                    TipoLayout = TipoLayoutMenu.Divisoria
                },
                new ItemMenu
                {
                    Titulo = "Mapa de campings e Pontos de Apoio",
                    TipoLayout = TipoLayoutMenu.Item,
                    TipoAcao = TipoAcaoMenu.AbrirMapa
                },
                new ItemMenu
                {
                    Titulo = "Busca de campings e Apoios RVs",
                    TipoLayout = TipoLayoutMenu.Item,
                    TipoAcao = TipoAcaoMenu.AbrirBuscaCamping
                },
                new ItemMenu
                {
                    Titulo = "Favoritos",
                    TipoLayout = TipoLayoutMenu.Item,
                    TipoAcao = TipoAcaoMenu.Favoritos
                },
                new ItemMenu
                {
                    TipoLayout = TipoLayoutMenu.Divisoria
                },
                new ItemMenu
                {
                    Titulo = "Notícias e novidades",
                    TipoLayout = TipoLayoutMenu.Item,
                    TipoAcao = TipoAcaoMenu.AbrirNoticias
                },
                new ItemMenu
                {
                    Titulo = "Dicas de campismo e caravanismo",
                    TipoLayout = TipoLayoutMenu.Item,
                    TipoAcao = TipoAcaoMenu.AbrirDicasCampismo,
                    TituloPagina = "Dicas de campismo"
                },
                new ItemMenu
                {
                    Titulo = "Eventos, encontros e grupos",
                    TipoLayout = TipoLayoutMenu.Item,
                    TipoAcao = TipoAcaoMenu.AbrirEventos
                },
                new ItemMenu
                {
                    Titulo = "Classificados",
                    TipoLayout = TipoLayoutMenu.Item,
                    TipoAcao = TipoAcaoMenu.AbrirClassificados
                },
                new ItemMenu
                {
                    TipoLayout = TipoLayoutMenu.Divisoria
                },
                new ItemMenu
                {
                    Titulo = "Cadastre um camping",
                    TipoLayout = TipoLayoutMenu.Item,
                    TipoAcao = TipoAcaoMenu.CadastreUmCamping
                },
                new ItemMenu
                {
                    TipoLayout = TipoLayoutMenu.Divisoria
                },
                new ItemMenu
                {
                    Titulo = "Nossos parceiros",
                    TipoLayout = TipoLayoutMenu.Item,
                    TipoAcao = TipoAcaoMenu.AbrirParceiros,
                    TituloPagina = "Nossos parceiros"
                },
                new ItemMenu
                {
                    Titulo = "Sobre o MaCamp",
                    TipoLayout = TipoLayoutMenu.Item,
                    TipoAcao = TipoAcaoMenu.AbrirSobreAEmpresa
                },
                new ItemMenu
                {
                    TipoLayout = TipoLayoutMenu.Divisoria
                },
                new ItemMenu
                {
                    Titulo = "Atualizar listagem de campings",
                    TipoLayout = TipoLayoutMenu.Item,
                    TipoAcao = TipoAcaoMenu.AtualizarCampings
                }
            });
        }
    }
}
