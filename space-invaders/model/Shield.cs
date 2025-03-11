using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CapstoneProg3.utils;

namespace CapstoneProg3.model
{
    public class Shield
    {
        private ImageBrush _imageBrush;
        public int life { get; private set; }
        private double opacity { get; set; }
        public Rectangle shape { get; set; }

        public Shield() 
        {
            life = 5;
            opacity = 1;
            shape = new Rectangle 
            {
                Width = 90,
                Height = 64
            };

            string imagePath = PathUtils.GetFullPath("assets/Shield.png");

            BitmapImage image = new BitmapImage(new Uri(imagePath, UriKind.Absolute));
            _imageBrush = new ImageBrush(image);
            shape.Fill = _imageBrush;
        }

        public void UpdateOpacity()
        {
            _imageBrush.Opacity = opacity;
        }

        public void removeShieldLife()
        {
            life--;
            if (life > 0)
            {
                opacity = opacity -= 0.2;
                UpdateOpacity();
            }
        }

    }
}