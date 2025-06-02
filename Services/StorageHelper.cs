using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text.Json;
using MaCamp.Models;
using MaCamp.Utils;

namespace MaCamp.Services
{
    public static class StorageHelper
    {
        private static string FilePath { get; }
        private static ObservableCollection<Item> ListFavorites { get; }
        private static JsonSerializerOptions JsonSerializerOptions { get; }

        static StorageHelper()
        {
            FilePath = App.PATH;
            JsonSerializerOptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            var listFavorites = LoadData<List<Item>>(AppConstants.FavoritesFilename);

            ListFavorites = listFavorites != null ? new ObservableCollection<Item>(listFavorites) : new ObservableCollection<Item>();

            ListFavorites.CollectionChanged += ListFavorites_CollectionChanged;
        }

        public static void AddOrUpdateItem(Item item) => ListFavorites.ReplaceOrAdd(item, x => x.Id == item.Id);

        public static Item? GetItemById(int id) => ListFavorites.FirstOrDefault(x => x.Id == id);

        public static bool IsFavoriteItem(int id) => ListFavorites.Any(x => x.Id == id);

        private static void SaveData<T>(T data, string fileName)
        {
            try
            {
                var fullPath = Path.Combine(FilePath, fileName);
                var json = JsonSerializer.Serialize(data, JsonSerializerOptions);

                File.WriteAllText(fullPath, json);
            }
            catch (Exception ex)
            {
                Workaround.ShowExceptionOnlyDevolpmentMode(nameof(StorageHelper), nameof(SaveData), ex);
            }
        }

        public static T? LoadData<T>(string fileName)
        {
            try
            {
                var fullPath = Path.Combine(FilePath, fileName);

                if (File.Exists(fullPath))
                {
                    var json = File.ReadAllText(fullPath);

                    return JsonSerializer.Deserialize<T>(json);
                }
            }
            catch (Exception ex)
            {
                Workaround.ShowExceptionOnlyDevolpmentMode(nameof(StorageHelper), nameof(LoadData), ex);
            }

            return default;
        }

        private static void ListFavorites_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (sender is ObservableCollection<Item> listFavorites)
            {
                SaveData(listFavorites, AppConstants.FavoritesFilename);
            }
        }
    }
}
