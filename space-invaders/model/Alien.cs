using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CapstoneProg3.utils;

namespace CapstoneProg3.model
{
    public class Alien
    {
        public int point { get; set; }
        public char type { get; set; }
        public Rectangle shape { get; set; }

        public Alien(char type)
        {
            ImageBrush imageBrush;
            BitmapImage image;
            string imagePath;
            this.type = type;
            switch (type)
            {
                case 'b': // Boss
                    point = 200;
                    
                    imagePath = PathUtils.GetFullPath("assets/boss.png");
                    
                    image = new BitmapImage(new Uri(imagePath, UriKind.Absolute));
                    shape = new Rectangle
                    {
                        Width = image.Width,
                        Height = image.Height
                    };
                    break;

                case 'e': // Easy
                    point = 10;
                    
                    imagePath = PathUtils.GetFullPath("assets/mobEasy.png");
                    
                    image = new BitmapImage(new Uri(imagePath, UriKind.Absolute));
                    shape = new Rectangle
                    {
                        Width = image.Width,
                        Height = image.Height
                    };

                    break;

                case 'm': // Medium
                    point = 20;
                    
                    imagePath = PathUtils.GetFullPath("assets/mobMedium.png");

                    image = new BitmapImage(new Uri(imagePath, UriKind.Absolute));
                    shape = new Rectangle
                    {
                        Width = image.Width,
                        Height = image.Height
                    };
                    break;

                case 'h': // Hard
                    point = 40;
                    imagePath = PathUtils.GetFullPath("assets/mobHard.png");
                    
                    image = new BitmapImage(new Uri(imagePath, UriKind.Absolute));
                    
                    shape = new Rectangle
                    {
                        Width = image.Width,
                        Height = image.Height,
                    };
                    
                    break;

                default:
                    throw new ArgumentException("Tipo de inimigo desconhecido: " + type);
            }

            imageBrush = new ImageBrush(image);
            shape.Fill = imageBrush;
        }
    }

}
