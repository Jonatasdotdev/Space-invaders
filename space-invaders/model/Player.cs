using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CapstoneProg3.utils;

namespace CapstoneProg3.model
{
    public class Player
    {
        public int points { get; set; }
        public Rectangle shape { get; set; }
        private ImageBrush imageBrush;
        public event Action? onDeath; 
        public HealthSystem Health { get; private set; }
        
        public Player()
        {
            points = 0;
            Health = new HealthSystem();

            // Criar o retângulo que representará a nave
            shape = new Rectangle
            {
                Width = 64,  // Largura da nave
                Height = 64, // Altura da nave
            };

            // Carregar a imagem da nave
            string imagePath = PathUtils.GetFullPath("assets/spaceship.png");
            BitmapImage image = new BitmapImage(new Uri(imagePath, UriKind.Absolute));
            imageBrush = new ImageBrush(image);

            // Aplicar a imagem ao retângulo
            shape.Fill = imageBrush;
        }

        public void TookedDamage()
        {
           //TODO

        }


        public void removeLife()
        {
            this.Health.TakeDamage();
            TookedDamage();
            if (Health.currentHealth <= 0) onDeath?.Invoke();
        }
    }
}