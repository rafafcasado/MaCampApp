﻿using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using MaCamp.Models.Anuncios;
using MaCamp.Utils;
using SQLite;

namespace MaCamp.Models
{
    public class Item : INotifyPropertyChanged
    {
        public int id;
        public int idlocal;
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


        public int IdLocal
        {
            get => idlocal;
            set
            {
                idlocal = value;
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
                    return string.Empty;
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
        //    get { return (!string.IsNullOrEmpty(color_tag)) ? color_tag : "#000"; }
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
        //        if (string.IsNullOrEmpty(visualizacoes))
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

        [Ignore]
        public List<ItemIdentificador> Identificadores
        {
            get
            {
                try
                {
                    if (!string.IsNullOrEmpty(IdentificadoresSerializado))
                    {
                        return JsonSerializer.Deserialize<List<ItemIdentificador>>(IdentificadoresSerializado) ?? new List<ItemIdentificador>();
                    }
                }
                catch (Exception ex)
                {
                    Workaround.ShowExceptionOnlyDevolpmentMode(nameof(Item), nameof(Identificadores), ex);
                }

                return new List<ItemIdentificador>();
            }
            set => IdentificadoresSerializado = JsonSerializer.Serialize(value);
        }

        [JsonIgnore]
        public string? IdentificadoresSerializado { get; set; }

        #endregion

        public bool EhAdMobRetangulo { get; set; } = false;
        public bool EhAnuncio { get; set; } = false;

        [Ignore]
        public Anuncio? Anuncio { get; set; }

        public override string? ToString()
        {
            return Nome;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class Video : INotifyPropertyChanged
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
                if (string.IsNullOrEmpty(views))
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

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}