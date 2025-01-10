using MaCamp.AppSettings;
using MaCamp.Models;
using MaCamp.Models.DataAccess;
using StackLayout = Microsoft.Maui.Controls.StackLayout;

namespace MaCamp.Views.Campings
{
    public partial class FiltrosPage : ContentPage
    {
        //public string EstadoSelecionado { get; set; }
        //public string CidadeSelecionada { get; set; }

        //private string ParametroTODAS = " - TODAS - ";
        //private string ParametroTODOS = " - TODOS - ";
        private DBContract DB { get; }
        private List<string> EstabelecimentosSelecionados { get; }
        private List<string> ComodidadesSelecionadas { get; }
        //private bool UsarLocalizacaoUsuario { get; set; }

        public FiltrosPage(bool busca = false)
        {
            InitializeComponent();

            EstabelecimentosSelecionados = new List<string>();
            ComodidadesSelecionadas = new List<string>();

            if (busca)
            {
                Title = "Busca";
                btFiltrar.Text = "BUSCAR";
            }
            else
            {
                Title = "Filtros";
            }

            DB = DBContract.Instance;

            //Task.Run(() =>
            //{
            //    CarregarCidadesEstados();
            //    CarregarLocalizacaoUsuario();
            //});
            CarregarFiltrosEstabelecimentoSelecionados();
            CarregarFiltrosServicosSelecionados();

            //btBuscarPorCidadeEstado.Clicked += (s, e) =>
            //{
            //    AlterarBuscaLocalizacao(true);
            //};

            //Plugin.GoogleAnalytics.GoogleAnalytics.Current.Tracker.SendView("Filtro Estabelecimentos e Serviços");
        }

        //private void AlterarBuscaLocalizacao(bool usarCidadeEstado)
        //{
        //    Dispatcher.Dispatch(() =>
        //    {
        //        if (usarCidadeEstado)
        //        {
        //            slBuscaUFCidade.IsVisible = true;
        //            UsarLocalizacaoUsuario = false;
        //            lbUsandoLocalizacao.IsVisible = false;
        //            slUsarMinhaLocalizacao.IsVisible = true;
        //            btBuscarPorCidadeEstado.IsVisible = false;
        //        }
        //        else
        //        {
        //            slBuscaUFCidade.IsVisible = false;
        //            UsarLocalizacaoUsuario = true;
        //            lbUsandoLocalizacao.IsVisible = true;
        //            slUsarMinhaLocalizacao.IsVisible = false;
        //            btBuscarPorCidadeEstado.IsVisible = true;
        //        }
        //    });
        //}

        //private void CarregarLocalizacaoUsuario()
        //{
        //    string valorChaveUsarLocalizacaoUsuario = DB.ObterValorChave("FILTROS_LOCALIZACAO_SELECIONADA");
        //    if (valorChaveUsarLocalizacaoUsuario != null && Convert.ToBoolean(valorChaveUsarLocalizacaoUsuario))
        //    {
        //        UsarLocalizacaoUsuario = true;
        //        AlterarBuscaLocalizacao(false);
        //    }
        //}

        //private async void CarregarCidadesEstados()
        //{
        //    string EstadoBD = DB.ObterValorChave("FILTROS_ESTADO_SELECIONADO");
        //    string CIDADE_BD = DB.ObterValorChave("FILTROS_CIDADE_SELECIONADA");
        //    List<Cidade> cidadesWS = DB.ListarCidades();

        //    if (cidadesWS.Count == 0)
        //    {
        //        using (var client = new HttpClient())
        //        {
        //            string jsonCidades = await client.GetStringAsync(AppConstants.Url_ListaCidades);
        //            cidadesWS = JsonConvert.DeserializeObject<List<Cidade>>(jsonCidades).Where(x => !x.Estado.Contains("_")).ToList();
        //            DB.InserirListaDeModelo(cidadesWS);
        //        }
        //    }

        //    var gruposCidadePorUF = cidadesWS.GroupBy(c => c.Estado);

        //    List<string> estados = new List<string>();
        //    estados.Add(ParametroTODOS);
        //    foreach (var grupoEstado in gruposCidadePorUF)
        //    {
        //        estados.Add(grupoEstado.Key);
        //    }

        //    Dispatcher.Dispatch(() =>
        //    {
        //        pkUF.Title = "Selecione o Estado";
        //        pkUF.ItemsSource = estados;
        //        pkUF.SelectedIndexChanged += (s, e) =>
        //        {
        //            EstadoSelecionado = ((s as Picker).SelectedItem as string);
        //            if (EstadoSelecionado == ParametroTODOS)
        //            {
        //                pkCidade.ItemsSource = null;
        //                pkCidade.Title = " - ";
        //                pkCidade.IsEnabled = false;
        //                CidadeSelecionada = string.Empty;
        //            }
        //            else
        //            {
        //                var cidadesDisponiveis = cidadesWS.Where(c => c.Estado == EstadoSelecionado).ToList();
        //                cidadesDisponiveis.Insert(0, new Cidade { Nome = ParametroTODAS, Estado = EstadoSelecionado });
        //                pkCidade.ItemsSource = cidadesDisponiveis;
        //                pkCidade.ItemDisplayBinding = new Binding(nameof(Cidade.Nome));
        //                pkCidade.Title = "Selecione a cidade";
        //                pkCidade.IsEnabled = true;

