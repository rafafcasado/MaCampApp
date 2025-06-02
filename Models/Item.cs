using System.Text.Json;
using System.Text.Json.Serialization;
using MaCamp.Models.Anuncios;
using MaCamp.Utils;
using SQLite;

namespace MaCamp.Models
{
    public class Item
    {
        [PrimaryKey]
        [AutoIncrement]
        [JsonPropertyName("id")]
        public int Id { get; set; }

        public int IdLocal { get; set; }

        [JsonPropertyName("title")]
        public string? Titulo { get; set; }

        [JsonPropertyName("subtitle")]
        public string? Subtitulo { get; set; }

        [JsonPropertyName("pubdate")]
        public DateTime? DataPublicacao { get; set; }

        [Ignore]
        [JsonIgnore]
        public string TextoDataPublicacao => DataPublicacao != null ? DataPublicacao.Value.ToString("dd 'de' MMMM 'de' yyyy") : string.Empty;

        [JsonPropertyName("image")]
        public string? URLImagem { get; set; }

        [JsonPropertyName("image_larger")]
        public string? URLImagemMaior { get; set; }

        [JsonPropertyName("type")]
        public string Tipo { get; set; }

        [JsonPropertyName("color_tag")]
        public string? CorTag { get; set; }

        [JsonPropertyName("tag")]
        public string? Tag { get; set; }

        [JsonPropertyName("url")]
        public string? UrlSite { get; set; }

        public bool Visualizado { get; set; }

        public bool Favoritado { get; set; }

        [JsonPropertyName("urlExterna")]
        public string? UrlExterna { get; set; }

        public bool DeveAbrirExternamente { get; set; }

        [Indexed]
        [JsonPropertyName("IdPost")]
        public int IdPost { get; set; }

        [Indexed]
        [JsonPropertyName("IdCamping")]
        public int IdCamping { get; set; }

        [Indexed]
        [JsonPropertyName("Nome")]
        public string? Nome { get; set; }

        [JsonPropertyName("Descricao")]
        public string? Descricao { get; set; }

        [JsonPropertyName("Endereco")]
        public string? Endereco { get; set; }

        [JsonPropertyName("Cidade")]
        public string? Cidade { get; set; }

        [JsonPropertyName("Estado")]
        public string? Estado { get; set; }

        [JsonPropertyName("Pais")]
        public string? Pais { get; set; }

        [JsonPropertyName("UF")]
        public string? UF { get; set; }

        [Ignore]
        [JsonIgnore]
        public string CidadeEstado => Cidade + "/" + UF;

        [Ignore]
        [JsonIgnore]
        public string EnderecoCompleto => Endereco + ". " + Cidade + "/" + UF;

        [JsonPropertyName("Telefone")]
        public string? Telefone1 { get; set; }

        [JsonPropertyName("Telefone2")]
        public string? Telefone2 { get; set; }

        [JsonPropertyName("Telefone3")]
        public string? Telefone3 { get; set; }

        [JsonPropertyName("Telefone4")]
        public string? Telefone4 { get; set; }

        [JsonPropertyName("Funcionamento")]
        public string? Funcionamento { get; set; }

        [JsonPropertyName("Site")]
        public string? Site { get; set; }

        [JsonPropertyName("Facebook")]
        public string? Facebook { get; set; }

        [JsonPropertyName("Instagram")]
        public string? Instagram { get; set; }

        [JsonPropertyName("Youtube")]
        public string? Youtube { get; set; }

        [JsonPropertyName("LinkPrecos")]
        public string? LinkPrecos { get; set; }

        [JsonPropertyName("Email")]
        public string? Email { get; set; }

        [JsonPropertyName("QuantidadeEstrelas")]
        public int? QuantidadeEstrelas { get; set; }

        [JsonPropertyName("Latitude")]
        public double? Latitude { get; set; }

        [JsonPropertyName("Longitude")]
        public double? Longitude { get; set; }

        [Ignore]
        [JsonIgnore]
        public string LatitudeLongitude => Latitude.ToString()?.Replace(",", ".") + "," + Longitude.ToString()?.Replace(",", ".");

        [JsonPropertyName("LinksFotos")]
        public string? LinksFotos { get; set; }

        [JsonPropertyName("Ordem")]
        public int Ordem { get; set; }

        [JsonPropertyName("texturl")]
        public string? LinkTexto { get; set; }

        [JsonPropertyName("visualizacoes")]
        public string? Visualizacoes { get; set; }

        [JsonPropertyName("hasVideo")]
        public bool PossuiVideo { get; set; }

        [Ignore]
        [JsonIgnore]
        public string? LinkUltimaFoto => LinksFotos?.Split('|').LastOrDefault();

        [Ignore]
        [JsonPropertyName("Identificadores")]
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
        private string? IdentificadoresSerializado { get; set; }

        [Ignore]
        [JsonIgnore]
        public bool EhAdMobRetangulo { get; set; }

        [Ignore]
        [JsonIgnore]
        public bool EhAnuncio { get; set; }

        [JsonPropertyName("corporate")]
        public bool EhCorporativo { get; set; }

        [Ignore]
        [JsonIgnore]
        public Anuncio? Anuncio { get; set; }

        public override string? ToString() => Nome;
    }
}