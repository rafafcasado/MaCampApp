using System.Diagnostics;
using CommunityToolkit.Maui;
using FFImageLoading.Maui;
using FluentIcons.Maui;
using MaCamp.CustomControls;
using MaCamp.Dependencias;
using MaCamp.Utils;
using Microsoft.Extensions.Logging;
using Plugin.MauiMTAdmob;
using RGPopup.Maui.Extensions;
using Map = Microsoft.Maui.Controls.Maps.Map;

namespace MaCamp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder.UseMauiMaps();
            builder.UseMauiMTAdmob();
            builder.UseFFImageLoading();
            builder.UseFluentIcons(false);
            builder.UseMauiCommunityToolkit();

            builder.UseMauiRGPopup(config =>
            {
                config.BackPressHandler = null;
                config.FixKeyboardOverlap = true;
            });

            builder.Services.AddPlatformSingleton<ILocalize>();
            builder.Services.AddPlatformSingleton<ISQLite>();
            builder.Services.AddPlatformSingleton<IStoragePermission>();

            builder.UseMauiApp<App>();

            builder.ConfigureMauiHandlers(collection =>
            {
                collection.AddPlatformHandler<AdMobBannerView>();
                collection.AddPlatformHandler<AdMobBannerView>();
                collection.AddPlatformHandler<AdmobRectangleBannerView>();
                collection.AddPlatformHandler<CustomWebView>();
                collection.AddPlatformHandler<IconView>();
                collection.AddPlatformHandler<Map>();
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