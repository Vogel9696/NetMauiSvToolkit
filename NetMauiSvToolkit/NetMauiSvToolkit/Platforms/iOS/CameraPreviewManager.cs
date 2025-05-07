using NetMauiSvToolkit.Enums;
using NetMauiSvToolkit.Events;
using NetMauiSvToolkit.Interfaces;
using static Microsoft.Maui.ApplicationModel.Permissions;

namespace NetMauiSvToolkit.Platforms.iOS
{
    public class CameraPreviewManager : CameraPreviewManagerBase, ICameraPreviewManager
    {
        public CameraPreviewManager(IMauiContext context, CameraLocationType cameraLocation)
            : base(context, cameraLocation)
        {

        }

        public event EventHandler<CameraFrameReadyEventArgs> FrameReady;

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
        }

        public void Focus(Point point)
        {

        }

        public void AutoFocus()
        {

        }
        public void Connect() { }
    }
}
