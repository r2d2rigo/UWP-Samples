using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.UI.Notifications;
using Windows.UI.Xaml.Media.Imaging;
using NotificationsExtensions.Tiles;
using NotificationsExtensions;
using Windows.Foundation.Collections;

namespace BackgroundTask
{
    /// <summary>
    /// Background task that allows us to render our custom UserControl into an image.
    /// Remember to subclass XamlRenderingBackgroundTask as it's the only one with access to the UI thread.
    /// </summary>
    public sealed class TileUpdateTask : XamlRenderingBackgroundTask
    {
        private static readonly int TileWidth = 150;
        private static readonly int TileHeight = 150;
        private static readonly string TileImageFilename = "MediumTile.png";

        protected override async void OnRun(IBackgroundTaskInstance taskInstance)
        {
            var deferral = taskInstance.GetDeferral();

            // Create instance of the control that contains the layout of the Live Tile
            MediumTileControl control = new MediumTileControl();
            control.Width = TileWidth;
            control.Height = TileHeight;

            // If we have received a message in the parameters, overwrite the default "Hello, Live Tile!" one
            var triggerDetails = taskInstance.TriggerDetails as ApplicationTriggerDetails;
            if (triggerDetails != null)
            {
                object tileMessage = null;
                if (triggerDetails.Arguments.TryGetValue("Message", out tileMessage))
                {
                    if (tileMessage is string)
                    {
                        control.Message = (string)tileMessage;
                    }
                }
            }            

            // Render the tile control to a RenderTargetBitmap
            RenderTargetBitmap bitmap = new RenderTargetBitmap();
            await bitmap.RenderAsync(control, TileWidth, TileHeight);

            // Now we are going to save it to a PNG file, so create/open it on local storage
            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(TileImageFilename, CreationCollisionOption.ReplaceExisting);
            using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                // Create a BitmapEncoder for encoding to PNG
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);

                // Create a SoftwareBitmap from the RenderTargetBitmap, as it will be easier to save to disk
                using (var softwareBitmap = new SoftwareBitmap(BitmapPixelFormat.Bgra8, TileWidth, TileHeight, BitmapAlphaMode.Premultiplied))
                {
                    // Copy bitmap data
                    softwareBitmap.CopyFromBuffer(await bitmap.GetPixelsAsync());

                    // Encode and save to file
                    encoder.SetSoftwareBitmap(softwareBitmap);
                    await encoder.FlushAsync();
                }
            }

            // Use the NotificationsExtensions library to easily configure a tile update
            TileContent mediumTileContent = new TileContent()
            {
                Visual = new TileVisual()
                {
                    TileMedium = new TileBinding()
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            BackgroundImage = new TileBackgroundImage()
                            {
                                Overlay = 0,
                                Source = new TileImageSource("ms-appdata:///local/" + TileImageFilename),
                            }
                        }
                    }
                }
            };

            // Clean previous update from Live Tile and update with the new parameters
            var tileUpdater = TileUpdateManager.CreateTileUpdaterForApplication();
            tileUpdater.Clear();
            tileUpdater.Update(new TileNotification(mediumTileContent.GetXml()));
            
            deferral.Complete();
        }
    }
}
