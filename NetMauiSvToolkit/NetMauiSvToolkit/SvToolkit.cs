using NetMauiSvToolkit.ViewHandlers;
using NetMauiSvToolkit.Views;

namespace NetMauiSvToolkit
{
    public static class SvToolkit
    {
        public static MauiAppBuilder UseSvToolkit(this MauiAppBuilder app)
        {
#if ANDROID
            app.Services.ConfigureMauiHandlers(handlers => { handlers.AddHandler(typeof(CameraPreviewView), typeof(CameraPreviewViewHandler)); });
#elif IOS
            app.Services.ConfigureMauiHandlers(handlers => { handlers.AddHandler(typeof(CameraPreviewView), typeof(CameraPreviewViewHandler)); });
#endif
            return app;
        }
    }
}
