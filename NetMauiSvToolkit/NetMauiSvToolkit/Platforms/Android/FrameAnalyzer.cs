using AndroidX.Camera.Core;
using Java.Nio;
using System.Diagnostics;

namespace NetMauiSvToolkit.Platforms.Android
{
    internal class FrameAnalyzer : Java.Lang.Object, ImageAnalysis.IAnalyzer
    {
        private readonly Action<ByteBuffer, Size> frameCallback;

        public FrameAnalyzer(Action<ByteBuffer, Size> callback)
        {
            frameCallback = callback;
        }

        public void Analyze(IImageProxy image)
        {
            var buffer = image.GetPlanes()[0].Buffer;

            var size = new Size(image.Width, image.Height);

            try
            {
                frameCallback?.Invoke(buffer, size);
            }
            finally
            {
                image.Close();
            }
        }
    }
}
