using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Text.Json;
using DynamicData;
using MaCamp.Models;
using MaCamp.Utils;

namespace MaCamp.Services.DataAccess
{
    public static class StorageHelper
    {
        public static ObservableCollection<Item> ListFavorites { get; set; }

        private static string FilePath { get; }
        private static JsonSerializerOptions JsonSerializerOptions { get; }

        static StorageHelper()
        {
            FilePath = GetStoragePath();
            JsonSerializerOptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            var listFavorites = LoadData<List<Item>>(AppConstants.FavoritesFilename);

            ListFavorites = listFavorites != null ? new ObservableCollection<Item>(listFavorites) : new ObservableCollection<Item>();

            ListFavorites.CollectionChanged += ListFavorites_CollectionChanged;
        }

        public static void AddOrUpdateItem(Item item) => ListFavorites.ReplaceOrAdd(item, x => x.Id == item.id);

        public static Item? GetItemById(int id) => ListFavorites.FirstOrDefault(x => x.Id == id);

        public static bool IsFavoriteItem(int id) => ListFavorites.Any(x => x.Id == id);

        private static string GetStoragePath()
        {
            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                return Path.Combine("/storage/emulated/0/Documents", "MaCamp");
            }

            if (DeviceInfo.Platform == DevicePlatform.iOS)
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "..", "Library", "MaCamp");
            }

            if (DeviceInfo.Platform == DevicePlatform.WinUI)
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "MaCamp");
            }

            if (DeviceInfo.Platform == DevicePlatform.macOS)
            {
                return Path.Combine("Users/Shared/MaCamp");
            }

            return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        }

        private static void SaveData<T>(T data, string fileName)
        {
            try
            {
                if (!Directory.Exists(FilePath))
                {
                    Directory.CreateDirectory(FilePath);
                }

                var fullPath = Path.Combine(FilePath, fileName);
                var json = JsonSerializer.Serialize(data, JsonSerializerOptions);

                File.WriteAllText(fullPath, json);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Erro ao salvar arquivo: " + ex.Message);
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
                Debug.WriteLine("Erro ao carregar arquivo: " + ex.Message);
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
