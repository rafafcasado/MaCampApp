using System.Text.Json.Serialization;
using static MaCamp.Utils.Enumeradores;

namespace MaCamp.Models.Anuncios
{
    public class Anuncio
    {
        [JsonPropertyName("Titulo")]
        public string? Titulo { get; set; }

        [JsonPropertyName("Subtitulo")]
        public string? SubTitulo { get; set; }

        [JsonPropertyName("URLImagem")]
        public string? UrlImagem { get; set; }

        [JsonPropertyName("URLExterna")]
        public string? UrlExterna { get; set; }

        [JsonPropertyName("Tipo")]
        public TipoAnuncio Tipo { get; set; }
    }
}