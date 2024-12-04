using MaCamp.AppSettings;
using MaCamp.Dependencias;
using SQLite;

namespace MaCamp.Platforms.iOS.Services
{
    public class SqliteDatabase : ISQLite
    {
        public SQLiteConnection ObterConexao()
        {
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var libraryPath = Path.Combine(documentsPath, "..", "Library");
            var path = Path.Combine(libraryPath, AppConstants.SqliteFilename);
            var connection = new SQLiteConnection(path);

            return connection;
        }
    }
}