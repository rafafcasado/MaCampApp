using System.Reflection;
using Microsoft.Maui.Handlers;

namespace MaCamp.Utils
{
    public static class Registers
    {
        public static bool IsFromRunningPlatform(this Type type)
        {
            var platform = DeviceInfo.Current.Platform.ToString();
            var value = type.Namespace != null && type.Namespace.Contains(platform, StringComparison.OrdinalIgnoreCase);

            return value;
        }

        public static bool IsViewHandler(this Type type, Type viewType)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ViewHandler<,>))
            {
                return type.GetGenericArguments().FirstOrDefault() == viewType;
            }

            if (type.BaseType != null && type.BaseType.IsGenericType && type.BaseType.GetGenericTypeDefinition() == typeof(ViewHandler<,>))
            {
                var baseType = type.BaseType.GetGenericArguments().FirstOrDefault();

                return viewType == baseType || viewType.BaseType == baseType || viewType.GetInterfaces().Any(x => baseType == x);
            }

            return false;
        }

        public static IServiceCollection AddPlatformSingleton<T>(this IServiceCollection serviceCollection) where T : class
        {
            var type = typeof(T);
            var assembly = Assembly.GetExecutingAssembly();
            var listPlataformServices = assembly.GetTypes().Where(x => x.IsClass && !x.IsAbstract && type.IsAssignableFrom(x) && x.IsFromRunningPlatform()).ToList();

            foreach (var plataformService in listPlataformServices)
            {
                serviceCollection.AddSingleton(type, plataformService);
            }

            return serviceCollection;
        }

        public static IServiceCollection AddPlatformHandler<T>(this IMauiHandlersCollection handlersCollection) where T : class
        {
            var type = typeof(T);
            var assembly = Assembly.GetExecutingAssembly();
            var listPlataformHandlers = assembly.GetTypes().Where(x => x.IsClass && !x.IsAbstract && x.BaseType != null && x.BaseType.IsViewHandler(type) && x.IsFromRunningPlatform()).ToList();

            foreach (var plataformHandle in listPlataformHandlers)
            {
                handlersCollection.AddHandler(type, plataformHandle);
            }

            return handlersCollection;
        }
    }
}
