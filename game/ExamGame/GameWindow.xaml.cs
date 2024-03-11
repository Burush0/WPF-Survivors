using System;
using System.Diagnostics;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ExamGame
{
    public partial class MainWindow : Window
    {
        private int enemySpawnDelay = 500;
        private int bulletSpawnDelay = 100;

        private int stage;

        private bool isPaused = false;

        private LevelingSystem levelingSystem;
        private Player player;
        private Enemy enemy;
        private List<Enemy> enemies = new List<Enemy>();

        private Stopwatch gameStopwatch = new Stopwatch();
        private Stopwatch enemyStopwatch = new Stopwatch();
        private Stopwatch bulletStopwatch = new Stopwatch();
        private Stopwatch iFramesStopwatch = new Stopwatch();

        private TimeSpan elapsedTime;
        private int killCounter = 0;
        private int _level = 1;

        private int goldGained = 0;

        private Canvas _gameCanvas;

        private MediaPlayer gameOver;
        private MediaPlayer bgm;

        public MainWindow(int level)
        {
            stage = level;
            InitializeComponent();
            InitializeGame();
        }

        private void InitializeGame()
        {

            bgm = new MediaPlayer();
            bgm.MediaEnded += BGM_Ended;

            AddBackgroundImages();

            Uri path = new Uri(@"assets/sounds/sfx/game_over.wav", UriKind.RelativeOrAbsolute);
            gameOver = new MediaPlayer();
            gameOver.Open(path);

            _gameCanvas = gameCanvas;

            player = new Player(gameCanvas, enemies);
            levelingSystem = new LevelingSystem();
            UpdateProgressBar();

            ((App)Application.Current).EnemySpawnDelay = enemySpawnDelay;
            ((App)Application.Current).EnemySpawnDelay = bulletSpawnDelay;

            CompositionTarget.Rendering += CompositionTarget_Rendering;
            bgm.Play();

            gameStopwatch = new Stopwatch();
            enemyStopwatch = new Stopwatch();
            bulletStopwatch = new Stopwatch();
            iFramesStopwatch = new Stopwatch();

            gameStopwatch.Start();
            enemyStopwatch.Start();
            bulletStopwatch.Start();
            iFramesStopwatch.Start();

            player.KeyDown += OnKeyDown;
            player.KeyUp += OnKeyUp;
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(1);
            if (gameStopwatch.Elapsed >= timeSpan)
            {
                elapsedTime += timeSpan;
                UpdateGameTimeText();
                gameStopwatch.Restart();
            }

            TimeSpan enemySpawnTime = TimeSpan.FromMilliseconds(enemySpawnDelay);
            if (enemyStopwatch.Elapsed >= enemySpawnTime)
            {
                enemy = new Enemy(gameCanvas, player);
                enemies.Add(enemy);
                enemyStopwatch.Restart();
            }

            if (((App)Application.Current).Autoshoot)
            {
                TimeSpan bulletSpawnTime = TimeSpan.FromMilliseconds(bulletSpawnDelay);
                if (bulletStopwatch.Elapsed >= bulletSpawnTime)
                {
                    player.ShootBullet();
                    bulletStopwatch.Restart();
                }
            }

            if (player.iFrames)
            {
                TimeSpan iFramesSpan = TimeSpan.FromSeconds(2);
                double opacity = 1;

                if (iFramesStopwatch.ElapsedMilliseconds < 400)
                {
                    opacity = 0.5;
                }
                else if (iFramesStopwatch.ElapsedMilliseconds < 800)
                {
                    opacity = 1.0;
                }
                else if (iFramesStopwatch.ElapsedMilliseconds < 1200)
                {
                    opacity = 0.5;
                }
                else if (iFramesStopwatch.ElapsedMilliseconds < 1600)
                {
                    opacity = 1.0;
                }
                else if (iFramesStopwatch.ElapsedMilliseconds < 2000)
                {
                    opacity = 0.5;
                }
                else
                {
                    opacity = 1.0;
                }
                player.playerSprite.Opacity = opacity;

                if (iFramesStopwatch.Elapsed >= iFramesSpan)
                {
                    player.playerSprite.Opacity = 1;
                    player.iFrames = false;
                    iFramesStopwatch.Restart();
                }
            }

            GameLoop();
        }

        // Цикл игры - обновляется каждый кадр
        private void GameLoop()
        {
            bgm.Volume = Configuration.Instance.MusicEnabled ? Configuration.Instance.MusicVolume : 0;
            if (!isPaused)
            {
                player.UpdatePlayerPosition(gameCanvas);
                foreach (var enemy in enemies)
                {
                    enemy.UpdateEnemyPosition();

                    // Предотвращение столкновений врагов
                    foreach (var otherEnemy in enemies.Where(e => e != enemy))
                    {
                        if (enemy.IsCollidingWith(otherEnemy))
                        {
                            enemy.ResolveCollisionWith(otherEnemy);
                        }
                    }
                }
                UpdateCameraPosition();
                foreach (var bullet in player.bullets.ToList())
                {
                    bullet.UpdateBulletPosition();

                    foreach (var enemy in enemies.ToList())
                    {
                        if (CheckBulletEnemyCollision(bullet, enemy))
                        {
                            bullet.KillEnemy(enemy);
                            killCounter++;
                            levelingSystem.GainExperience(1);
                            UpdateProgressBar();
                            goldGained += 5;
                            _gameCanvas.Children.Remove(bullet.bullet);
                            player.bullets.Remove(bullet);
                            RemoveEnemy(enemy);
                        }
                    }
                }
                foreach (var enemy in enemies.ToList())
                {
                    if (CheckEnemyPlayerCollision(enemy, player))
                    {
                        if (!player.iFrames)
                        {
                            iFramesStopwatch.Restart();
                            player.iFrames = true;
                            player.playerCurHp -= 10;
                            player.UpdateHP(player.playerCurHp);
                            if (player.playerCurHp <= 0)
                            {
                                if (!player.isPlayerDead)
                                {
                                    gameStopwatch.Stop();
                                    enemyStopwatch.Stop();
                                    player.PlayerDeath();
                                    player.GameOver();
                                    bgm.Stop();
                                    gameOver.Play();
                                }
                            }
                        }
                    }
                }
                if (player.deathComplete)
                {
                    foreach (var enemy in enemies.ToList())
                    {
                        enemy.EnemyDeath();
                        enemy.UnloadEnemy();
                    }
                    var cursorUri = new Uri("assets/cursors/menu.cur", UriKind.RelativeOrAbsolute);
                    Cursor = new Cursor(Application.GetResourceStream(cursorUri).Stream);
                    uiElements.Opacity = 0.5;
                    game.Opacity = 0.5;
                    gameOverScreen.Visibility = Visibility.Visible;
                    _level = int.Parse(levelTextBlock.Text);
                }
            }
        }

        private void UpdateProgressBar()
        {
            levelProgressBar.Value = (double)levelingSystem.Experience / levelingSystem.EnemiesToNextLevel * 100;
            levelTextBlock.Text = levelingSystem.Level + "";
        }

        private void TogglePause()
        {
            if (!player.isPlayerDead)
            {
                isPaused = !isPaused;

                if (isPaused)
                {
                    bgm.Pause();
                    gameStopwatch.Stop();
                    enemyStopwatch.Stop();
                    player.playerContainer.Children.Remove(player.playerSprite);
                    player.AddPlayerSprite(1);
                    player.playerContainer.Children.Add(player.playerSprite);
                    foreach (Enemy enemy in enemies)
                    {
                        if (!enemy.isDead)
                        {
                            enemy.enemyContainer.Children.Remove(enemy.enemySprite);
                            enemy.AddEnemySprite(1);
                            enemy.enemyContainer.Children.Add(enemy.enemySprite);
                        }
                    }
                    var cursorUri = new Uri("assets/cursors/menu.cur", UriKind.RelativeOrAbsolute);
                    Cursor = new Cursor(Application.GetResourceStream(cursorUri).Stream);
                    uiElements.Opacity = 0.5;
                    game.Opacity = 0.5;
                    pauseScreen.Visibility = Visibility.Visible;
                }
                else
                {
                    bgm.Play();
                    gameStopwatch.Start();
                    enemyStopwatch.Start();
                    player.playerContainer.Children.Remove(player.playerSprite);
                    player.AddPlayerSprite(0);
                    player.playerContainer.Children.Add(player.playerSprite);
                    foreach (Enemy enemy in enemies)
                    {
                        if (!enemy.isDead)
                        {
                            enemy.enemyContainer.Children.Remove(enemy.enemySprite);
                            enemy.AddEnemySprite(0);
                            enemy.enemyContainer.Children.Add(enemy.enemySprite);
                        }
                    }
                    var cursorUri = new Uri("assets/cursors/game.cur", UriKind.RelativeOrAbsolute);
                    Cursor = new Cursor(Application.GetResourceStream(cursorUri).Stream);
                    uiElements.Opacity = 1;
                    game.Opacity = 1;
                    pauseScreen.Visibility = Visibility.Hidden;
                }
            }
        }

        private bool CheckBulletEnemyCollision(Bullet bullet, Enemy enemy)
        {
            Rect bulletBounds = new Rect(bullet.bulletPosition.X, bullet.bulletPosition.Y, bullet.bullet.Width, bullet.bullet.Height);
            Rect enemyBounds = new Rect(enemy.enemyPosition.X, enemy.enemyPosition.Y, 40, 80);

            return bulletBounds.IntersectsWith(enemyBounds);
        }

        private bool CheckEnemyPlayerCollision(Enemy enemy, Player player)
        {
            Rect enemyBounds = new Rect(enemy.enemyPosition.X, enemy.enemyPosition.Y, 40, 80);
            Rect playerBounds = new Rect(player.playerPosition.X, player.playerPosition.Y, 40, 50);

            return enemyBounds.IntersectsWith(playerBounds);
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && !isPaused)
            {
                player.ShootBullet();
            }
        }

        private void UpdateGameTimeText()
        {
            gameTime.Text = $"{elapsedTime:m\\:ss}";
        }

        private void RemoveEnemy(Enemy enemy)
        {
            enemies.Remove(enemy);
        }

        private void BGM_Ended(object sender, EventArgs e)
        {
            bgm.Position = TimeSpan.Zero;
            bgm.Play();
        }

        // Инициализация карты, 25x25 плиток
        private void AddBackgroundImages()
        {
            Uri imageUri;
            Uri bgmUri;
            switch (stage)
            {
                case 2:
                    imageUri = new Uri("assets/images/chess_tile.jpg", UriKind.RelativeOrAbsolute);
                    bgmUri = new Uri(@"assets/sounds/bgm/stage2.mp3", UriKind.RelativeOrAbsolute);
                    break;
                default:
                    imageUri = new Uri("assets/images/grass_tile.jpg", UriKind.RelativeOrAbsolute);
                    bgmUri = new Uri(@"assets/sounds/bgm/stage1.mp3", UriKind.RelativeOrAbsolute);
                    break;

            }

            bgm.Open(bgmUri);

            for (int row = 0; row < 25; row++)
            {
                for (int col = 0; col < 25; col++)
                {

                    Image backgroundImage = new Image
                    {
                        Source = new BitmapImage(imageUri),
                        Width = gameCanvas.Width / 25,
                        Height = gameCanvas.Height / 25
                    };

                    Canvas.SetLeft(backgroundImage, col * backgroundImage.Width);
                    Canvas.SetTop(backgroundImage, row * backgroundImage.Height);

                    gameCanvas.Children.Add(backgroundImage);
                }
            }
        }

        // Камера двигается за игроком
        private void UpdateCameraPosition()
        {
            double targetX = player.playerPosition.X - ActualWidth / 2;
            double targetY = player.playerPosition.Y - ActualHeight / 2;

            theScrollView.ScrollToHorizontalOffset(targetX);
            theScrollView.ScrollToVerticalOffset(targetY);
        }

        private void PauseGame_Click(object sender, RoutedEventArgs e)
        {
            TogglePause();
        }

        private void ResumeGame_Click(object sender, RoutedEventArgs e)
        {
            TogglePause();
        }

        private void Options_Click(object sender, RoutedEventArgs e)
        {
            OptionsWindow window = new OptionsWindow();
            window.Show();
        }

        private void SaveAndQuit_Click(object sender, RoutedEventArgs e)
        {
            Quit_Click(sender, e);
        }
        private void ClearCanvas()
        {
            foreach (UIElement element in gameCanvas.Children)
            {
                if (element is Image image)
                {
                    image.Source = null;
                }
            }
            gameCanvas.Children.Clear();
        }

        private void DisposeStopwatches()
        {
            gameStopwatch = null;
            enemyStopwatch = null;
            bulletStopwatch = null;
            iFramesStopwatch = null;
        }

        private void Quit_Click(object sender, RoutedEventArgs e)
        {
            ClearCanvas();
            gameOver.Stop();
            gameOver.Close();
            if (((App)Application.Current).Gold < 0)
            {
                ((App)Application.Current).Gold = 0;
            }
            ((App)Application.Current).Gold += goldGained;
            ResultWindow window = new ResultWindow(killCounter, _level, elapsedTime);
            window.Show();
            CompositionTarget.Rendering -= CompositionTarget_Rendering;
            DisposeStopwatches();
            Close();
        }

        private void OnKeyUp(object? sender, KeyEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                TogglePause();
            }
            else if (e.Key == Key.J)
            {
                ((App)Application.Current).Autoaim = !((App)Application.Current).Autoaim;
            }
            else if (e.Key == Key.K)
            {
                ((App)Application.Current).Autoshoot = !((App)Application.Current).Autoshoot;
            }
        }
    }
}
