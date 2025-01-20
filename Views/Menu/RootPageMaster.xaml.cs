using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MaCamp.Utils;

namespace MaCamp.Views.Menu
{
    public partial class RootPageMaster : ContentPage
    {
        public CollectionView CollectionView { get; set; }

        public RootPageMaster()
        {
            InitializeComponent();

            BindingContext = new RootPageMasterViewModel();
            MenuItemsListView.ItemTemplate = new MenuTemplateSelector();
            CollectionView = MenuItemsListView;
        }

        private async void ExibirAlerta(string titulo, string mensagem, string botaoOk = "OK")
        {
            await Dispatcher.DispatchAsync(async () => await DisplayAlert(titulo, mensagem, botaoOk));
        }

        private void AbrirPatrocinador(object sender, EventArgs e)
        {
            Launcher.OpenAsync(AppConstants.Url_EasyTransports);
        }

        private class RootPageMasterViewModel : INotifyPropertyChanged
        {
            public ObservableCollection<ItemMenu> MenuItems { get; set; }

            public RootPageMasterViewModel()
            {
                MenuItems = new ObservableCollection<ItemMenu>(new[]
                {
                    new ItemMenu
                    {
                        Titulo = "Home",
                        TipoLayout = Enumeradores.TipoLayoutMenu.Item,
                        TipoAcao = Enumeradores.TipoAcaoMenu.Home,
                        TituloPagina = "Guia de Campings"
                    },
                    new ItemMenu
                    {
                        TipoLayout = Enumeradores.TipoLayoutMenu.Divisoria
                    },
                    new ItemMenu
                    {
                        Titulo = "Mapa de campings e Pontos de Apoio",
                        TipoLayout = Enumeradores.TipoLayoutMenu.Item,
                        TipoAcao = Enumeradores.TipoAcaoMenu.AbrirMapa
                    },
                    new ItemMenu
                    {
                        Titulo = "Busca de campings e Apoios RVs",
                        TipoLayout = Enumeradores.TipoLayoutMenu.Item,
                        TipoAcao = Enumeradores.TipoAcaoMenu.AbrirBuscaCamping
                    },
                    new ItemMenu
                    {
                        Titulo = "Favoritos",
                        TipoLayout = Enumeradores.TipoLayoutMenu.Item,
                        TipoAcao = Enumeradores.TipoAcaoMenu.Favoritos
                    },
                    new ItemMenu
                    {
                        TipoLayout = Enumeradores.TipoLayoutMenu.Divisoria
                    },
                    new ItemMenu
                    {
                        Titulo = "Notícias e novidades",
                        TipoLayout = Enumeradores.TipoLayoutMenu.Item,
                        TipoAcao = Enumeradores.TipoAcaoMenu.AbrirNoticias
                    },
                    new ItemMenu
                    {
                        Titulo = "Dicas de campismo e caravanismo",
                        TipoLayout = Enumeradores.TipoLayoutMenu.Item,
                        TipoAcao = Enumeradores.TipoAcaoMenu.AbrirDicasCampismo,
                        TituloPagina = "Dicas de campismo"
                    },
                    new ItemMenu
                    {
                        Titulo = "Eventos, encontros e grupos",
                        TipoLayout = Enumeradores.TipoLayoutMenu.Item,
                        TipoAcao = Enumeradores.TipoAcaoMenu.AbrirEventos
                    },
                    new ItemMenu
                    {
                        TipoLayout = Enumeradores.TipoLayoutMenu.Divisoria
                    },
                    new ItemMenu
                    {
                        Titulo = "Cadastre um camping",
                        TipoLayout = Enumeradores.TipoLayoutMenu.Item,
                        TipoAcao = Enumeradores.TipoAcaoMenu.CadastreUmCamping
                    },
                    new ItemMenu
                    {
                        TipoLayout = Enumeradores.TipoLayoutMenu.Divisoria
                    },
                    new ItemMenu
                    {
                        Titulo = "Nossos parceiros",
                        TipoLayout = Enumeradores.TipoLayoutMenu.Item,
                        TipoAcao = Enumeradores.TipoAcaoMenu.AbrirParceiros,
                        TituloPagina = "Nossos parceiros"
                    },
                    new ItemMenu
                    {
                        Titulo = "Sobre o MaCamp",
                        TipoLayout = Enumeradores.TipoLayoutMenu.Item,
                        TipoAcao = Enumeradores.TipoAcaoMenu.AbrirSobreAEmpresa
                    },
                    new ItemMenu
                    {
                        TipoLayout = Enumeradores.TipoLayoutMenu.Divisoria
                    },
                    new ItemMenu
                    {
                        Titulo = "Atualizar listagem de campings",
                        TipoLayout = Enumeradores.TipoLayoutMenu.Item,
                        TipoAcao = Enumeradores.TipoAcaoMenu.AtualizarCampings
                    }
                });
            }

            public event PropertyChangedEventHandler? PropertyChanged;

            private void OnPropertyChanged([CallerMemberName] string propertyName = "")
            {
                if (PropertyChanged == null)
                {
                    return;
                }

                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}