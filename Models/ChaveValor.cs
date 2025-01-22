using MaCamp.Utils;
using SQLite;

namespace MaCamp.Models
{
    public class ChaveValor
    {
        [PrimaryKey]
        public string Chave { get; set; }

        public string? Valor { get; set; }
        public Enumeradores.TipoChave Tipo { get; set; }

        public ChaveValor()
        {
            Chave = null!;
        }

        public ChaveValor(string chave, string valor, Enumeradores.TipoChave tipo)
        {
            Chave = chave;
            Valor = valor;
            Tipo = tipo;
        }
    }
}