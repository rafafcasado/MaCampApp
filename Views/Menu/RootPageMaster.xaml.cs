using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MaCamp.AppSettings;

namespace MaCamp.Views.Menu
{
    public partial class RootPageMaster : ContentPage
    {
        public ListView ListView { get; set; }

        public RootPageMaster()
        {
            InitializeComponent();

            BindingContext = new RootPageMasterViewModel();
            MenuItemsListView.ItemTemplate = new MenuTemplateSelector();
            ListView = MenuItemsListView;
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