using AVFoundation;
using CoreAnimation;
using CoreFoundation;
using CoreVideo;
using Foundation;
using NetMauiSvToolkit.Enums;
using NetMauiSvToolkit.Events;
using NetMauiSvToolkit.Interfaces;
using NetMauiSvToolkit.Models;
using UIKit;

namespace NetMauiSvToolkit.Platforms.MacCatalyst
{
    public class CameraPreviewManager(IMauiContext context, CameraLocationType cameraLocation) : CameraPreviewManagerBase(context, cameraLocation), ICameraPreviewManager
    {
        AVCaptureSession? captureSession;
        AVCaptureDevice? captureDevice;
        AVCaptureInput? captureInput = null;
        PreviewView? view;
        AVCaptureVideoDataOutput? videoDataOutput;
        AVCaptureVideoPreviewLayer? videoPreviewLayer;
        FrameAnalyzer? captureDelegate;
        DispatchQueue? dispatchQueue;

        public event EventHandler<CameraFrameReadyEventArgs>? FrameReady;

        public NativePlatformView CreateNativeView()
        {
            captureSession = new AVCaptureSession
            {
                SessionPreset = AVCaptureSession.Preset640x480
            };

            videoPreviewLayer = new AVCaptureVideoPreviewLayer(captureSession);
            videoPreviewLayer.VideoGravity = AVLayerVideoGravity.ResizeAspectFill;

            view = new PreviewView(videoPreviewLayer);

            return view;
        }

        public void Disconnect()
        {
            if (captureSession != null)
            {
                if (captureSession.Running)
                    captureSession.StopRunning();

                captureSession.RemoveOutput(videoDataOutput);

                // Cleanup old input
                if (captureInput != null && captureSession.Inputs.Length > 0 && captureSession.Inputs.Contains(captureInput))
                {
                    captureSession.RemoveInput(captureInput);
                    captureInput.Dispose();
                    captureInput = null;
                }

                // Cleanup old device
                if (captureDevice != null)
                {
                    captureDevice.Dispose();
                    captureDevice = null;
                }
            }
        }

        public void Dispose()
        {
        }

