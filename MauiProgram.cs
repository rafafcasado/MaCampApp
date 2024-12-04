using CommunityToolkit.Maui;
using FFImageLoading.Maui;
using MaCamp.CustomControls;
using MaCamp.Handlers;
using Microsoft.Extensions.Logging;
using Plugin.MauiMTAdmob;
using RGPopup.Maui.Extensions;

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
            builder.UseMauiCommunityToolkit();

            builder.UseMauiRGPopup(config =>
            {
                config.BackPressHandler = null;
                config.FixKeyboardOverlap = true;
            });

            builder.UsePlatformServices();
            builder.UseMauiApp<App>();

            builder.ConfigureMauiHandlers(collection =>
            {
                collection.AddHandler<AdMobBannerView, AdmobBannerHandler>();
                collection.AddHandler<AdmobRectangleBannerView, AdmobRectangleBannerHandler>();
                collection.AddHandler<CustomWebView, CustomWebViewHandler>();
                collection.AddHandler<IconView, IconViewHandler>();
            });

            builder.ConfigureFonts(collection =>
            {
                collection.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                collection.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

            // TODO: Add the entry points to your Apps here.
            // See also: https://learn.microsoft.com/dotnet/maui/fundamentals/app-lifecycle
            //builder.Services.AddTransient<AppShell>();

            if (System.Diagnostics.Debugger.IsAttached)
            {
                builder.Logging.AddDebug();
            }

            return builder.Build();
        }
    }
}