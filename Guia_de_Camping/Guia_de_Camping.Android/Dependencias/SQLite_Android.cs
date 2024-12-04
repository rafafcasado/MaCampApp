using Aspbrasil.Dependencias;
using Aspbrasil.Droid.Dependencias;
using System.IO;
using SQLite;
using Xamarin.Forms;

[assembly: Dependency(typeof(SQLite_Android))]
namespace Aspbrasil.Droid.Dependencias
{
    public class SQLite_Android : ISQLite
    {
        public SQLiteConnection ObterConexao()
        {
            string sqliteFilename = "app.db3";
            string documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal); // Documents folder
            var path = Path.Combine(documentsPath, sqliteFilename);
            // Create the connection
            var conn = new SQLiteConnection(path);
            // Return the database connection
            return conn;
        }
    }
}