using NetMauiSvToolkit.Enums;

namespace NetMauiSvToolkit.Interfaces
{
    public interface ICameraPreviewView : IView, ICameraFrameAnalyzer
    {
        CameraLocationType CameraLocationType { get; set; }

        void AutoFocus();

        void Focus(Point point);

        bool IsTorchOn { get; set; }
    }
}
