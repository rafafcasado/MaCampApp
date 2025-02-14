using MaCamp.Models.Anuncios;
using MaCamp.Services.DataAccess;
using MaCamp.ViewModels;
using SQLite;

namespace MaCamp.Models
{
    public class Item : ObservableModel
    {

        public int id;
        public string? title;
        public string? subtitle;
        public DateTime? pubdate;
        public string? image;
        public string? image_larger;
        public string? type;
        public string? tag;

        //public string color_tag;
        //public bool corporate;
        public string? url;
        public string? texturl;
        public string? visualizacoes;
        public bool hasVideo;
        public string? urlExterna;

        //public Video video;
        public bool foiVisualizado;
        public bool foiFavoritada;

        [PrimaryKey]
        [AutoIncrement]
        public int Id
        {
            get => id;
            set
            {
                id = value;
                OnPropertyChanged();
            }
        }

        public string? Titulo
        {
            get => title;
            set
            {
                title = value;
                OnPropertyChanged();
            }
        }

        public string? Subtitulo
        {
            get => subtitle;
            set
            {
                subtitle = value;
                OnPropertyChanged();
            }
        }

        public DateTime? DataPublicacao
        {
            get => pubdate;
            set
            {
                pubdate = value;
                OnPropertyChanged();
            }
        }

        public string TextoDataPublicacao
        {
            get
            {
                if (pubdate == null)
                {
                    return "";
                }

                return pubdate.Value.ToString("dd 'de' MMMM 'de' yyyy");

                //TimeSpan tempoDesdeAPublicacao = DateTime.Now - pubdate.Value;

                //if (tempoDesdeAPublicacao.TotalMinutes <= 3)
                //{
                //    return "Agora";
                //}
                //else if (tempoDesdeAPublicacao.TotalMinutes <= 59)
                //{
                //    string plural = ((int)tempoDesdeAPublicacao.TotalMinutes) > 1 ? "s" : string.Empty;
                //    return $"Há {(int)tempoDesdeAPublicacao.TotalMinutes} minuto{plural}";
                //}
                //else if (tempoDesdeAPublicacao.TotalHours < 24)
                //{
                //    string plural = ((int)tempoDesdeAPublicacao.TotalHours) > 1 ? "s" : string.Empty;
                //    return $"Há {(int)tempoDesdeAPublicacao.TotalHours} hora{plural}";
                //}
                //else if (pubdate.Value.Day == DateTime.Now.Day - 1)
                //{
                //    return $"Ontem";
                //}
                //else if (pubdate.Value.Day == DateTime.Now.Day - 2)
                //{
                //    return $"Há 2 dias";
                //}
                //else if (pubdate.Value.Day == DateTime.Now.Day - 3)
                //{
                //    return $"Há 3 dias";
                //}
                //else
                //{
                //    return pubdate.Value.ToString("dd/MM/yyyy");
                //}
            }
        }

        public string? URLImagem
        {
            get => image;
            set
            {
                image = value;
                OnPropertyChanged();
            }
        }

        public string? URLImagemMaior
        {
            get => image_larger;
            set
            {
                image_larger = value;
                OnPropertyChanged();
            }
        }

        public string? Tag
        {
            get => tag;
            set
            {
                tag = value;
                OnPropertyChanged();
            }
        }

        //public string ColorTag
        //{
        //    get { return (!string.IsNullOrWhiteSpace(color_tag)) ? color_tag : "#000"; }
        //    set { color_tag = value; OnPropertyChanged(); }
        //}

        public string? Tipo
        {
            get => type;
            set
            {
                type = value;
                OnPropertyChanged();
            }
        }

        //public bool IsCorporate
        //{
        //    get { return corporate; }
        //    set { corporate = value; OnPropertyChanged(); }
        //}
        public string? UrlSite
        {
            get => url;
            set
            {
                url = value;
                OnPropertyChanged();
            }
        }

        public string? LinkTexto
        {
            get => texturl;
            set
            {
                texturl = value;
                OnPropertyChanged();
            }
        }

        //public string Visualizacoes
        //{
        //    get
        //    {
        //        if (string.IsNullOrWhiteSpace(visualizacoes))
        //        {
        //             return string.Empty
        //        }

        //        var qtd = Convert.ToInt32(visualizacoes);
        //        if (qtd < 1000) return qtd.ToString();
        //        var exp = Convert.ToInt32(Math.Log(qtd) / Math.Log(1000));
        //        return $"{(qtd / Math.Pow(1000, exp)).ToString("#.#")} {"kMGTPE"[exp - 1]}";
        //    }
        //    set { visualizacoes = value; OnPropertyChanged(); }
        //}

        public bool PossuiVideo
        {
            get => hasVideo;
            set
            {
                hasVideo = value;
                OnPropertyChanged();
            }
        }

