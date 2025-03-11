using System.Windows.Media;
using System.Windows.Shapes;

namespace CapstoneProg3.model.Projectiles
{
    public class Bullet : IProjectile
    {
        public Rectangle shape { get; private set; }

        public Bullet()
        {
            shape = new Rectangle
            {
                Width = 5,  // Largura do tiro
                Height = 15, // Altura do tiro
                Fill = Brushes.Yellow // Cor do tiro
            };
        }
    }
}