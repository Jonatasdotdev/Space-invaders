using System.IO;
using System.Net.Mime;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CapstoneProg3.model;
using CapstoneProg3.utils;

namespace CapstoneProg3.screens
{
    public partial class ScoreScreen : Window
    {
        public ScoreScreen()
        {
            InitializeComponent();
            DataContext = this;
            
            var iconPath = PathUtils.GetFullPath("assets/alien.png");
            this.Icon = new BitmapImage(new Uri(iconPath, UriKind.Absolute));
            
            SpaceInvadersTitle.Source =
                new BitmapImage(new Uri(PathUtils.GetFullPath("assets/space-invaders-title.png"), UriKind.Absolute));

            LoadScores();
        }

        private void LoadScores()
        {
            var fontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#Space Invaders");
            var thickness = new Thickness(5, 5, 5, 5);
            
            var jsonPath = PathUtils.GameFilePath;

            var json = File.ReadAllText(jsonPath);
            var allScores = JsonSerializer.Deserialize<List<Score>>(json);

            var topScores = allScores.OrderByDescending(s => s.points).Take(10).ToList();

            string[] ordinals = ["1ST.", "2ND.", "3RD.", "4TH.", "5TH.", "6TH.", "7TH.", "8TH.", "9TH.", "10TH."];

            ScoresPanel.Children.Clear();

            for (var i = 0; i < topScores.Count; i++)
            {
                var score = topScores[i];

                // row definition for each score column
                var scoreRow = new Grid();
                scoreRow.ColumnDefinitions.Add(new ColumnDefinition
                    { Width = new GridLength(100) }); // position
                scoreRow.ColumnDefinitions.Add(new ColumnDefinition 
                    { Width = new GridLength(100) }); // score
                scoreRow.ColumnDefinitions.Add(new ColumnDefinition
                    { Width = new GridLength(1, GridUnitType.Star) }); // username

                // position text
                var ordinalText = new TextBlock
                {
                    Text = ordinals[i],
                    Foreground = Brushes.White,
                    FontSize = 28,
                    FontWeight = FontWeights.Bold,
                    FontFamily = fontFamily,
                    TextAlignment = TextAlignment.Left,
                    Margin = thickness
                };
                Grid.SetColumn(ordinalText, 0);
                scoreRow.Children.Add(ordinalText);

                // score text
                var scoreText = new TextBlock
                {
                    Text = score.points.ToString(),
                    Foreground = Brushes.White,
                    FontSize = 28,
                    FontWeight = FontWeights.Bold,
                    FontFamily = fontFamily,
                    TextAlignment = TextAlignment.Right,
                    Margin = thickness
                };
                Grid.SetColumn(scoreText, 1);
                scoreRow.Children.Add(scoreText);

                // username text
                var usernameText = new TextBlock
                {
                    Text = score.username.ToUpper(),
                    Foreground = Brushes.White,
                    FontSize = 28,
                    FontWeight = FontWeights.Bold,
                    FontFamily = fontFamily,
                    TextAlignment = TextAlignment.Left,
                    Margin = thickness
                };
                Grid.SetColumn(usernameText, 2);
                scoreRow.Children.Add(usernameText);
                
                ScoresPanel.Children.Add(scoreRow);
            }
        }
    }
}