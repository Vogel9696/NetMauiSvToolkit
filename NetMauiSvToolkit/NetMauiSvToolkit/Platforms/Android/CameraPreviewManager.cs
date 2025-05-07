using AndroidX.Camera.Core;
using AndroidX.Camera.Lifecycle;
using AndroidX.Camera.View;
using AndroidX.Core.Content;
using Java.Util.Concurrent;
using NetMauiSvToolkit.Enums;
using NetMauiSvToolkit.Events;
using NetMauiSvToolkit.Interfaces;
using NetMauiSvToolkit.Models;
using Point = Android.Graphics.Point;
using Size = Android.Util.Size;

namespace NetMauiSvToolkit.Platforms.Android
{
    public class CameraPreviewManager(IMauiContext context, CameraLocationType cameraLocation) : CameraPreviewManagerBase(context, cameraLocation), ICameraPreviewManager
    {
        AndroidX.Camera.Core.Preview? cameraPreview;
        ImageAnalysis? imageAnalyzer;
        PreviewView? previewView;
        IExecutorService? cameraExecutor;
        CameraSelector? cameraSelector = null;
        ProcessCameraProvider? cameraProvider;
        ICamera? camera;

        public event EventHandler<CameraFrameReadyEventArgs>? FrameReady;

        public void UpdateTorch(bool on)
        {
            camera?.CameraControl?.EnableTorch(on);
        }

        public void Focus(Point point)
        {

        }

        public void AutoFocus()
        {

        }

        public NativePlatformCameraPreviewView CreateNativeView()
        {
            previewView = new PreviewView(Context.Context);
            cameraExecutor = Executors.NewSingleThreadExecutor();

            return previewView;
        }

        public override void UpdateCameraLocation(CameraLocationType cameraLocation)
        {
            base.UpdateCameraLocation(cameraLocation);

            UpdateCamera();
        }
        public void Connect()
        {
            var cameraProviderFuture = ProcessCameraProvider.GetInstance(Context.Context);

            cameraProviderFuture.AddListener(new Java.Lang.Runnable(() =>
            {
                // Used to bind the lifecycle of cameras to the lifecycle owner
                cameraProvider = (ProcessCameraProvider)cameraProviderFuture.Get();

                // Preview
                cameraPreview = new AndroidX.Camera.Core.Preview.Builder().Build();
                cameraPreview.SetSurfaceProvider(previewView.SurfaceProvider);

                // Frame by frame analyze
                imageAnalyzer = new ImageAnalysis.Builder()
                    .SetDefaultResolution(new Size(1280, 720))
                    .SetBackpressureStrategy(ImageAnalysis.StrategyKeepOnlyLatest)
                    .Build();

                imageAnalyzer.SetAnalyzer(cameraExecutor, new FrameAnalyzer((buffer, size) =>
                    FrameReady?.Invoke(this, new CameraFrameReadyEventArgs(new CameraFramePixelBufferHolderModel { Data = buffer, Size = size }))));

                UpdateCamera();

            }), ContextCompat.GetMainExecutor(Context.Context)); //GetMainExecutor: returns an Executor that runs on the main thread.
        }

        public void UpdateCamera()
        {
            if (cameraProvider != null)
            {
                // Unbind use cases before rebinding
                cameraProvider.UnbindAll();

                var cameraLocation = CameraLocation;

                // Select back camera as a default, or front camera otherwise
                if (cameraLocation == CameraLocationType.RearCamera && cameraProvider.HasCamera(CameraSelector.DefaultBackCamera))
                    cameraSelector = CameraSelector.DefaultBackCamera;
                else if (cameraLocation == CameraLocationType.FrontCamera && cameraProvider.HasCamera(CameraSelector.DefaultFrontCamera))
                    cameraSelector = CameraSelector.DefaultFrontCamera;
                else
                    cameraSelector = CameraSelector.DefaultBackCamera;

                if (cameraSelector == null)
                    throw new System.Exception("Camera not found");

                // The Context here SHOULD be something that's a lifecycle owner
                if (Context.Context is AndroidX.Lifecycle.ILifecycleOwner lifecycleOwner)
                {
                    camera = cameraProvider.BindToLifecycle(lifecycleOwner, cameraSelector, cameraPreview, imageAnalyzer);
                }
                else if (Microsoft.Maui.ApplicationModel.Platform.CurrentActivity is AndroidX.Lifecycle.ILifecycleOwner maLifecycleOwner)
                {
                    // if not, this should be sufficient as a fallback
                    camera = cameraProvider.BindToLifecycle(maLifecycleOwner, cameraSelector, cameraPreview, imageAnalyzer);
                }
            }
        }

        public void Dispose()
        {
            cameraProvider?.Shutdown();

            cameraExecutor?.Shutdown();
            cameraExecutor?.Dispose();
        }
        public void Disconnect()
        { }
    }
}