        //                pkCidade.SelectedIndexChanged += (senderCidade, eventCidade) =>
        //                {
        //                    Cidade cidadeSelecionada = ((senderCidade as Picker).SelectedItem as Cidade);
        //                    if (cidadeSelecionada != null) { CidadeSelecionada = cidadeSelecionada.Nome; }
        //                    else { CidadeSelecionada = string.Empty; }
        //                };
        //                if (CIDADE_BD != null && CIDADE_BD != null)
        //                {
        //                    pkCidade.SelectedItem = cidadesDisponiveis.Where(c => c.Nome == CIDADE_BD).FirstOrDefault();
        //                }
        //            }
        //        };
        //        pkUF.SelectedItem = EstadoBD;
        //    });
        //}

        private void CarregarFiltrosEstabelecimentoSelecionados()
        {
            var valoresFiltrosSelecionados = DB.ObterValorChave("FILTROS_ESTABELECIMENTO_SELECIONADOS");

            if (valoresFiltrosSelecionados == null)
            {
                EstabelecimentosSelecionados.Add("Campings");
                EstabelecimentosSelecionados.Add("PontodeApoioaRV`s");
                EstabelecimentosSelecionados.Add("CampingSelvagem/WildCamping/Bushcfaft");
                EstabelecimentosSelecionados.Add("SemFunçãoCamping/ApoioouFechado");
                SelecionarView(slFiltroCamping);
                SelecionarView(slFiltroApoioRVs);
                SelecionarView(slFiltroWild);
                SelecionarView(slFiltroSemFuncao);
            }
            else
            {
                foreach (var filtro in valoresFiltrosSelecionados.Split(','))
                {
                    EstabelecimentosSelecionados.Add(filtro);

                    switch (filtro)
                    {
                        case "Campings":
                            DeselecionarView(slFiltroCamping);

                            break;
                        case "PontodeApoioaRV`s":
                            DeselecionarView(slFiltroApoioRVs);

                            break;
                        case "CampingSelvagem/WildCamping/Bushcfaft":
                            DeselecionarView(slFiltroWild);

                            break;
                        case "SemFunçãoCamping/ApoioouFechado":
                            DeselecionarView(slFiltroSemFuncao);

                            break;
                    }
                }
            }
        }

        private void CarregarFiltrosServicosSelecionados()
        {
            var valoresFiltrosSelecionados = DB.ObterValorChave("FILTROS_SERVICO_SELECIONADOS");

            if (valoresFiltrosSelecionados == null)
            {
                //ComodidadesSelecionadas.Add("AceitaBarracas");
                //ComodidadesSelecionadas.Add("AceitaRVs");
                //ComodidadesSelecionadas.Add("PossuiChalesCabanasOuSuites");
                //ComodidadesSelecionadas.Add("AceitaAnimais");
                //SelecionarView(slAceitaBarracas);
                //SelecionarView(slAceitaRVs);
                //SelecionarView(slPossuiChales);
                //SelecionarView(slAceitaAnimais);
            }
            else
            {
                foreach (var filtro in valoresFiltrosSelecionados.Split(','))
                {
                    ComodidadesSelecionadas.Add(filtro);

                    switch (filtro)
                    {
                        case "AceitaBarracas":
                            DeselecionarView(slAceitaBarracas);
                            break;
                        case "AceitaRVs":
                            DeselecionarView(slAceitaRVs);
                            break;
                        case "PossuiChalesCabanasOuSuites":
                            DeselecionarView(slPossuiChales);
                            break;
                        case "AceitaAnimais":
                            DeselecionarView(slAceitaAnimais);
                            break;
                        case "Restaurante":
                            DeselecionarView(slRestaurante);
                            break;
                        case "ChurrasqueiraCozinha":
                            DeselecionarView(slChurrasqueiraCozinha);
                            break;
                        case "TelefoneInternet":
                            DeselecionarView(slTelefoneInternet);
                            break;
                        case "Piscina":
                            DeselecionarView(slPiscina);
                            break;
                        case "Acessibilidade":
                            DeselecionarView(slAcessibilidade);
                            break;
                        case "Transporte":
                            DeselecionarView(slAcessoTransporteColetivo);
                            break;
                        case "Esgoto":
                            DeselecionarView(slLocalDetritos);
                            break;
                        case "PossuiEnergiaEletrica":
                            DeselecionarView(slEnergiaEletrica);
                            break;
                        case "PossuiPraiaProxima":
                            DeselecionarView(slPossuiPraiaProxima);
                            break;
                            //case "Sanitarios":
                            //    SelecionarView(slBanheiros);
                            //    break;
                    }
                }
            }
        }