        public void UpdateTorch(bool on)
        {
            if (captureDevice != null && captureDevice.HasTorch && captureDevice.TorchAvailable)
            {
                var isOn = captureDevice?.TorchActive ?? false;

                try
                {
                    if (on != isOn)
                    {
                        CaptureDevicePerformWithLockedConfiguration(() =>
                                                                        captureDevice.TorchMode = on ? AVCaptureTorchMode.On : AVCaptureTorchMode.Off);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        public void Focus(Microsoft.Maui.Graphics.Point point)
        {
            if (captureDevice == null)
                return;

            var focusMode = AVCaptureFocusMode.AutoFocus;
            if (captureDevice.IsFocusModeSupported(AVCaptureFocusMode.ContinuousAutoFocus))
                focusMode = AVCaptureFocusMode.ContinuousAutoFocus;

            //See if it supports focusing on a point
            if (captureDevice.FocusPointOfInterestSupported && !captureDevice.AdjustingFocus)
            {
                CaptureDevicePerformWithLockedConfiguration(() =>
                {
                    //Focus at the point touched
                    captureDevice.FocusPointOfInterest = point;
                    captureDevice.FocusMode = focusMode;
                });
            }
        }

        void CaptureDevicePerformWithLockedConfiguration(Action handler)
        {
            if (captureDevice.LockForConfiguration(out var err))
            {
                try
                {
                    handler();
                }
                finally
                {
                    captureDevice.UnlockForConfiguration();
                }
            }
        }

        public void AutoFocus()
        {
            if (captureDevice == null)
                return;

            var focusMode = AVCaptureFocusMode.AutoFocus;
            if (captureDevice.IsFocusModeSupported(AVCaptureFocusMode.ContinuousAutoFocus))
                focusMode = AVCaptureFocusMode.ContinuousAutoFocus;

            CaptureDevicePerformWithLockedConfiguration(() =>
            {
                if (captureDevice.FocusPointOfInterestSupported)
                    captureDevice.FocusPointOfInterest = CoreGraphics.CGPoint.Empty;
                captureDevice.FocusMode = focusMode;
            });
        }
        public void Connect()
        {
            UpdateCamera();

            if (videoDataOutput == null)
            {
                videoDataOutput = new AVCaptureVideoDataOutput();

                var videoSettings = NSDictionary.FromObjectAndKey(
                                                                  new NSNumber((int)CVPixelFormatType.CV32BGRA),
                                                                  CVPixelBuffer.PixelFormatTypeKey);

                videoDataOutput.WeakVideoSettings = videoSettings;

                if (captureDelegate == null)
                {
                    captureDelegate = new FrameAnalyzer
                    {
                        SampleProcessor = cvPixelBuffer =>
                                              FrameReady?.Invoke(this, new CameraFrameReadyEventArgs(new CameraFramePixelBufferHolderModel
                                              {
                                                  Data = cvPixelBuffer,
                                                  Size = new Size(cvPixelBuffer.Width, cvPixelBuffer.Height)
                                              }))
                    };
                }

                if (dispatchQueue == null)
                    dispatchQueue = new DispatchQueue("CameraBufferQueue");

                videoDataOutput.AlwaysDiscardsLateVideoFrames = true;
                videoDataOutput.SetSampleBufferDelegate(captureDelegate, dispatchQueue);
            }

            captureSession.AddOutput(videoDataOutput);
        }

        public void UpdateCamera()
        {
            if (captureSession != null)
            {
                if (captureSession.Running)
                    captureSession.StopRunning();

                // Cleanup old input
                if (captureInput != null && captureSession.Inputs.Length > 0 && captureSession.Inputs.Contains(captureInput))
                {
                    captureSession.RemoveInput(captureInput);
                    captureInput.Dispose();
                    captureInput = null;
                }

                // Cleanup old device
                if (captureDevice != null)
                {
                    captureDevice.Dispose();
                    captureDevice = null;
                }

                var devices = AVCaptureDevice.DevicesWithMediaType(AVMediaTypes.Video.GetConstant());
                foreach (var device in devices)
                {
                    if (CameraLocation == CameraLocationType.FrontCamera &&
                        device.Position == AVCaptureDevicePosition.Front)
                    {
                        captureDevice = device;
                        break;
                    }
                    else if (CameraLocation == CameraLocationType.RearCamera && device.Position == AVCaptureDevicePosition.Back)
                    {
                        captureDevice = device;
                        break;
                    }
                }

                if (captureDevice == null)
                    captureDevice = AVCaptureDevice.GetDefaultDevice(AVMediaTypes.Video);

                if (captureDevice is null)
                    return;

                captureInput = new AVCaptureDeviceInput(captureDevice, out var err);

                captureSession.AddInput(captureInput);

                captureSession.StartRunning();
            }
        }
    }

    class PreviewView : UIView
    {
        public PreviewView(AVCaptureVideoPreviewLayer layer) : base()
        {
            PreviewLayer = layer;

            PreviewLayer.Frame = Layer.Bounds;
            Layer.AddSublayer(PreviewLayer);
        }

        public readonly AVCaptureVideoPreviewLayer PreviewLayer;

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            CATransform3D transform = CATransform3D.MakeRotation(0, 0, 0, 1.0f);
            switch (UIDevice.CurrentDevice.Orientation)
            {
                case UIDeviceOrientation.Portrait:
                    transform = CATransform3D.MakeRotation(0, 0, 0, 1.0f);
                    break;
                case UIDeviceOrientation.PortraitUpsideDown:
                    transform = CATransform3D.MakeRotation((nfloat)Math.PI, 0, 0, 1.0f);
                    break;
                case UIDeviceOrientation.LandscapeLeft:
                    transform = CATransform3D.MakeRotation((nfloat)(-Math.PI / 2), 0, 0, 1.0f);
                    break;
                case UIDeviceOrientation.LandscapeRight:
                    transform = CATransform3D.MakeRotation((nfloat)Math.PI / 2, 0, 0, 1.0f);
                    break;
            }

            PreviewLayer.Transform = transform;
            PreviewLayer.Frame = Layer.Bounds;
        }
    }
}
