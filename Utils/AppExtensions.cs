using System.Collections.ObjectModel;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using MaCamp.Models;
using Maui.GoogleMaps;
using Map = Maui.GoogleMaps.Map;

namespace MaCamp.Utils
{
    public static class AppExtensions
    {
        public static Dictionary<string, string> ToDictionary(this object? source)
        {
            if (source != null)
            {
                var listProperties = source.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList();
                var listPropertiesValues = listProperties.Where(x => x.CanRead && x.GetValue(source) != null).ToList();
                var dictionary = listPropertiesValues.ToDictionary(x => x.Name, x => x.GetValue(source)?.ToString() ?? string.Empty);

                return dictionary;
            }

            return new Dictionary<string, string>();
        }

        public static void ForEach<T>(this IEnumerable<T>? source, Action<T>? action)
        {
            if (source != null && action != null)
            {
                foreach (var item in source)
                {
                    action(item);
                }
            }
        }

        public static async Task ForEachAsync<T>(this IEnumerable<T>? source, Func<T, Task>? action)
        {
            if (source != null && action != null)
            {
                foreach (var item in source)
                {
                    await action(item);
                }
            }
        }

        public static void SetQueryAttributable<T>(this T value, Dictionary<string, object>? parameters = default) where T : BindableObject
        {
            if (parameters != null)
            {
                if (value is IQueryAttributable attributable)
                {
                    attributable.ApplyQueryAttributes(parameters);
                }
                else if (value.BindingContext is IQueryAttributable pageAttributable)
                {
                    pageAttributable.ApplyQueryAttributes(parameters);
                }
            }
        }

        public static string GetHexadecimalValue(this Color color)
        {
            var red = Convert.ToInt32(color.Red * 255);
            var green = Convert.ToInt32(color.Green * 255);
            var blue = Convert.ToInt32(color.Blue * 255);
            var alpha = Convert.ToInt32(color.Alpha * 255);
            var hex = $"#{alpha:X2}{red:X2}{green:X2}{blue:X2}";

            return hex;
        }

        public static string Truncate(this string value, int maxSize)
        {
            if (string.IsNullOrEmpty(value) || maxSize <= 0)
            {
                return string.Empty;
            }

            if (value.Length > maxSize)
            {
                return value.Substring(0, maxSize) + "...";
            }

            return value;
        }

        public static void MoveMapToRegion(this Map map, List<Position>? listPositions)
        {
            if (listPositions == null || listPositions.Count == 0)
            {
                return;
            }

            // Encontra os limites da lista de localizações
            var minLatitude = listPositions.Min(x => x.Latitude);
            var maxLatitude = listPositions.Max(x => x.Latitude);
            var minLongitude = listPositions.Min(x => x.Longitude);
            var maxLongitude = listPositions.Max(x => x.Longitude);

            // Calcula o centro do mapa
            var centerLatitude = (minLatitude + maxLatitude) / 2;
            var centerLongitude = (minLongitude + maxLongitude) / 2;

            // Calcula a distância em graus para a visualização
            var latitudeDegrees = maxLatitude - minLatitude;
            var longitudeDegrees = maxLongitude - minLongitude;

            // 1 grau = ~111 km
            var distance = Distance.FromKilometers(Math.Max(latitudeDegrees, longitudeDegrees) * 111);

            // Define um MapSpan com uma margem extra para a visualização
            var region = MapSpan.FromCenterAndRadius(new Position(centerLatitude, centerLongitude), distance);

            // Move o mapa para a região calculada
            map.MoveToRegion(region);
        }

        public static Task ColorTo(this VisualElement self, Color fromColor, Color toColor, Action<Color> callback, uint duration)
        {
            var animation = new Animation(x =>
            {
                var red = Convert.ToSingle(fromColor.Red + (toColor.Red - fromColor.Red) * x);
                var green = Convert.ToSingle(fromColor.Green + (toColor.Green - fromColor.Green) * x);
                var blue = Convert.ToSingle(fromColor.Blue + (toColor.Blue - fromColor.Blue) * x);
                var alpha = Convert.ToSingle(fromColor.Alpha + (toColor.Alpha - fromColor.Alpha) * x);

                callback(new Color(red, green, blue, alpha));
            });

            return Task.Run(() => self.Animate("ColorTo", animation, 16, duration, Easing.Linear));
        }

        public static string RemoveDiacritics(this string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            var stringBuilder = new StringBuilder();
            var textNormalized = text.Trim().Normalize(NormalizationForm.FormD);

            foreach (var caracter in textNormalized)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(caracter);

                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(caracter);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        public static T GetInstance<T>() where T : new()
        {
            var key = typeof(T).Name;

            if (AppConstants.DictionaryData.TryGetValue(key, out var value) && value is T instance)
            {
                return instance;
            }

            return new T();
        }

        public static void ReplaceOrAdd<T>(this ObservableCollection<T> collection, T newItem, Func<T, bool> predicate)
        {
            var existingItem = collection.FirstOrDefault(predicate);

            if (existingItem != null)
            {
                var index = collection.IndexOf(existingItem);

                collection[index] = newItem;
            }
            else
            {
                collection.Add(newItem);
            }
        }

        public static Position GetPosition(this Item? item)
        {
            if (item != null && item.Latitude != null && item.Longitude != null)
            {
                return new Position(item.Latitude.Value, item.Longitude.Value);
            }

            return default;
        }

        public static Position GetPosition(this Location? location)
        {
            if (location != null)
            {
                return new Position(location.Latitude, location.Longitude);
            }

            return default;
        }

        public static string NormalizeText(this string value)
        {
            var text = value.ToLowerInvariant();
            var normalized = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalized)
            {
                var uc = CharUnicodeInfo.GetUnicodeCategory(c);

                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            var mounted = stringBuilder.ToString().Normalize(NormalizationForm.FormC);
            var cleared = Regex.Replace(mounted, @"\s+", "_");
            var filtered = Regex.Replace(cleared, @"[^a-z0-9_]", string.Empty);

            return filtered;
        }

        public static object? Execute(this object obj, string methodName, Type[] genericTypes, params object[] parameters)
        {
            try
            {
                var objType = obj.GetType();
                var method = objType.GetMethods().FirstOrDefault(x => x.Name == methodName && x.IsGenericMethod && x.GetGenericArguments().Length == genericTypes.Length);

                if (method == null)
                {
                    throw new InvalidOperationException($"Método '{methodName}' não encontrado ou com assinatura inválida.");
                }

                var genericMethod = method.MakeGenericMethod(genericTypes);

                return genericMethod.Invoke(obj, parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine($@"Erro inesperado: {ex.Message}");

                return null;
            }
        }

        public static bool IsValidLocation(this Item item)
        {
            return item.Latitude != null && item.Latitude != 0 && item.Longitude != null && item.Longitude != 0;
        }
    }
}