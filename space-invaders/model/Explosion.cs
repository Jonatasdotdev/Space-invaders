using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using CapstoneProg3.utils;

namespace CapstoneProg3.model
{
    public class Explosion
    {
        public Image shape { get; private set; }
        private BitmapImage spriteSheet; // Imagem contendo todos os frames
        private int frameWidth; // Largura de cada frame
        private int frameHeight; // Altura de cada frame
        private int currentFrame = 0; // Frame atual
        private int horizontalSprites = 5;
        private int verticalSprites = 1;
        private int totalFrames; // Número total de frames
        private DispatcherTimer animationTimer; // Timer para controlar a animação
        public delegate void ExplosionFinishedHandler(Explosion explosion);
        public event ExplosionFinishedHandler ExplosionFinished;

        public Explosion(double x, double y)
        {
            // Carregar o sprite sheet
            string spriteSheetPath = PathUtils.GetFullPath("assets/explosion_sheet2.png");
            spriteSheet = new BitmapImage(new Uri(spriteSheetPath, UriKind.Absolute));

            // Definir o tamanho de cada frame

            frameWidth = (int)spriteSheet.Width/horizontalSprites; // Largura de cada frame
            frameHeight = (int)spriteSheet.Height/verticalSprites; // Altura de cada frame
            totalFrames = (int)(horizontalSprites * verticalSprites); // Número total de frames

            // Criar a imagem que exibirá o frame atual
            shape = new Image
            {
                Width = frameWidth,
                Height = frameHeight,
                Source = new CroppedBitmap(spriteSheet,
                    new Int32Rect(0, 0, frameWidth, frameHeight)) // Exibir o primeiro frame
            };

            // Posicionar a explosão
            Canvas.SetLeft(shape, x - frameWidth / 2); // Centralizar no alien
            Canvas.SetTop(shape, y - frameHeight / 2);

            // Iniciar a animação
            animationTimer = new DispatcherTimer();
            animationTimer.Interval = TimeSpan.FromMilliseconds(90); // Trocar de frame a cada 100ms
            animationTimer.Tick += AnimationTimer_Tick;
            animationTimer.Start();
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            currentFrame++;
            int frameX = (currentFrame % horizontalSprites) * frameWidth; // Posição X do frame no sprite sheet
            int frameY = (currentFrame / horizontalSprites) * frameHeight; // Posição Y do frame no sprite sheet
            
            if (currentFrame >= totalFrames)
            {
                animationTimer.Stop(); // Parar a animação quando todos os frames forem exibidos
                ExplosionFinished?.Invoke(this);
            }
            else
            {
                // Exibir o próximo frame
                shape.Source = new CroppedBitmap(spriteSheet, new Int32Rect(frameX, frameY, frameWidth, frameHeight));
            }
        }
    }
}