using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Maui.Maps;
using Map = Microsoft.Maui.Controls.Maps.Map;

namespace MaCamp.AppSettings
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
            var red = (int)(color.Red * 255);
            var green = (int)(color.Green * 255);
            var blue = (int)(color.Blue * 255);
            var alpha = (int)(color.Alpha * 255);
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
            var minLatitude = locations.Min(loc => loc.Latitude);
            var maxLatitude = locations.Max(loc => loc.Latitude);
            var minLongitude = locations.Min(loc => loc.Longitude);
            var maxLongitude = locations.Max(loc => loc.Longitude);

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
            var animation = new Animation(v =>
            {
                var red = Convert.ToSingle(fromColor.Red + (toColor.Red - fromColor.Red) * v);
                var green = Convert.ToSingle(fromColor.Green + (toColor.Green - fromColor.Green) * v);
                var blue = Convert.ToSingle(fromColor.Blue + (toColor.Blue - fromColor.Blue) * v);
                var alpha = Convert.ToSingle(fromColor.Alpha + (toColor.Alpha - fromColor.Alpha) * v);

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

            var stFormD = text.Normalize(NormalizationForm.FormD);
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

            if (keyValuePair.TryGetValue(current, out var value))
            {
                return value;
            }

            return defaultValue;
        }
    }
}