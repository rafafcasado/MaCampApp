using SQLite;

namespace Aspbrasil.Models
{
    public class ChaveValor
    {
        [PrimaryKey]
        public string Chave { get; set; }
        public string Valor { get; set; }
        public TipoChave Tipo { get; set; }

        public ChaveValor()
        {

        }

        public ChaveValor(string chave, string valor, TipoChave tipo)
        {
            Chave = chave;
            Valor = valor;
            Tipo = tipo;
        }
    }

    public enum TipoChave
    {
        ChaveUsuario,
        ControleInterno,
        Cor
    }
}