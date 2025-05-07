using Microsoft.Maui.Handlers;
using NetMauiSvToolkit.Enums;
using NetMauiSvToolkit.Events;
using NetMauiSvToolkit.Interfaces;
#if ANDROID
using NetMauiSvToolkit.Platforms.Android;
#elif IOS
using NetMauiSvToolkit.Platforms.iOS;
#endif

namespace NetMauiSvToolkit.ViewHandlers
{
    public class CameraPreviewViewHandler : ViewHandler<ICameraPreviewView, NativePlatformCameraPreviewView>
    {
        public static PropertyMapper<ICameraPreviewView, CameraPreviewViewHandler> CameraViewMapper = new()
        {
            [nameof(ICameraPreviewView.IsTorchOn)] = (handler, virtualView) => handler.cameraManager.UpdateTorch(virtualView.IsTorchOn),
            [nameof(ICameraPreviewView.CameraLocationType)] = (handler, virtualView) => handler.cameraManager.UpdateCameraLocation(virtualView.CameraLocationType)
        };

        public static CommandMapper<ICameraPreviewView, CameraPreviewViewHandler> CameraCommandMapper = new()
        {
            [nameof(ICameraPreviewView.Focus)] = MapFocus,
            [nameof(ICameraPreviewView.AutoFocus)] = MapAutoFocus,
        };

#if ANDROID
        CameraPreviewManager cameraManager;
#elif IOS
CameraPreviewManager cameraManager;
#endif

        public CameraPreviewViewHandler() : base(CameraViewMapper)
        {
        }

        public CameraPreviewViewHandler(PropertyMapper mapper = null) : base(mapper ?? CameraViewMapper)
        {
        }

        protected override NativePlatformCameraPreviewView CreatePlatformView()
        {
            if (cameraManager == null)
                cameraManager = new(MauiContext, VirtualView?.CameraLocationType ?? CameraLocationType.RearCamera);
            var v = cameraManager.CreateNativeView();
            return v;
        }

        protected override async void ConnectHandler(NativePlatformCameraPreviewView nativeView)
        {
            base.ConnectHandler(nativeView);

            if (cameraManager != null)
            {
                if (await cameraManager.CheckPermissions())
                    cameraManager.Connect();

                cameraManager.FrameReady += CameraManager_FrameReady;
            }
        }

        void CameraManager_FrameReady(object sender, CameraFrameReadyEventArgs e)
            => VirtualView?.FrameReady(e);

        protected override void DisconnectHandler(NativePlatformCameraPreviewView nativeView)
        {
            if (cameraManager != null)
            {
                cameraManager.FrameReady -= CameraManager_FrameReady;

                cameraManager.Disconnect();
                cameraManager.Dispose();
            }

            base.DisconnectHandler(nativeView);
        }

        public void Dispose()
            => cameraManager?.Dispose();

        public void Focus(Point point)
            => cameraManager?.Focus(point);

        public void AutoFocus()
            => cameraManager?.AutoFocus();

        private static void MapFocus(ICameraPreviewView handler, ICameraPreviewView view, object? parameter)
        {
            if (parameter is not Point point)
                throw new ArgumentException("Invalid parameter", "point");

            handler.Focus(point);
        }

        public static void MapAutoFocus(CameraPreviewViewHandler handler, ICameraPreviewView cameraBarcodeReaderView, object? parameters)
            => handler.AutoFocus();
    }
}
