using System.Diagnostics;
using CommunityToolkit.Maui;
using FFImageLoading.Maui;
using FluentIcons.Maui;
using MaCamp.CustomControls;
using MaCamp.Dependencias;
using MaCamp.Dependencias.Permissions;
using MaCamp.Utils;
using Microsoft.Extensions.Logging;
using MPowerKit.GoogleMaps;
using Plugin.MauiMTAdmob;
using RGPopup.Maui.Extensions;

namespace MaCamp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder.UseFluentIcons();
            builder.UseMauiMTAdmob();
            builder.UseMauiApp<App>();
            builder.UseFFImageLoading();
            builder.UsePlatformMaps();
            builder.UseMauiCommunityToolkit();
            builder.UseMauiRGPopup(config =>
            {
                config.BackPressHandler = null;
                config.FixKeyboardOverlap = true;
            });

            builder.Services.AddPlatformSingleton<ISQLite>();
            builder.Services.AddPlatformSingleton<ILocalize>();
            builder.Services.AddPlatformSingleton<IStoragePermission>();
            builder.Services.AddPlatformSingleton<ILocationPermission>();
            builder.Services.AddPlatformSingleton<INotification>();

            builder.ConfigureMauiHandlers(collection =>
            {
                collection.AddPlatformHandler<AdMobBannerView>();
                collection.AddPlatformHandler<AdmobRectangleBannerView>();
                collection.AddPlatformHandler<GoogleMap>();
                collection.AddPlatformHandler<CustomWebView>();
                collection.AddPlatformHandler<IconView>();
            });

            builder.ConfigureFonts(collection =>
            {
                collection.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                collection.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

            if (Debugger.IsAttached)
            {
                builder.Logging.AddDebug();
            }

            return builder.Build();
        }
    }
}