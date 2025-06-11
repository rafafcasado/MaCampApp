using System.Text.Json.Serialization;
using SQLite;
using static MaCamp.Utils.Enumeradores;

namespace MaCamp.Models
{
    public class ItemIdentificador
    {
        [PrimaryKey]
        [JsonPropertyName("Id")]
        public int Id { get; set; }

        [Indexed]
        [JsonPropertyName("IdItem")]
        public int IdItem { get; set; }

        [JsonPropertyName("NomeExibicao")]
        public string? NomeExibicao { get; set; }

        [Indexed]
        [JsonPropertyName("Identificador")]
        public string? Identificador { get; set; }

        [JsonPropertyName("Secao")]
        public string? Secao { get; set; }

        [JsonPropertyName("NomeExibicaoOpcao")]
        public string? NomeExibicaoOpcao { get; set; }

        [JsonPropertyName("Opcao")]
        public int Opcao { get; set; }

        public TipoIdentificador? TipoIdentificador { get; set; }
    }
}