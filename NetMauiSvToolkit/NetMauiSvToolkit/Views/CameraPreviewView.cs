using NetMauiSvToolkit.Enums;
using NetMauiSvToolkit.Events;
using NetMauiSvToolkit.Interfaces;
using NetMauiSvToolkit.ViewHandlers;

namespace NetMauiSvToolkit.Views
{
    public class CameraPreviewView : View, ICameraPreviewView
    {
        public event EventHandler<CameraFrameReadyEventArgs> FrameReady;

        public CameraPreviewView()
        {
            Unloaded += (s, e) => Cleanup();
        }

        void ICameraFrameAnalyzer.FrameReady(CameraFrameReadyEventArgs e)
            => FrameReady?.Invoke(this, e);

        public static readonly BindableProperty IsTorchOnProperty =
            BindableProperty.Create(nameof(IsTorchOn), typeof(bool), typeof(CameraPreviewView), defaultValue: true);

        public bool IsTorchOn
        {
            get => (bool)GetValue(IsTorchOnProperty);
            set => SetValue(IsTorchOnProperty, value);
        }

        public static readonly BindableProperty CameraLocationProperty =
            BindableProperty.Create(nameof(CameraLocationType), typeof(CameraLocationType), typeof(CameraPreviewView), defaultValue: CameraLocationType.RearCamera);

        public CameraLocationType CameraLocationType
        {
            get => (CameraLocationType)GetValue(CameraLocationProperty);
            set => SetValue(CameraLocationProperty, value);
        }

        public void AutoFocus()
            => StrongHandler?.Invoke(nameof(AutoFocus), null);

        public void Focus(Point point)
            => StrongHandler?.Invoke(nameof(Focus), point);

        CameraPreviewViewHandler StrongHandler
            => Handler as CameraPreviewViewHandler;

        private void Cleanup()
            => Handler?.DisconnectHandler();
    }
}
