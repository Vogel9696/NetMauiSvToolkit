using NetMauiSvToolkit.Enums;
using NetMauiSvToolkit.Events;
using NetMauiSvToolkit.Interfaces;
using static Microsoft.Maui.ApplicationModel.Permissions;

namespace NetMauiSvToolkit.Platforms.Windows
{
    public class CameraPreviewManager(IMauiContext context, CameraLocationType cameraLocation) : CameraPreviewManagerBase(context, cameraLocation), ICameraPreviewManager
    {
        public event EventHandler<CameraFrameReadyEventArgs> FrameReady;

        public void Connect()
        {
            throw new NotImplementedException();
        }

        public NativePlatformView CreateNativeView()
        {
            throw new NotImplementedException();
        }

        public void Disconnect()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void UpdateTorch(bool on)
        {
            throw new NotImplementedException();
        }

        public void Focus(Point point)
        {
            throw new NotImplementedException();
        }

        public void AutoFocus()
        {
            throw new NotImplementedException();
        }
    }
}
