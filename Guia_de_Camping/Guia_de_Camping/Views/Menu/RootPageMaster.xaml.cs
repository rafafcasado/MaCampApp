﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Aspbrasil.Views.Menu
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RootPageMaster : ContentPage
    {
        public ListView ListView;

        public RootPageMaster()
        {
            InitializeComponent();

            BindingContext = new RootPageMasterViewModel();
            MenuItemsListView.ItemTemplate = new MenuTemplateSelector();
            ListView = MenuItemsListView;
        }

        private void ExibirAlerta(string titulo, string mensagem, string botaoOk = "OK") { Device.BeginInvokeOnMainThread(async () => await DisplayAlert(titulo, mensagem, botaoOk)); }

        private async void AbrirPatrocinador(object sender, EventArgs e)
        {
            await Launcher.OpenAsync("http://www.easytransport.com.br/");
        }

        class RootPageMasterViewModel : INotifyPropertyChanged
        {
            public ObservableCollection<ItemMenu> MenuItems { get; set; }

            public RootPageMasterViewModel()
            {
                MenuItems = new ObservableCollection<ItemMenu>(new[]
                {
                    new ItemMenu { Titulo = "Home",  TipoLayout = TipoLayoutMenu.Item, TipoAcao = TipoAcaoMenu.Home, TituloPagina = "Guia de Campings" },
                    new ItemMenu { TipoLayout = TipoLayoutMenu.Divisoria },
                    new ItemMenu { Titulo = "Mapa de campings e Pontos de Apoio",  TipoLayout = TipoLayoutMenu.Item, TipoAcao = TipoAcaoMenu.AbrirMapa },
                    new ItemMenu { Titulo = "Busca de campings e Apoios RVs",  TipoLayout = TipoLayoutMenu.Item, TipoAcao = TipoAcaoMenu.AbrirBuscaCamping },
                    new ItemMenu { Titulo = "Favoritos",  TipoLayout = TipoLayoutMenu.Item, TipoAcao = TipoAcaoMenu.Favoritos },
                    new ItemMenu { TipoLayout = TipoLayoutMenu.Divisoria },
                    new ItemMenu { Titulo = "Notícias e novidades",  TipoLayout = TipoLayoutMenu.Item, TipoAcao = TipoAcaoMenu.AbrirNoticias },
                    new ItemMenu { Titulo = "Dicas de campismo e caravanismo",  TipoLayout = TipoLayoutMenu.Item, TipoAcao = TipoAcaoMenu.AbrirDicasCampismo, TituloPagina = "Dicas de campismo" },
                    new ItemMenu { Titulo = "Eventos, encontros e grupos",  TipoLayout = TipoLayoutMenu.Item, TipoAcao = TipoAcaoMenu.AbrirEventos },
                    new ItemMenu { TipoLayout = TipoLayoutMenu.Divisoria },
                    new ItemMenu { Titulo = "Cadastre um camping",  TipoLayout = TipoLayoutMenu.Item, TipoAcao = TipoAcaoMenu.CadastreUmCamping },
                    new ItemMenu { TipoLayout = TipoLayoutMenu.Divisoria },
                    new ItemMenu { Titulo = "Nossos parceiros",  TipoLayout = TipoLayoutMenu.Item, TipoAcao = TipoAcaoMenu.AbrirParceiros, TituloPagina = "Nossos parceiros" },
                    new ItemMenu { Titulo = "Sobre o MaCamp",  TipoLayout = TipoLayoutMenu.Item, TipoAcao = TipoAcaoMenu.AbrirSobreAEmpresa },
                    new ItemMenu { TipoLayout = TipoLayoutMenu.Divisoria },
                    new ItemMenu { Titulo = "Atualizar listagem de campings",  TipoLayout = TipoLayoutMenu.Item, TipoAcao = TipoAcaoMenu.AtualizarCampings },
                });
            }

            #region INotifyPropertyChanged Implementation
            public event PropertyChangedEventHandler PropertyChanged;
            void OnPropertyChanged([CallerMemberName] string propertyName = "")
            {
                if (PropertyChanged == null)
                    return;

                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            #endregion
        }

    }
}