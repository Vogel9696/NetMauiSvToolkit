using NetMauiSvToolkit.Events;

namespace NetMauiSvToolkit.Interfaces
{
    public interface ICameraFrameAnalyzer
    {
        void FrameReady(CameraFrameReadyEventArgs args);
    }
}
