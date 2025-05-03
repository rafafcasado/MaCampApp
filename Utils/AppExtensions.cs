using System.Collections.ObjectModel;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using MaCamp.Models;
using MPowerKit.GoogleMaps;

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

        public static string ToHexadecimal(this Color color)
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

        public static Location? GetLocation(this Item? item)
        {
            if (item != null && item.Latitude != null && item.Longitude != null)
            {
                return new Location(item.Latitude.Value, item.Longitude.Value);
            }

            return default;
        }

        public static Point GetPosition(this Item? item)
        {
            if (item != null && item.Latitude != null && item.Longitude != null)
            {
                return new Point(item.Latitude.Value, item.Longitude.Value);
            }

            return default;
        }

        public static Point GetPosition(this Location? location)
        {
            if (location != null)
            {
                return new Point(location.Latitude, location.Longitude);
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

        public static bool IsValidLocation(this Item? item)
        {
            return item != null && item.Latitude != null && item.Latitude != 0 && item.Longitude != null && item.Longitude != 0;
        }

        public static bool IsValid(this Location? location)
        {
            return location != null && location.Latitude != 0 && location.Longitude != 0;
        }

        /// <summary>
        /// Adiciona uma coleção de itens em lotes, de forma assíncrona, evitando travar a UI.
        /// </summary>
        /// <typeparam name="T">Tipo da coleção (deve implementar ICollection&lt;TItem&gt;)</typeparam>
        /// <typeparam name="TK">Tipo dos itens a serem adicionados</typeparam>
        /// <param name="collection">Coleção de destino</param>
        /// <param name="items">Itens a serem adicionados</param>
        /// <param name="batchSize">Tamanho do lote (padrão: 100)</param>
        /// <param name="delayMilliseconds">Tempo de espera entre lotes (padrão: 50ms)</param>
        /// <param name="cancellationToken">Token de cancelamento opcional</param>
        public static async Task AddRangeAsync<T, TK>(this IList<TK> collection, IEnumerable<TK> items, int batchSize = 100, int delayMilliseconds = 50, CancellationToken cancellationToken = default)
        {
            var itemList = items.ToList();

            for (var i = 0; i < itemList.Count; i += batchSize)
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    var batch = itemList.Skip(i).Take(batchSize).ToList();

                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        foreach (var item in batch)
                        {
                            collection.Add(item);
                        }
                    });

                    if (i + batchSize < itemList.Count)
                    {
                        await Task.Delay(delayMilliseconds, cancellationToken);
                    }
                }
            }
        }

        /// <summary>
        /// Adiciona uma coleção de itens em lotes, de forma assíncrona, evitando travar a UI.
        /// </summary>
        /// <typeparam name="T">Tipo da coleção (deve implementar ICollection&lt;TItem&gt;)</typeparam>
        /// <typeparam name="TK">Tipo dos itens a serem adicionados</typeparam>
        /// <param name="collection">Coleção de destino</param>
        /// <param name="items">Itens a serem adicionados</param>
        /// <param name="cancellationToken">Token de cancelamento opcional</param>
        public static async Task AddRangeAsync<T, TK>(this IList<TK> collection, IEnumerable<TK> items, CancellationToken cancellationToken = default)
        {
            var batchSize = Environment.ProcessorCount * 100;
            var delayMilliseconds = AppConstants.Delay / Environment.ProcessorCount;

            await AddRangeAsync<T, TK>(collection, items, batchSize, delayMilliseconds, cancellationToken);
        }

        public static double? GetDistanceKilometersFromUser(this Item? item)
        {
            if (item.IsValidLocation() && App.LOCALIZACAO_USUARIO.IsValid())
            {
                var itemPosition = item.GetPosition();
                var userPosition = App.LOCALIZACAO_USUARIO.GetPosition();
                var distanceKilometers = Location.CalculateDistance(itemPosition.X, itemPosition.Y, userPosition.X, userPosition.Y, DistanceUnits.Kilometers);

                return distanceKilometers;
            }

            return default;
        }

        public static string ToGeoJson(this IEnumerable<Pin> source)
        {
            var listFeatures = source.Select(pin => new
            {
                type = "Feature",
                geometry = new
                {
                    type = "Point",
                    coordinates = new[]
                    {
                        pin.Position.Y,
                        pin.Position.X
                    }
                },
                properties = new
                {
                    id = pin.ClassId,
                    title = pin.Title,
                    snippet = pin.Snippet,
                    icon = pin.Icon.GetResourceValue()
                }
            }).ToList();
            var geoJson = new
            {
                type = "FeatureCollection",
                features = listFeatures
            };
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            return JsonSerializer.Serialize(geoJson, options);
        }

        public static bool TryGetValue<T>(this JsonNode? jsonNode, string key, out T value)
        {
            if (jsonNode != null)
            {
                var data = jsonNode[key];

                if (data != null)
                {
                    value = data is T response ? response : data.GetValue<T>();

                    return true;
                }
            }

            value = default!;

            return false;
        }

        public static bool TryGetValue<T>(this JsonArray? jsonArray, int index, out T value)
        {
            if (jsonArray != null)
            {
                var element = jsonArray.ElementAtOrDefault(index);

                if (element is JsonNode jsonNode)
                {
                    value = jsonNode.GetValue<T>();

                    return true;
                }
            }

            value = default!;

            return false;
        }

        public static bool TryGetValue<T, TK>(this Dictionary<T, TK> dictionary, T key, out TK value) where T : notnull
        {
            if (dictionary.TryGetValue(key, out var response) && response != null && (response is not Stream stream || stream != Stream.Null))
            {
                value = response;

                return true;
            }

            value = default!;

            return false;
        }

        public static byte[] GetManifestResourceBytes(this Assembly assembly, string name)
        {
            using var stream = assembly.GetManifestResourceStream(name);
            using var memoryStream = new MemoryStream();

            if (stream != null)
            {
                stream.CopyTo(memoryStream);

                return memoryStream.ToArray();
            }

            return Array.Empty<byte>();
        }

        /// <summary>
        /// Gera um CameraUpdate que centraliza e dá zoom de forma a exibir todos os pins.
        /// Se não houver pins, usa a posição do usuário.
        /// </summary>
        /// <param name="pins">Coleção de pins (cada pin.Position.X=latitude, Y=longitude).</param>
        /// <param name="userPosition">
        /// Posição fallback caso não haja pins (Latitude/Longitude).
        /// </param>
        public static CameraUpdate ToCameraUpdate(this List<Pin>? pins, Location? userPosition = null)
        {
            var positions = new List<Point>();

            if (pins != null && pins.Any())
            {
                positions.AddRange(pins.Select(p => p.Position));
            }
            else if (userPosition != null)
            {
                positions.Add(new Point(userPosition.Latitude, userPosition.Longitude));
            }
            else
            {
                // Se a localização do usuário não estiver disponível, use uma posição padrão (São Paulo)
                positions.Add(new Point(-23.550520, -46.633308));
            }

            var minLat = positions.Min(p => p.X);
            var maxLat = positions.Max(p => p.X);
            var minLon = positions.Min(p => p.Y);
            var maxLon = positions.Max(p => p.Y);

            var centerLat = (minLat + maxLat) / 2;
            var centerLon = (minLon + maxLon) / 2;
            var center = new Point(centerLat, centerLon);

            var maxDistMeters = positions.Max(p =>
            {
                var distKm = Location.CalculateDistance(centerLat, centerLon, p.X, p.Y, DistanceUnits.Kilometers);

                return distKm * 1000;
            });

            return CameraUpdateFactory.FromCenterAndRadius(center, maxDistMeters * 0.85);
        }

        public static string GetResourceValue(this ImageSource source)
        {
            if (source is ResourceImageSource namedStreamImageSource)
            {
                return namedStreamImageSource.Value;
            }

            throw new InvalidOperationException("O ImageSource não é um ResourceImageSource.");
        }
    }
}
