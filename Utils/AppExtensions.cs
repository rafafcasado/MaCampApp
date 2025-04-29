using System.Collections.ObjectModel;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
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

        public static Position? GetCenterPosition(this IEnumerable<Position>? source)
        {
            var listPositions = source != null ? source.ToList() : new List<Position>();

            if (listPositions.Any())
            {
                // Encontra os limites da lista de localizações
                var minLatitude = listPositions.Min(x => x.Latitude);
                var maxLatitude = listPositions.Max(x => x.Latitude);
                var minLongitude = listPositions.Min(x => x.Longitude);
                var maxLongitude = listPositions.Max(x => x.Longitude);

                // Calcula o centro do mapa
                var centerLatitude = (minLatitude + maxLatitude) / 2;
                var centerLongitude = (minLongitude + maxLongitude) / 2;

                return new Position(centerLatitude, centerLongitude);
            }

            return null;
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

        public static Location? GetLocation(this Item? item)
        {
            if (item != null && item.Latitude != null && item.Longitude != null)
            {
                return new Location(item.Latitude.Value, item.Longitude.Value);
            }

            return default;
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
        /// <typeparam name="TCollection">Tipo da coleção (deve implementar ICollection&lt;TItem&gt;)</typeparam>
        /// <typeparam name="TItem">Tipo dos itens a serem adicionados</typeparam>
        /// <param name="collection">Coleção de destino</param>
        /// <param name="items">Itens a serem adicionados</param>
        /// <param name="batchSize">Tamanho do lote (padrão: 100)</param>
        /// <param name="delayMilliseconds">Tempo de espera entre lotes (padrão: 50ms)</param>
        /// <param name="cancellationToken">Token de cancelamento opcional</param>
        public static async Task AddRangeAsync<TCollection, TItem>(this TCollection collection, IEnumerable<TItem> items, int batchSize = 100, int delayMilliseconds = 50, CancellationToken cancellationToken = default) where TCollection : ICollection<TItem>
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
        /// <typeparam name="TCollection">Tipo da coleção (deve implementar ICollection&lt;TItem&gt;)</typeparam>
        /// <typeparam name="TItem">Tipo dos itens a serem adicionados</typeparam>
        /// <param name="collection">Coleção de destino</param>
        /// <param name="items">Itens a serem adicionados</param>
        /// <param name="cancellationToken">Token de cancelamento opcional</param>
        public static async Task AddRangeAsync<TCollection, TItem>(this TCollection collection, IEnumerable<TItem> items, CancellationToken cancellationToken = default) where TCollection : ICollection<TItem>
        {
            var batchSize = Environment.ProcessorCount * 100;
            var delayMilliseconds = 500 / Environment.ProcessorCount;

            await AddRangeAsync(collection, items, batchSize, delayMilliseconds, cancellationToken);
        }

        public static bool Contains(this Bounds bounds, double latitude, double longitude)
        {
            return latitude >= bounds.SouthWest.Latitude && latitude <= bounds.NorthEast.Latitude && longitude >= bounds.SouthWest.Longitude && longitude <= bounds.NorthEast.Longitude;
        }

        public static bool Contains(this MapRegion? region, Position position)
        {
            if (region != null)
            {
                var minLat = Math.Min(region.FarLeft.Latitude, region.NearLeft.Latitude);
                var maxLat = Math.Max(region.FarRight.Latitude, region.NearRight.Latitude);
                var minLon = Math.Min(region.FarLeft.Longitude, region.FarRight.Longitude);
                var maxLon = Math.Max(region.NearLeft.Longitude, region.NearRight.Longitude);
                var latitude = position.Latitude >= minLat && position.Latitude <= maxLat;
                var longitude = position.Longitude >= minLon && position.Longitude <= maxLon;

                return latitude && longitude;
            }

            return false;
        }

        public static bool Contains(this CameraPosition? camera, Position position, double visibleRadiusKm = 10)
        {
            if (camera != null)
            {
                // Distância entre centro da câmera e o ponto
                var distance = Location.CalculateDistance(camera.Target.Latitude, camera.Target.Longitude, position.Latitude, position.Longitude, DistanceUnits.Kilometers);
                // Zoom influencia o quão perto/far está visível
                // Aproximação: cada nível de zoom dobra ou divide o raio.
                // Zoom 15 → ~1km, Zoom 10 → ~10km
                var effectiveRadiusKm = Math.Pow(2, 21 - camera.Zoom) * 0.001;

                return distance <= (effectiveRadiusKm * visibleRadiusKm);
            }

            return false;
        }

        public static Bounds? ToBounds(this IEnumerable<Position>? source)
        {
            if (source != null)
            {
                var positions = source.ToList();

                if (positions.Any())
                {
                    var minLat = positions.Min(p => p.Latitude);
                    var minLng = positions.Min(p => p.Longitude);
                    var maxLat = positions.Max(p => p.Latitude);
                    var maxLng = positions.Max(p => p.Longitude);

                    return new Bounds(new Position(minLat, minLng), new Position(maxLat, maxLng));
                }
            }

            return null;
        }

        public static double? GetDistanceKilometersFromUser(this Item? item)
        {
            if (item.IsValidLocation() && App.LOCALIZACAO_USUARIO.IsValid())
            {
                var itemPosition = item.GetPosition();
                var userPosition = App.LOCALIZACAO_USUARIO.GetPosition();
                var distanceKilometers = Location.CalculateDistance(itemPosition.Latitude, itemPosition.Longitude, userPosition.Latitude, userPosition.Longitude, DistanceUnits.Kilometers);

                return distanceKilometers;
            }

            return default;
        }

        public static string ToGeoJson(this IEnumerable<Pin> source)
        {
            var features = source.Select(pin => new
            {
                type = "Feature",
                geometry = new
                {
                    type = "Point",
                    coordinates = new[]
                    {
                        pin.Position.Longitude,
                        pin.Position.Latitude
                    }
                },
                properties = new
                {
                    title = pin.Label,
                    snippet = pin.Address
                }
            });
            var geoJson = new
            {
                type = "FeatureCollection",
                features = features
            };
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            return JsonSerializer.Serialize(geoJson, options);
        }

    }
}