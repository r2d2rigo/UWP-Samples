using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace BackgroundTask
{
    /// <summary>
    /// Custom UserControl for displaying Live Tile contents.
    /// </summary>
    public sealed partial class MediumTileControl : UserControl
    {
        internal static readonly DependencyProperty BackgroundColorProperty = DependencyProperty.Register(
            "BackgroundColor",
            typeof(SolidColorBrush),
            typeof(MediumTileControl),
            new PropertyMetadata(new SolidColorBrush(Colors.Teal)));

        internal static readonly DependencyProperty MessageProperty = DependencyProperty.Register(
            "Message",
            typeof(string),
            typeof(MediumTileControl),
            new PropertyMetadata("Hello, Live Tile!"));

        public SolidColorBrush BackgroundColor
        {
            get { return (SolidColorBrush)this.GetValue(BackgroundColorProperty); }
            set { this.SetValue(BackgroundColorProperty, value); }
        }

        public string Message
        {
            get { return (string)this.GetValue(MessageProperty); }
            set { this.SetValue(MessageProperty, value); }
        }

        public MediumTileControl()
        {
            this.InitializeComponent();
        }
    }
}
