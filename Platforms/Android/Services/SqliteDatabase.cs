﻿using MaCamp.Dependencias;
using SQLite;

namespace MaCamp.Platforms.Android.Services
{
    public class SqliteDatabase : ISQLite
    {
        public SQLiteConnection ObterConexao(string filename)
        {
            //var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var documentsPath = FileSystem.AppDataDirectory;
            var path = Path.Combine(documentsPath, filename);
            var connection = new SQLiteConnection(path);

            return connection;
        }
    }
}