using MaCamp.Dependencias;
using SQLite;

namespace MaCamp.Platforms.iOS.Services
{
    public class SqliteDatabase : ISQLite
    {
        public SQLiteConnection ObterConexao(string filename)
        {
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var libraryPath = Path.Combine(documentsPath, "..", "Library");
            var path = Path.Combine(libraryPath, filename);
            var connection = new SQLiteConnection(path);

            return connection;
        }
    }
}