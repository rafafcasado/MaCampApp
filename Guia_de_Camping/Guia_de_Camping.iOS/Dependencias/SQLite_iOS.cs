using System;
using Xamarin.Forms;
using System.IO;
using Aspbrasil.Dependencias;
using SQLite;

[assembly: Dependency(typeof(Aspbrasil.iOS.SQLite_iOS))]
namespace Aspbrasil.iOS
{
    public class SQLite_iOS : ISQLite
    {
        public SQLite_iOS()
        {
        }

        public SQLiteConnection ObterConexao()
        {
            var sqliteFileName = "app.db3";
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); // Documents folder
            string libraryPath = Path.Combine(documentsPath, "..", "Library"); // Library folder
            var path = Path.Combine(libraryPath, sqliteFileName);
            // Create the connection
            var conn = new SQLiteConnection(path);
            // Return the database connection
            return conn;
        }
    }
}