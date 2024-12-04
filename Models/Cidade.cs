using SQLite;

namespace MaCamp.Models
{
    public class Cidade
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }

        public string? Nome { get; set; }
        public string? Estado { get; set; }
    }
}