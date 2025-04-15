using System.Collections.ObjectModel;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using MaCamp.Dependencias;
using MaCamp.Models;
using Microsoft.Maui.Maps;
using SkiaSharp;
using Map = Microsoft.Maui.Controls.Maps.Map;

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

        public static string? ToConstantName(this string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                var listCharacters = value.Select(x => x).ToList();
                var listCustomCharacters = listCharacters.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + char.ToUpper(x) : char.ToUpper(x).ToString()).ToList();
                var constantName = string.Join(string.Empty, listCustomCharacters);

                return constantName;
            }

            return default;
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

        public static string RemoveSpecialCharacteres(this string value, bool removeSpaces = false)
        {
            var textoAlterado = string.Empty;
            var regexPattern = "\\p{P}+";

            if (removeSpaces)
            {
                regexPattern = "[\\W]";
            }

            var textoSemCaracteresEspeciais = Regex.Replace(value, regexPattern, string.Empty);

            foreach (var charactere in textoSemCaracteresEspeciais)
            {
                if (AppConstants.CharsAcentuados.Contains(charactere.ToString()))
                {
                    var specialPosition = AppConstants.CharsAcentuados.IndexOf(charactere);

                    // utiliza o caracter correspondente sem acentuação
                    textoAlterado += AppConstants.CharsRegulares[specialPosition].ToString();
                }
                else
                {
                    // Se não encontrar nenhum acento, utiliza o caracter que já está no texto
                    textoAlterado += charactere.ToString();
                }
            }

            return textoAlterado;
        }

        public static void MoveMapToRegion(this Map map, List<Location>? locations)
        {
            if (locations == null || locations.Count == 0)
            {
                return;
            }

            // Encontra os limites da lista de localizações
            var minLatitude = locations.Min(x => x.Latitude);
            var maxLatitude = locations.Max(x => x.Latitude);
            var minLongitude = locations.Min(x => x.Longitude);
            var maxLongitude = locations.Max(x => x.Longitude);

            // Calcula o centro do mapa
            var centerLatitude = (minLatitude + maxLatitude) / 2;
            var centerLongitude = (minLongitude + maxLongitude) / 2;

            // Calcula a distância em graus para a visualização
            var latitudeDegrees = maxLatitude - minLatitude;
            var longitudeDegrees = maxLongitude - minLongitude;

            // 1 grau = ~111 km
            var distance = Distance.FromKilometers(Math.Max(latitudeDegrees, longitudeDegrees) * 111);

            // Define um MapSpan com uma margem extra para a visualização
            var region = MapSpan.FromCenterAndRadius(new Location(centerLatitude, centerLongitude), distance);

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
            if (string.IsNullOrWhiteSpace(text))
            {
                return text;
            }

            var stFormD = text.Trim().Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var caracter in stFormD)
            {
                var uc = CharUnicodeInfo.GetUnicodeCategory(caracter);

                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(caracter);
                }
            }

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }

        public static T ToPlatform<T>(this T defaultValue, Dictionary<DevicePlatform, T> keyValuePair)
        {
            var current = DeviceInfo.Platform;

            return keyValuePair.GetValueOrDefault(current, defaultValue);
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

        public static T GetInstance<T>(Action<T> action) where T : new()
        {
            var instance = GetInstance<T>();

            action.Invoke(instance);

            return instance;
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

        public class RunActions
        {
            public Action? Start { get; set; }
            public Action? End { get; set; }
            public Action? Granted { get; set; }
            public Action? Denied { get; set; }
        }

        public static async Task Run(this IStoragePermission service, RunActions actions)
        {
            actions.Start?.Invoke();

            var response = await service.Request();

            if (response)
            {
                actions.Granted?.Invoke();
            }
            else
            {
                actions.Denied?.Invoke();
            }

            actions.End?.Invoke();
        }

        public static bool IsInside(this MapSpan region, Location location)
        {
            var minLat = region.Center.Latitude - (region.LatitudeDegrees / 2);
            var maxLat = region.Center.Latitude + (region.LatitudeDegrees / 2);
            var minLon = region.Center.Longitude - (region.LongitudeDegrees / 2);
            var maxLon = region.Center.Longitude + (region.LongitudeDegrees / 2);

            return location.Latitude >= minLat && location.Latitude <= maxLat && location.Longitude >= minLon && location.Longitude <= maxLon;
        }

        public static bool IsInside(this MapSpan region, double? latitude, double? longetitude)
        {
            if (latitude != null && longetitude != null)
            {
                return region.IsInside(new Location(latitude.Value, longetitude.Value));
            }

            return false;
        }

        public static Location GetLocation(this Item? item)
        {
            if (item != null && item.Latitude != null && item.Longitude != null)
            {
                return new Location(item.Latitude.Value, item.Longitude.Value);
            }

            return new Location();
        }

        public static SKColor ToSKColor(this Color color)
        {
            var red = Convert.ToByte(color.Red * 255);
            var green = Convert.ToByte(color.Green * 255);
            var blue = Convert.ToByte(color.Blue * 255);
            var alpha = Convert.ToByte(color.Alpha * 255);

            return new SKColor(red, green, blue, alpha);
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
    }
}