using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation.Collections;
using Windows.Graphics.DirectX.Direct3D11;
using Windows.Media.Effects;
using Windows.Media.MediaProperties;
using Windows.Storage.Streams;

namespace CustomVideoEffect.VideoEffects
{
    /// <summary>
    /// Applies custom video effect to a media clip, transforming RGB colours to grayscale.
    /// Please note that video effects must be implemented in Windows Runtime Components.
    /// </summary>
    public sealed class GrayscaleVideoEffect : IBasicVideoEffect
    {
        /// <summary>
        /// This means that the video effect won't modify the source frame.
        /// </summary>
        public bool IsReadOnly
        {
            get { return true; }
        }

        /// <summary>
        /// Returning an empty list will provide frames in ARGB32 format.
        /// Strangely, the frame bitmaps will be provided in BRGA32 instead.
        /// See https://msdn.microsoft.com/en-us/library/windows.media.effects.ibasicvideoeffect.supportedencodingproperties
        /// </summary>
        public IReadOnlyList<VideoEncodingProperties> SupportedEncodingProperties
        {
            get { return new List<VideoEncodingProperties>(); }
        }

        /// <summary>
        /// Our video effect will do all processing in the CPU.
        /// </summary>
        public MediaMemoryTypes SupportedMemoryTypes
        {
            get { return MediaMemoryTypes.Cpu; }
        }

        /// <summary>
        /// The video effect is time independent - any frame of the video will be able to be processed.
        /// </summary>
        public bool TimeIndependent
        {
            get { return true; }
        }

        /// <summary>
        /// Called when the video effect is unloaded, and used for freeing any used resources.
        /// </summary>
        /// <param name="reason"></param>
        public void Close(MediaEffectClosedReason reason)
        {
        }

        /// <summary>
        /// Called when the decoding process needs to skip frames.
        /// </summary>
        public void DiscardQueuedFrames()
        {
        }

        /// <summary>
        /// Used for applying our video effect to a single frame. 
        /// </summary>
        /// <param name="context"></param>
        public void ProcessFrame(ProcessVideoFrameContext context)
        {
            var inputFrameBitmap = context.InputFrame.SoftwareBitmap;

            // Create intermediate buffer for holding the frame pixel data.
            var frameSize = inputFrameBitmap.PixelWidth * inputFrameBitmap.PixelHeight * 4;
            var frameBuffer = new Buffer((uint)frameSize);

            // Copy bitmap data from the input frame.
            inputFrameBitmap.CopyToBuffer(frameBuffer);

            // Iterate through all pixels in the frame.
            var framePixels = frameBuffer.ToArray();
            for (int i = 0; i < frameSize; i += 4)
            {
                // Calculate the luminance based on the RGB values - this way we can convert it to grayscale.
                var bValue = framePixels[i];
                var gValue = framePixels[i + 1];
                var rValue = framePixels[i + 2];

                var luminance = ((rValue / 255.0f) * 0.2126f) +
                                ((gValue / 255.0f) * 0.7152f) +
                                ((bValue / 255.0f) * 0.0722f);

                // Set the pixel data to the calculated grayscale values.
                framePixels[i] = framePixels[i + 1] = framePixels[i + 2] = (byte)(luminance * 255.0f);
            }

            // Copy the modified frame data to the output frame.
            context.OutputFrame.SoftwareBitmap.CopyFromBuffer(framePixels.AsBuffer());
        }

        /// <summary>
        /// Called when the encoding properties change.
        /// </summary>
        /// <param name="encodingProperties"></param>
        /// <param name="device"></param>
        public void SetEncodingProperties(VideoEncodingProperties encodingProperties, IDirect3DDevice device)
        {
        }

        /// <summary>
        /// Called for passing any custom parameters to the video effect.
        /// </summary>
        /// <param name="configuration"></param>
        public void SetProperties(IPropertySet configuration)
        {
        }
    }
}
