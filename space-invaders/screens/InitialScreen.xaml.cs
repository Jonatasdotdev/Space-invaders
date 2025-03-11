using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using CapstoneProg3.background;
using CapstoneProg3.utils;

namespace CapstoneProg3.screens
{
    public partial class InitialScreen : Window
    {
        DispatcherTimer backgroundTimer = new DispatcherTimer();
        public InitialScreen()
        {
            InitializeComponent();

            PathUtils.CreateGameDirectory();
            PathUtils.CreateScoreFile();

            Title.Source = new BitmapImage(new Uri(PathUtils.GetFullPath("assets/space-invaders-title.png"), UriKind.Absolute));
            //VideoPlayer.Source = new Uri(PathUtils.GetFullPath("assets/test2.mp4"));
            
            var iconPath = PathUtils.GetFullPath("assets/alien.png");
            this.Icon = new BitmapImage(new Uri(iconPath, UriKind.Absolute));
            
            // VideoPlayer.MediaEnded += VideoPlayer_MediaEnded;
            // VideoPlayer.Play();
            
            var cyclingBackground = new CyclingBackgroundHardcore(Background);
            backgroundTimer = new DispatcherTimer{ Interval = TimeSpan.FromMilliseconds(20) };
            backgroundTimer.Tick += cyclingBackground.OnTick;
            backgroundTimer.Start();
            
        }
        
        private void ButtonPlay_OnClick(object sender, RoutedEventArgs e)
        {
            Game mainWindow = new Game();
            mainWindow.Show();
            backgroundTimer.Stop();
            this.Close();
        }
        
        private void VideoPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            // VideoPlayer.Position = System.TimeSpan.Zero;
            // VideoPlayer.Play();
        }

        private void ButtonScore_OnClick(object sender, RoutedEventArgs e)
        {
            var scoreScreen = new ScoreScreen();
            scoreScreen.Show();
        }
        
        private void ButtonExit_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
