using System.Windows.Media;
using System.Windows.Shapes;

namespace CapstoneProg3.model.Projectiles
{
    public class EnemyBullet : IProjectile
    {
        public Rectangle shape { get; private set; }

        public EnemyBullet()
        {
            shape = new Rectangle
            {
                Width = 5,  
                Height = 15, 
                Fill = Brushes.Red 
            };
        }
    }
}