using SQLite;

namespace Aspbrasil.Dependencias
{
    public interface ISQLite
    {
        SQLiteConnection ObterConexao();
    }
}