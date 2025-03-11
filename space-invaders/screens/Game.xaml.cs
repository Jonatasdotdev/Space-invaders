using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using CapstoneProg3.background;
using CapstoneProg3.model;
using CapstoneProg3.model.Projectiles;
using CapstoneProg3.utils;

namespace CapstoneProg3.screens
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Game : Window
    {
        private Player player;
        private readonly List<Shield> shieldList = new List<Shield>(); // Lista de escudos
        private readonly List<Alien> aliensList = new List<Alien>();
        private readonly DispatcherTimer enemyBulletCd;
        private readonly DispatcherTimer globalTimer;
        private bool canShoot = true;
        private double alienSpeed = 1.5; // Velocidade de movimento dos aliens
        private bool moveDown = false; // Indica se os aliens devem descer
        private readonly List<Bullet> bullets = new List<Bullet>(); // Lista de tiros ativos
        private Random random = new Random();
        private double enemyBulletShootingTime;
        private readonly List<EnemyBullet> enemyBullets = [];
        private readonly HashSet<Key> keys = [];
        private bool bossAdded = false;
        private Alien boss;
        private int bossApparitionPoints = 500;
        private double bossSpeed = 1.5; // Velocidade de movimento do boss
        private double bulletFrequency = 1;
        public double BulletSpeed = 8;

        public Game()
        {
            InitializeComponent();
            Loaded += GameWindowLoaded; // Executar após o layout estar carregado
            KeyDown += MainKeyDown;
            KeyUp += MainKeyUp;

            string backgroundMusicPath = PathUtils.GetFullPath("assets/sounds/musica_fundo.mpeg");
            SoundManager.PlayBackgroundMusic(backgroundMusicPath);

            globalTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(16) };
            globalTimer.Start();
            globalTimer.Tick += AttGame;
            var cyclingBackground = new CyclingBackgroundHardcore(background);
            globalTimer.Tick += cyclingBackground.OnTick!;

            enemyBulletShootingTime = random.Next(1, 3);
            enemyBulletCd = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };
            enemyBulletCd.Tick += EnemyShoot;
            enemyBulletCd.Start();
        }

        private void AttGame(object? sender, EventArgs e)
        {
            PlayerActions();
            MoveBoss();
            MoveBullets();
            MoveEnemyBullets();
            CheckBulletsCollisions();
            CheckEnemyBulletCollisions();
            MoveAliens();
        }

        private void GameWindowLoaded(object sender, RoutedEventArgs e)
        {
            InitializingGame(); // Chamar o método após o layout estar pronto
        }

        private void UpdateHealthUi()
        {
            healthCanvas.Children.Clear();

            for (int i = 0; i < 3; i++)
            {
                Image heartImage = new Image
                {
                    Width = 32,
                    Height = 32,
                    Source = new BitmapImage(new Uri(i < player.Health.currentHealth
                        ? PathUtils.FullHealthFilePath
                        : PathUtils.EmptyHealthFilePath))
                };

                Canvas.SetLeft(heartImage, i * 40);
                healthCanvas.Children.Add(heartImage);
            }
        }

        private void PlayerActions()
        {
            double x = Canvas.GetLeft(player.shape);
            double canvasWidth = background.ActualWidth;
            double playerWidth = player.shape.ActualWidth;

            if (keys.Contains(Key.Left)) x -= GameConstants.PlayerSpeed;
            if (keys.Contains(Key.Right)) x += GameConstants.PlayerSpeed;
            // if(keys.Contains(Key.Up)) player.removeLife(); Apenas para testes
            // if(keys.Contains(Key.Down)) player.Health.Heal();
            if (keys.Contains(Key.Space)) ShootBullet();

            x = Math.Max(0, Math.Min(x, canvasWidth - playerWidth));

            Canvas.SetLeft(player.shape, x);
        }


        private void MoveBoss()
        {
            if (boss == null) return; // Se não houver boss, não fazer nada

            double x = Canvas.GetLeft(boss.shape);

            // Movimento horizontal
            x += bossSpeed;

            // Verifica se atingiu os limites da tela
            if (x + boss.shape.ActualWidth > background.ActualWidth || x < 0)
            {
                bossSpeed *= -1; // Inverte a direção do movimento horizontal
            }

            // Aplica a nova posição
            Canvas.SetLeft(boss.shape, x);
        }

        private void MoveBullets()
        {
            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                var b = bullets[i];
                double y = Canvas.GetTop(b.shape);
                Canvas.SetTop(b.shape, y - BulletSpeed); // Subtrai para ir para cima

                if (y < 0)
                {
                    background.Children.Remove(b.shape);
                    bullets.RemoveAt(i);
                    canShoot = true;
                }
            }
        }

        private void MoveEnemyBullets()
        {
            for (int i = enemyBullets.Count - 1; i >= 0; i--)
            {
                var b = enemyBullets[i];
                double y = Canvas.GetTop(b.shape);
                Canvas.SetTop(b.shape, y + BulletSpeed); // Adiciona para descer

                if (y > background.ActualHeight) // Verifica se saiu da tela
                {
                    background.Children.Remove(b.shape);
                    enemyBullets.RemoveAt(i);
                }
            }
        }

        private void AddPlayer()
        {
            // Criar o jogador
            player = new Player();
            UpdateHealthUi();
            player.Health.OnHealthChanged += UpdateHealthUi;
            player.onDeath += GameOver;
            player.onDeath += SaveScore;
            // Calcular a posição central inferior
            double leftPlayer = (background.ActualWidth - player.shape.Width) / 2; // Centro horizontal
            double topPlayer = background.ActualHeight - player.shape.Height * 1.2; // Parte inferior
            // Adicionar o jogador ao Canvas
            background.Children.Add(player.shape);

            // Aplicar a posição ao jogador
            Canvas.SetLeft(player.shape, leftPlayer);
            Canvas.SetTop(player.shape, topPlayer);
        }

        private void InitializingGame()
        {
            AddPlayer();
            AddShield();
            AddAliens();

            var iconPath = PathUtils.GetFullPath("assets/alien.png");
            this.Icon = new BitmapImage(new Uri(iconPath, UriKind.Absolute));
        }

        private void AddShield()
        {
            double leftShield = 135;
            double topShield = background.ActualHeight - (player.shape.Height * 2) - 30;

            for (int i = 0; i < 4; i++)
            {
                Shield shield = new Shield();
                background.Children.Add(shield.shape);
                Canvas.SetTop(shield.shape, topShield);
                Canvas.SetLeft(shield.shape, leftShield);
                shieldList.Add(shield);
                leftShield += 238;
            }
        }

        private void AddAliens()
        {
            int leftAlien;
            int topAlien = 10;

            for (int i = 0; i < 5; i++) // 5 linhas de aliens
            {
                topAlien += 70; // Espaçamento vertical entre as linhas
                leftAlien = 120; // Posição inicial horizontal

                for (int j = 0; j < 10; j++) // 10 aliens por linha
                {
                    leftAlien += 70; // Espaçamento horizontal entre os aliens

                    Alien alien;
                    switch (i)
                    {
                        case 0:
                            alien = new Alien('h'); // Alien tipo 1
                            break;
                        case 1:
                        case 2:
                            alien = new Alien('m'); // Alien tipo 2
                            break;
                        case 3:
                        case 4:
                            alien = new Alien('e'); // Alien tipo 3
                            break;
                        default:
                            alien = new Alien('b'); // Alien padrão
                            break;
                    }

                    // Adicionar o alien ao Canvas e à lista
                    background.Children.Add(alien.shape);
                    Canvas.SetTop(alien.shape, topAlien);
                    Canvas.SetLeft(alien.shape, leftAlien);
                    aliensList.Add(alien);
                }
            }
        }

        private void MoveAliens()
        {
            moveDown = false; // Reinicia a flag de movimento para baixo

            foreach (var alien in aliensList)
            {
                double x = Canvas.GetLeft(alien.shape);

                // Movimento horizontal
                x += alienSpeed;

                // Verifica se atingiu os limites da tela
                if (x + alien.shape.ActualWidth > background.ActualWidth || x < 0)
                {
                    moveDown = true; // Ativa a flag para mover os aliens para baixo
                }

                // Aplica a nova posição
                Canvas.SetLeft(alien.shape, x);
            }

            // Movimento vertical (se necessário)
            if (moveDown)
            {
                alienSpeed *= -1; // Inverte a direção do movimento horizontal

                foreach (var alien in aliensList)
                {
                    double y = Canvas.GetTop(alien.shape);
                    y += 20; // Move os aliens para baixo
                    Canvas.SetTop(alien.shape, y);
                }
            }
        }

        private void MainKeyDown(object sender, KeyEventArgs e) => keys.Add(e.Key);
        private void MainKeyUp(object sender, KeyEventArgs e) => keys.Remove(e.Key);

        private void ShootBullet()
        {
            if (!canShoot) return;

            Bullet bullet = new Bullet();
            double bulletX = Canvas.GetLeft(player.shape) + player.shape.ActualWidth / 2 -
                             bullet.shape.ActualWidth / 2;
            double bulletY = Canvas.GetTop(player.shape) - bullet.shape.ActualHeight;

            background.Children.Add(bullet.shape);
            Canvas.SetLeft(bullet.shape, bulletX);
            Canvas.SetTop(bullet.shape, bulletY);

            bullets.Add(bullet);

            // Reproduzir som de tiro
            string shootSoundPath = PathUtils.GetFullPath("assets/sounds/tiro.wav");
            SoundManager.PlaySoundEffect(shootSoundPath);

            canShoot = false;
        }

        private void CheckBulletsCollisions()
        {
            // Percorre a lista de balas de trás para frente
            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                var bullet = bullets[i];
                bool bulletRemoved = false;

                // Verificar colisões com os aliens
                for (int j = aliensList.Count - 1; j >= 0; j--)
                {
                    var alien = aliensList[j];

                    if (CheckCollision(bullet.shape, alien.shape))
                    {
                        // Reproduzir som de invasor morto
                        string killedSoundPath = PathUtils.GetFullPath("assets/sounds/invasor_killed.wav");
                        SoundManager.PlaySoundEffect(killedSoundPath);

                        // Exibir explosão no local do alien
                        double explosionX = Canvas.GetLeft(alien.shape) + alien.shape.ActualWidth / 2;
                        double explosionY = Canvas.GetTop(alien.shape) + alien.shape.ActualHeight / 2;
                        Explosion explosion = new Explosion(explosionX, explosionY);
                        background.Children.Add(explosion.shape);

                        // Remover o alien
                        background.Children.Remove(alien.shape);
                        aliensList.RemoveAt(j);

                        // Remover o tiro
                        background.Children.Remove(bullet.shape);
                        bullets.RemoveAt(i);
                        bulletRemoved = true;

                        // Atualizar pontuação
                        player.points += alien.point;
                        if(bossAdded) bossApparitionPoints += alien.point;
                        scoreText.Text = $"Score: {player.points}";

                        explosion.ExplosionFinished += RemoveExplosion;
                        canShoot = true;

                        // Verificar se todos os aliens foram destruídos
                        if (aliensList.Count == 0)
                        {
                            AddNewHorde(); // Adicionar uma nova horda
                            IncreaseOverallSpeed();
                            player.Health.Heal();
                        }

                        break; // Sair do loop de aliens após a colisão
                    }
                }

                if (bulletRemoved) continue; // Evita verificar colisões se a bala já foi removida

                // Verificar colisões com o boss
                if (bossAdded && boss != null && CheckCollision(bullet.shape, boss.shape))
                {
                    // Exibir explosão no local do boss
                    double explosionX = Canvas.GetLeft(boss.shape) + boss.shape.ActualWidth / 2;
                    double explosionY = Canvas.GetTop(boss.shape) + boss.shape.ActualHeight / 2;
                    Explosion explosion = new Explosion(explosionX, explosionY);
                    background.Children.Add(explosion.shape);
                    SoundManager.PlaySoundEffect(PathUtils.ExplosionSoundPath);

                    // Atualizar pontuação
                    player.points += boss.point;
                    scoreText.Text = $"Score: {player.points}";

                    // Remover o boss
                    background.Children.Remove(boss.shape);
                    boss = null;
                    bossAdded = false;

                    // Remover o tiro
                    background.Children.Remove(bullet.shape);
                    bullets.RemoveAt(i);
                    canShoot = true;

                    explosion.ExplosionFinished += RemoveExplosion;
                    continue; // Pula a verificação de colisão com os escudos se a bala já foi removida
                }

                // Verificar colisões com os escudos
                for (int j = shieldList.Count - 1; j >= 0; j--)
                {
                    var shield = shieldList[j];

                    if (CheckCollision(bullet.shape, shield.shape))
                    {
                        shield.removeShieldLife();

                        if (shield.life == 0)
                        {
                            // Remover o escudo do canvas e da lista
                            background.Children.Remove(shield.shape);
                            shieldList.RemoveAt(j);
                        }

                        // Remover o projétil após a colisão
                        background.Children.Remove(bullet.shape);
                        bullets.RemoveAt(i);
                        canShoot = true;
                        break; // Sair do loop de escudos
                    }
                }
            }

            // Verificar se é hora de adicionar o boss
            if (!bossAdded && player.points >= bossApparitionPoints)
            {
                bossApparitionPoints += 500;
                AddBoss();
                bossAdded = true; // Evita adicionar o boss novamente
            }
        }


        private void CheckEnemyBulletCollisions()
        {
            // Percorre a lista de balas inimigas de trás para frente
            for (int i = enemyBullets.Count - 1; i >= 0; i--)
            {
                var bullet = enemyBullets[i];

                // Verifica colisão com o jogador
                if (CheckCollision(bullet.shape, player.shape))
                {
                    player.removeLife();
                    background.Children.Remove(bullet.shape);
                    enemyBullets.RemoveAt(i);
                    continue; // Pula a verificação de colisão com os escudos se já foi removido
                }

                // Percorre a lista de escudos de trás para frente para evitar problemas ao remover
                for (int j = shieldList.Count - 1; j >= 0; j--)
                {
                    var shield = shieldList[j];

                    if (CheckCollision(bullet.shape, shield.shape))
                    {
                        shield.removeShieldLife();
                        background.Children.Remove(bullet.shape);
                        enemyBullets.RemoveAt(i);

                        if (shield.life == 0)
                        {
                            background.Children.Remove(shield.shape);
                            shieldList.RemoveAt(j);
                        }

                        break; // Sai do loop de escudos, pois a bala já foi removida
                    }
                }
            }
        }

        private void AddNewHorde()
        {
            // Aumentar a dificuldade (opcional)
            IncreaseOverallSpeed(); // Aumenta a velocidade dos aliens
            AddAliens(); // Adiciona uma nova horda de aliens
        }

        private void IncreaseOverallSpeed()
        {
            bulletFrequency *= 1.1;
            BulletSpeed *= 1.05;

            if (bossSpeed > 0) bossSpeed += 0.1;
            else bossSpeed -= 0.1; //Objetos que se movem em ambos os sentidos podem acabar tendo valores negativos

            if (alienSpeed > 0) alienSpeed += 0.1;
            else
                alienSpeed -=
                    0.1; //Fazendo com que tenha a necessidade de ter um if para checagem na hora de aumentar a speed;
        }

        private void RemoveExplosion(Explosion explosion)
        {
            background.Children.Remove(explosion.shape);
        }

        private void EnemyShoot(object sender, EventArgs e)
        {
            var alienExist = aliensList.Where(alien => alien.type == 'h');
            if (!alienExist.Any()) return;

            Alien shooter = alienExist.ElementAt(random.Next(alienExist.Count()));

            EnemyBullet bullet = new EnemyBullet();
            double bulletX = Canvas.GetLeft(shooter.shape) + shooter.shape.ActualWidth / 2 -
                             bullet.shape.ActualWidth / 2;
            double bulletY = Canvas.GetTop(shooter.shape) + shooter.shape.ActualHeight;

            background.Children.Add(bullet.shape);
            Canvas.SetLeft(bullet.shape, bulletX);
            Canvas.SetTop(bullet.shape, bulletY);

            enemyBullets.Add(bullet);

            enemyBulletShootingTime = random.Next(1, 3);
            enemyBulletCd.Interval = TimeSpan.FromSeconds(enemyBulletShootingTime / bulletFrequency);
            enemyBulletCd.Start();
        }

        private void AddBoss()
        {
            boss = new Alien('b'); // Instanciar o boss

            // Calcular posição central no topo da tela
            double leftBoss = (background.ActualWidth - boss.shape.Width) / 2;
            double topBoss = 20; // Posição inicial acima dos aliens

            // Adicionar ao canvas
            background.Children.Add(boss.shape);
            Canvas.SetLeft(boss.shape, leftBoss);
            Canvas.SetTop(boss.shape, topBoss);
        }

        private bool CheckCollision(FrameworkElement element1, FrameworkElement element2)
        {
            Rect rect1 = new Rect(
                Canvas.GetLeft(element1),
                Canvas.GetTop(element1),
                element1.ActualWidth, // Usar ActualWidth para garantir as dimensões reais
                element1.ActualHeight // Usar ActualHeight para garantir as dimensões reais
            );

            Rect rect2 = new Rect(
                Canvas.GetLeft(element2),
                Canvas.GetTop(element2),
                element2.ActualWidth, // Usar ActualWidth para garantir as dimensões reais
                element2.ActualHeight // Usar ActualHeight para garantir as dimensões reais
            );

            return rect1.IntersectsWith(rect2);
        }

        private void StopTimers()
        {
            enemyBulletCd.Stop();
            globalTimer.Stop();
            keys.Clear();
            SoundManager.StopBackgroundMusic();
        }

        private void GameOver()
        {
            StopTimers();
            MessageBox.Show($"Game Over!\nPontuação final: {scoreText.Text}", "Game Over");
            InitialScreen initialScreen = new InitialScreen();
            initialScreen.Show();
            this.Close();
        }

        private void SaveScore()
        {
            var username = Environment.UserName[..3];
            var scoreList = new List<Score>();

            Score score = new Score
            {
                username = username.ToUpper(),
                points = player.points
            };

            if (File.Exists(PathUtils.GameFilePath))
            {
                var json = File.ReadAllText(PathUtils.GameFilePath);
                scoreList = JsonSerializer.Deserialize<List<Score>>(json) ?? [];
            }

            scoreList.Add(score);

            var updatedJson = JsonSerializer.Serialize(scoreList, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(PathUtils.GameFilePath, updatedJson);

            Console.WriteLine("Username: " + username.ToUpper() + " - Score: " + player.points);
        }
    }
}