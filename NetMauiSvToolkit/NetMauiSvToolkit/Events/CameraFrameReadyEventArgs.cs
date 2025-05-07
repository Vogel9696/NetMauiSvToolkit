using NetMauiSvToolkit.Models;

namespace NetMauiSvToolkit.Events
{
    public class CameraFrameReadyEventArgs : EventArgs
    {
        public CameraFrameReadyEventArgs(CameraFramePixelBufferHolderModel holder) : base() => Data = holder;

        public readonly CameraFramePixelBufferHolderModel Data;
    }
}
