namespace MaCamp.Utils
{
    public sealed class ResourceImageSource : StreamImageSource
    {
        public string Value { get; }

        private ResourceImageSource(string value, Func<CancellationToken, Task<Stream>> streamFunc)
        {
            Value = value;
            Stream = streamFunc;
        }

        public static ImageSource From(string resourceId)
        {
            var assembly = typeof(ResourceImageSource).Assembly;
            var name = assembly.GetManifestResourceNames().FirstOrDefault(x => x.Contains(resourceId));
            var resourceStream = name != null ? assembly.GetManifestResourceStream(name) : System.IO.Stream.Null;
            var stream = resourceStream ?? System.IO.Stream.Null;

            return new ResourceImageSource(resourceId, cancellationToken => Task.FromResult(stream));
        }
    }
}
