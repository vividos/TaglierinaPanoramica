using CommunityToolkit.Maui;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace TaglierinaPanoramica
{
    /// <summary>
    /// Maup program
    /// </summary>
    public static class MauiProgram
    {
        /// <summary>
        /// Creates an app object
        /// </summary>
        /// <returns>app object</returns>
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiCommunityToolkit()
                .UseSkiaSharp()
                .UseMauiApp<App>();

#if ANDROID
            builder.Services.AddSingleton<IPhotoLibrary, Droid.PhotoLibrary>();
#elif WINDOWS
            builder.Services.AddSingleton<IPhotoLibrary, WinUI.PhotoLibrary>();
#endif

            return builder.Build();
        }
    }
}
