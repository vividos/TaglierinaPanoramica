using Android.App;
using Android.Runtime;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Distribute;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using System;

namespace TaglierinaPanoramica.Droid
{
    [Application]
    public class MainApplication : MauiApplication
{
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
            AppCenter.Start(
                "83722e40-26a1-40b3-bd01-10bcf629c46b",
                typeof(Distribute),
                typeof(Crashes));
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}
