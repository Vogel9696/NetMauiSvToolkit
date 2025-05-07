using NetMauiSvToolkit.Enums;
using NetMauiSvToolkit.Events;

namespace NetMauiSvToolkit.Interfaces
{
    public interface ICameraPreviewManager : IDisposable
    {
        public NativePlatformCameraPreviewView CreateNativeView();
        public event EventHandler<CameraFrameReadyEventArgs> FrameReady;
        public void Connect();
        public void Disconnect();
    }

    public abstract class CameraPreviewManagerBase
    {
        protected readonly IMauiContext Context;
        public CameraLocationType CameraLocation { get; private set; }
        protected CameraPreviewManagerBase(IMauiContext context, CameraLocationType cameraLocation)
        {
            Context = context;
            CameraLocation = cameraLocation;
        }

        public virtual void UpdateCameraLocation(CameraLocationType cameraLocation)
        {
            CameraLocation = cameraLocation;
        }

        public async Task<bool> CheckPermissions()
            => (await Permissions.RequestAsync<Permissions.Camera>()) == PermissionStatus.Granted;

    }
}
