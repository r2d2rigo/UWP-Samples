using CustomVideoEffect.VideoEffects;
using System;
using Windows.ApplicationModel;
using Windows.Media.Core;
using Windows.Media.Editing;
using Windows.Media.Effects;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CustomVideoEffect
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void VideoPlayer_Loaded(object sender, RoutedEventArgs e)
        {
            // Load video file
            var videoFile = await Package.Current.InstalledLocation.GetFileAsync("big_buck_bunny.mp4");

            // Create a MediaClip from the video file and apply our video effect
            MediaClip clip = await MediaClip.CreateFromFileAsync(videoFile);
            clip.VideoEffectDefinitions.Add(new VideoEffectDefinition(typeof(GrayscaleVideoEffect).FullName));

            // Create a MediaComposition object that will allow us to generate a stream source
            MediaComposition compositor = new MediaComposition();
            compositor.Clips.Add(clip);

            VideoPlayer.SetMediaPlayer(new Windows.Media.Playback.MediaPlayer());//otherwise it's null

            // Set the stream source to the MediaElement control
            this.VideoPlayer.MediaPlayer.Source = MediaSource.CreateFromIMediaSource(compositor.GenerateMediaStreamSource()) ;
        }

     
    }
}
