using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using Windows.Graphics.DirectX.Direct3D11;
using Windows.Media.Effects;
using Windows.Media.MediaProperties;
using Windows.Storage.Streams;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.IO;

namespace CustomVideoEffect.VideoEffects
{
    public sealed class GrayscaleVideoEffect : IBasicVideoEffect
    {
        public bool IsReadOnly
        {
            get { return true; }
        }

        public IReadOnlyList<VideoEncodingProperties> SupportedEncodingProperties
        {
            get { return new List<VideoEncodingProperties>(); }
        }

        public MediaMemoryTypes SupportedMemoryTypes
        {
            get { return MediaMemoryTypes.Cpu; }
        }

        public bool TimeIndependent
        {
            get { return true; }
        }

        public void Close(MediaEffectClosedReason reason)
        {
        }

        public void DiscardQueuedFrames()
        {
        }

        public void ProcessFrame(ProcessVideoFrameContext context)
        {
            var inputFrameBitmap = context.InputFrame.SoftwareBitmap;
            var frameSize = inputFrameBitmap.PixelWidth * inputFrameBitmap.PixelHeight * 4;
            var frameBuffer = new Buffer((uint)frameSize);

            inputFrameBitmap.CopyToBuffer(frameBuffer);

            var framePixels = frameBuffer.ToArray();
            for (int i = 0; i < frameSize; i += 4)
            {
                var bValue = framePixels[i];
                var gValue = framePixels[i + 1];
                var rValue = framePixels[i + 2];

                var luminance = ((rValue / 255.0f) * 0.2126f) +
                                ((gValue / 255.0f) * 0.7152f) +
                                ((bValue / 255.0f) * 0.0722f);

                framePixels[i] = framePixels[i + 1] = framePixels[i + 2] = (byte)(luminance * 255.0f);
            }

            context.OutputFrame.SoftwareBitmap.CopyFromBuffer(framePixels.AsBuffer());
        }

        public void SetEncodingProperties(VideoEncodingProperties encodingProperties, IDirect3DDevice device)
        {
        }

        public void SetProperties(IPropertySet configuration)
        {
        }
    }
}
