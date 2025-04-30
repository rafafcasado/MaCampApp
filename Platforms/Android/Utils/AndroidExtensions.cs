using System.Reflection;
using Android.Gms.Maps.Model;
using Android.Graphics;

namespace MaCamp.Platforms.Android.Utils
{
    public static class AndroidExtensions
    {
        public static BitmapDescriptor GetManifestResourceBitmapDescriptor(this Assembly assembly, string name)
        {
            using var stream = assembly.GetManifestResourceStream(name);
            using var memoryStream = new MemoryStream();

            if (stream != null)
            {
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
    }
}
