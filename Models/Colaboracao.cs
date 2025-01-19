using SQLite;

namespace MaCamp.Models
{
    [Table("Colaboracao")]
    public class Colaboracao
    {
        [PrimaryKey]
        public int IDColaborador { get; set; }

        public string? Nome { get; set; }
        public string? Email { get; set; }
        public string? WhatsApp { get; set; }

        [Ignore]
        public string? Informacao { get; set; }

        [Ignore]
        public int IDCamping { get; set; }

        [Ignore]
        public string? Camping { get; set; }

        [Ignore]
        public string? ValorPagoDiaria { get; set; }

        public string? Equipamento { get; set; }
    }
}