using Aspbrasil.Dependencias;
using Aspbrasil.UWP.Dependencias;
using SQLite;
using System.IO;
using Windows.Storage;
using Xamarin.Forms;

[assembly: Dependency(typeof(SQLite_UWP))]
namespace Aspbrasil.UWP.Dependencias
{
    class SQLite_UWP : ISQLite
    {
        public SQLiteAsyncConnection ObterConexao()
        {
            string sqliteFilename = "app.db3";
            string path = Path.Combine(ApplicationData.Current.LocalFolder.Path, sqliteFilename);
            // Create the connection
            var conn = new SQLiteAsyncConnection(path);
            // Return the database connection
            return conn;
        }
    }
}