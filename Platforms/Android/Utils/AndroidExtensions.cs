using System.Reflection;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Org.Json;

namespace MaCamp.Platforms.Android.Utils
{
    public static class AndroidExtensions
    {
        public static BitmapDescriptor GetManifestResourceBitmapDescriptor(this Assembly assembly, string name)
        {
            using var stream = assembly.GetManifestResourceStream(name);

            if (stream != null)
            {
                using var memoryStream = new MemoryStream();

                stream.CopyTo(memoryStream);

                var bytes = memoryStream.ToArray();
                var bitmap = BitmapFactory.DecodeByteArray(bytes, 0, bytes.Length);

                if (bitmap != null)
                {
                    return BitmapDescriptorFactory.FromBitmap(bitmap);
                }
            }

            return BitmapDescriptorFactory.DefaultMarker();
        }

        public static List<JSONObject> ToList(this JSONObject jsonObject, string key)
        {
            var list = new List<JSONObject>();

            if (jsonObject.Has(key))
            {
                var jsonArray = jsonObject.GetJSONArray(key);

                for (var i = 0; i < jsonArray.Length(); i++)
                {
                    var value = jsonArray.GetJSONObject(i);

                    if (value != null)
                    {
                        list.Add(value);
                    }
                }
            }

            return list;
        }

        public static bool TryGetValue<T>(this JSONObject jsonObject, string key, out T value)
        {
            var targetType = typeof(T);

            if (targetType == typeof(string) && jsonObject.Has(key) && jsonObject.GetString(key) is T stringValue)
            {
                value = stringValue;

                return true;
            }

            if (targetType == typeof(double) && jsonObject.Has(key) && jsonObject.GetDouble(key) is T doubleValue)
            {
                value = doubleValue;

                return true;
            }

            if (targetType.IsValueType && targetType.FullName != null && targetType.FullName.StartsWith("System.ValueTuple`"))
            {
                var array = jsonObject.GetJSONArray(key);

                if (CreateValueTupleFromJsonArray(targetType, array, out var tuple) && tuple is T tupleValue)
                {
                    value = tupleValue;

                    return true;
                }
            }

            throw new ArgumentException($"Cannot convert value of type {targetType} to {typeof(JSONObject)}");
        }

        private static bool CreateValueTupleFromJsonArray(Type tupleType, JSONArray jsonArray, out object? value)
        {
            value = null;

            var genericArgs = tupleType.GetGenericArguments();

            if (jsonArray.Length() == genericArgs.Length)
            {
                var ctorArgs = new object[genericArgs.Length];

                for (var i = 0; i < genericArgs.Length; i++)
                {
                    var argType = genericArgs[i];
                    object element = argType switch
                    {
                        var t when t == typeof(string) => jsonArray.GetString(i) ?? string.Empty,
                        var t when t == typeof(double) => jsonArray.GetDouble(i),
                        var t when t == typeof(int) => jsonArray.GetInt(i),
                        var t when t == typeof(bool) => jsonArray.GetBoolean(i),
                        _ => throw new NotSupportedException($"Tipo de tupla '{argType}' não suportado")
                    };

                    ctorArgs[i] = element;
                }

                value = Activator.CreateInstance(tupleType, ctorArgs);
            }

            return value != null;
        }
    }
}
