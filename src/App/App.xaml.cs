﻿using Microsoft.AppCenter;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Distribute;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace TaglierinaPanoramica
{
    /// <summary>
    /// Taglierina Panoramica app
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Creates a new app object
        /// </summary>
        public App()
        {
            AppCenter.Start(
                "83722e40-26a1-40b3-bd01-10bcf629c46b",
                typeof(Distribute),
                typeof(Crashes));

            this.InitializeComponent();

#if ANDROID
            DependencyService.Register<IPhotoLibrary, Droid.PhotoLibrary>();
#elif WINDOWS
            DependencyService.Register<IPhotoLibrary, WinUI.PhotoLibrary>();
#endif

            this.MainPage = new ImageCropPage();
        }
    }
}