        public void FiltroEstabelecimentoTapped(object sender, EventArgs e)
        {
            if (sender is View viewFiltro)
            {
                var filtroClicado = viewFiltro.ClassId;

                if (EstabelecimentosSelecionados.Contains(filtroClicado))
                {
                    EstabelecimentosSelecionados.Remove(filtroClicado);
                    DeselecionarView(viewFiltro);
                }
                else
                {
                    EstabelecimentosSelecionados.Add(filtroClicado);
                    SelecionarView(viewFiltro);
                }
            }
        }

        public void ServicoTapped(object sender, EventArgs e)
        {
            if (sender is View viewServico)
            {
                var servicoClicado = viewServico.ClassId;

                if (ComodidadesSelecionadas.Contains(servicoClicado))
                {
                    DeselecionarView(viewServico);
                    ComodidadesSelecionadas.Remove(servicoClicado);
                }
                else
                {
                    SelecionarView(viewServico);
                    ComodidadesSelecionadas.Add(servicoClicado);
                }
            }
        }

        private void SelecionarView(View view)
        {
            view.BackgroundColor = AppColors.CorDestaque;

            if (view is StackLayout sl)
            {
                foreach (var child in sl.Children)
                {
                    if (child is Label lb)
                    {
                        lb.TextColor = Colors.White;
                    }
                }
            }
        }

        private void DeselecionarView(View view)
        {
            view.BackgroundColor = Color.FromArgb("F5F5F5");

            if (view is StackLayout sl)
            {
                foreach (var child in sl.Children)
                {
                    if (child is Label lb)
                    {
                        lb.TextColor = Colors.Gray;
                    }
                }
            }
        }

        //private async void UsarMinhaLocalizacao(object sender, EventArgs e)
        //{
        //    loader.IsVisible = loader.IsRunning = true;
        //    slUsarMinhaLocalizacao.IsVisible = false;
        //    UsarLocalizacaoUsuario = true;

        //    try
        //    {
        //        App.LOCALIZACAO_USUARIO = await Geolocation.GetLocationAsync();
        //    }
        //    catch (Exception ex) { System.Diagnostics.Debug.WriteLine(ex.Message); }

        //    loader.IsVisible = loader.IsRunning = false;
        //    AlterarBuscaLocalizacao(false);
        //}

        private async void Filtrar(object sender, EventArgs e)
        {
            //if (UsarLocalizacaoUsuario)
            //{
            //    DB.InserirOuSubstituirModelo(new ChaveValor { Chave = "FILTROS_LOCALIZACAO_SELECIONADA", Valor = "true" });
            //    DB.InserirOuSubstituirModelo(new ChaveValor { Chave = "FILTROS_ESTADO_SELECIONADO", Valor = null });
            //    DB.InserirOuSubstituirModelo(new ChaveValor { Chave = "FILTROS_CIDADE_SELECIONADA", Valor = null });

            //}
            //else
            //{
            //    DB.InserirOuSubstituirModelo(new ChaveValor { Chave = "FILTROS_ESTADO_SELECIONADO", Valor = EstadoSelecionado == ParametroTODOS || EstadoSelecionado == string.Empty ? null : EstadoSelecionado });
            //    DB.InserirOuSubstituirModelo(new ChaveValor { Chave = "FILTROS_CIDADE_SELECIONADA", Valor = CidadeSelecionada == ParametroTODAS || CidadeSelecionada == string.Empty ? null : CidadeSelecionada });
            //    DB.InserirOuSubstituirModelo(new ChaveValor { Chave = "FILTROS_LOCALIZACAO_SELECIONADA", Valor = "false" });
            //}
            EstabelecimentosSelecionados.Remove("");
            var valorEstabelecimentos = string.Join(",", EstabelecimentosSelecionados);

            DB.InserirOuSubstituirModelo(new ChaveValor
            {
                Chave = "FILTROS_ESTABELECIMENTO_SELECIONADOS", Valor = valorEstabelecimentos
            });

            ComodidadesSelecionadas.Remove("");
            var valorComodidades = string.Join(",", ComodidadesSelecionadas);

            DB.InserirOuSubstituirModelo(new ChaveValor
            {
                Chave = "FILTROS_SERVICO_SELECIONADOS", Valor = valorComodidades
            });

            //Plugin.GoogleAnalytics.GoogleAnalytics.Current.Tracker.SendEvent("Filtro Estabelecimentos e Serviços", "Filtrar", valorEstabelecimentos + " - " + valorComodidades);
            MessagingCenter.Send(Application.Current, AppConstants.MessagingCenter_BuscaRealizada);

            await Navigation.PopAsync();
        }
    }
}