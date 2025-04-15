using SQLite;
using static MaCamp.Utils.Enumeradores;

namespace MaCamp.Models
{
    public class ItemIdentificador
    {
        [PrimaryKey]
        public int Id { get; set; }

        [Indexed]
        public int IdItem { get; set; }

        public string? NomeExibicao { get; set; }

        [Indexed]
        public string? Identificador { get; set; }

        public string? Secao { get; set; }
        public string? NomeExibicaoOpcao { get; set; }
        public int Opcao { get; set; }
        public TipoIdentificador? TipoIdentificador { get; set; }
    }
}