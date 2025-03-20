using SQLite;
using static MaCamp.Utils.Enumeradores;

namespace MaCamp.Models
{
    public class ChaveValor
    {
        [PrimaryKey]
        public string Chave { get; set; }

        public string? Valor { get; set; }
        public TipoChave Tipo { get; set; }

        public ChaveValor()
        {
            Chave = null!;
        }

        public ChaveValor(string chave, string valor, TipoChave tipo)
        {
            Chave = chave;
            Valor = valor;
            Tipo = tipo;
        }
    }
}