        //[Ignore]
        //public Video Video
        //{
        //    get { return video; }
        //    set { video = value; OnPropertyChanged(); }
        //}
        public bool Visualizado
        {
            get => foiVisualizado;
            set
            {
                foiVisualizado = value;
                OnPropertyChanged();
            }
        }

        public bool Favoritado
        {
            get => foiFavoritada;
            set
            {
                foiFavoritada = value;
                OnPropertyChanged();
            }
        }

        public string? UrlExterna
        {
            get => urlExterna;
            set => urlExterna = value;
        }

        public bool DeveAbrirExternamente { get; set; } = false;

        [Indexed]
        public int IdPost { get; set; }

        #region Infos Camping

        [Indexed]
        public int IdCamping { get; set; }

        [Indexed]
        public string? Nome { get; set; }

        public string? Descricao { get; set; }
        public string? Endereco { get; set; }
        public string? Cidade { get; set; }
        public string? Estado { get; set; }
        public string? Pais { get; set; }
        public string? UF { get; set; }

        [Ignore]
        public string CidadeEstado => Cidade + "/" + UF;

        [Ignore]
        public string EnderecoCompleto => Endereco + ". " + Cidade + "/" + UF;

        public string? Telefone { get; set; }
        public string? Telefone2 { get; set; }
        public string? Telefone3 { get; set; }
        public string? Telefone4 { get; set; }
        public string? Funcionamento { get; set; }
        public string? Site { get; set; }
        public string? Facebook { get; set; }
        public string? Instagram { get; set; }
        public string? Youtube { get; set; }
        public string? LinkPrecos { get; set; }
        public string? Email { get; set; }
        public int? QuantidadeEstrelas { get; set; } = 0;
        public double? Latitude { get; set; } = 0;
        public double? Longitude { get; set; } = 0;
        public string LatitudeLongitude => Latitude.ToString()?.Replace(",", ".") + "," + Longitude.ToString()?.Replace(",", ".");
        public int Ordem { get; set; }
        public string? LinksFotos { get; set; }
        public string? LinkUltimaFoto => LinksFotos?.Split('|').LastOrDefault();
        private List<ItemIdentificador> _identificadores = new List<ItemIdentificador>();

        [Ignore]
        public List<ItemIdentificador> Identificadores
        {
            get
            {
                if (_identificadores.Count == 0)
                {
                    _identificadores = DBContract.ListarItensIdentificadores(i => i.IdItem == IdCamping);
                }

                return _identificadores;
            }
            set => _identificadores = value;
        }

        #endregion

        public double DistanciaDoUsuario
        {
            get
            {
                if (App.LOCALIZACAO_USUARIO == null || Latitude == null || Longitude == null || (Latitude == 0 && Longitude == 0))
                {
                    return -1;
                }

                var latitude = App.LOCALIZACAO_USUARIO.Latitude * 0.0174532925199433;
                var longitude = App.LOCALIZACAO_USUARIO.Longitude * 0.0174532925199433;
                var num = Latitude.Value * 0.0174532925199433;
                var longitude1 = Longitude.Value * 0.0174532925199433;
                var num1 = longitude1 - longitude;
                var num2 = num - latitude;
                var num3 = Math.Pow(Math.Sin(num2 / 2), 2) + Math.Cos(latitude) * Math.Cos(num) * Math.Pow(Math.Sin(num1 / 2), 2);
                var num4 = 2 * Math.Atan2(Math.Sqrt(num3), Math.Sqrt(1 - num3));
                var num5 = 6376500 * num4;

                return num5;
            }
        }

        public bool EhAdMobRetangulo { get; set; } = false;
        public bool EhAnuncio { get; set; } = false;

        [Ignore]
        public Anuncio? Anuncio { get; set; }

        public override string? ToString()
        {
            return Nome;
        }
    }

    public class Video : ObservableModel
    {
        public string? id;
        public string? views;
        public string? url;

        public string? IdVideo
        {
            get => id;
            set
            {
                id = value;
                OnPropertyChanged();
            }
        }

        public string VisualizacoesVideo
        {
            get
            {
                if (string.IsNullOrWhiteSpace(views))
                {
                    return string.Empty;
                }

                var qtd = Convert.ToInt32(views);

                if (qtd < 1000)
                {
                    return qtd.ToString();
                }

                var exp = Convert.ToInt32(Math.Log(qtd) / Math.Log(1000));

                return $"{(qtd / Math.Pow(1000, exp)).ToString("#.#").Replace(".", ",")}{"kMGTPE"[exp - 1]}";
            }
            set
            {
                views = value;
                OnPropertyChanged();
            }
        }

        public string? UrlVideo
        {
            get => url;
            set
            {
                url = value;
                OnPropertyChanged();
            }
        }

        [PrimaryKey]
        public string? IdItem { get; set; }
    }
}