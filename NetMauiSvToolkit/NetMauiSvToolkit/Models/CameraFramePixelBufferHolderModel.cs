namespace NetMauiSvToolkit.Models
{
    public record CameraFramePixelBufferHolderModel
    {
#if ANDROID
        public Java.Nio.ByteBuffer? Data { get; init; }
#elif IOS || MACCATALYST
		public CoreVideo.CVPixelBuffer? Data {  get; init; }
#elif WINDOWS
		public Windows.Graphics.Imaging.SoftwareBitmap? Data {  get; init; }
#endif
        public Size? Size { get; init; }
    }
}
