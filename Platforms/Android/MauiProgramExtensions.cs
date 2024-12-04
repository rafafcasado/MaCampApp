using MaCamp.Dependencias;
using MaCamp.Platforms.Android.Services;

// ReSharper disable once CheckNamespace
namespace MaCamp
{
    public static partial class MauiProgramExtensions
    {
        public static partial MauiAppBuilder UsePlatformServices(this MauiAppBuilder builder)
        {
            builder.Services.AddSingleton<ILocalize, Localize>();
            builder.Services.AddSingleton<ISQLite, SqliteDatabase>();

            return builder;
        }
    }
}