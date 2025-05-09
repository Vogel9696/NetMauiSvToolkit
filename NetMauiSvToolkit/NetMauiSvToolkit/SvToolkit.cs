using NetMauiSvToolkit.ViewHandlers;
using NetMauiSvToolkit.Views;

namespace NetMauiSvToolkit
{
    public static class SvToolkit
    {
        public static MauiAppBuilder UseSvToolkit(this MauiAppBuilder app)
        {
            app.Services.ConfigureMauiHandlers(handlers => { handlers.AddHandler(typeof(CameraPreviewView), typeof(CameraPreviewViewHandler)); });

            return app;
        }
    }
}
