using SQLite;

namespace MaCamp.Dependencias
{
    public interface ISQLite
    {
        SQLiteConnection? ObterConexao(string filename);
    }
}