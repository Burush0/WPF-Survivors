using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfAnimatedGif;

namespace ExamGame
{
    internal class Player
    {
        private double playerSpeed = 5.0;
        public double playerMaxHp = 100.0;
        public double playerHpRegen = 0.1;

        public double playerCurHp;

        private Canvas _gameCanvas;

        private bool isFrozen;
        public bool iFrames;
        public bool isPlayerDead;
        public bool deathComplete = false;

        public Point playerPosition;
        private TranslateTransform playerTransform;
        private ScaleTransform playerScaleTransform;
        public Image playerSprite;
        public Grid playerContainer;
        private Rectangle playerHurtbox;
        private ProgressBar hpProgressBar;

        public List<Bullet> bullets = new List<Bullet>();
        public List<Enemy> _enemies = new List<Enemy>();

        public event EventHandler<KeyEventArgs> KeyDown;
        public event EventHandler<KeyEventArgs> KeyUp;

        public Player(Canvas gameCanvas, List<Enemy> enemies)
        {
            _gameCanvas = gameCanvas;
            _enemies = enemies;
            InitializePlayer();
        }

        private void InitializePlayer()
        {
            playerTransform = new TranslateTransform();
            playerScaleTransform = new ScaleTransform();

            AddPlayerSprite(0);
            AddPlayerHurtbox();
            AddHPBar(playerMaxHp);

            playerCurHp = playerMaxHp;

            // Контейнер для хёртбокса и спрайта
            playerContainer = new Grid();
            playerContainer.Children.Add(playerHurtbox);
            playerContainer.Children.Add(playerSprite);

            // Стартовая точка - в центре карты
            playerPosition = new Point(_gameCanvas.Width / 2, _gameCanvas.Height / 2);
            playerTransform.X = playerPosition.X;
            playerTransform.Y = playerPosition.Y;

            playerContainer.HorizontalAlignment = HorizontalAlignment.Center;
            playerContainer.VerticalAlignment = VerticalAlignment.Center;
            _gameCanvas.Children.Add(playerContainer);
        }

        public void ShootBullet()
        {
            if (!isFrozen)
            {
                if (((App)Application.Current).Autoaim)
                {
                    Enemy closestEnemy = GetClosestEnemy();
                    if (closestEnemy != null)
                    {
                        Vector direction = new Vector(closestEnemy.enemyPosition.X - playerPosition.X, closestEnemy.enemyPosition.Y - playerPosition.Y);
                        if (direction.Length > 0)
                            direction.Normalize();

                        Bullet bullet = new Bullet(_gameCanvas, playerPosition, direction);
                        bullets.Add(bullet);
                    }
                }
                else
                {
                    Point offsetPosition = playerPosition;
                    Point cursorPosition = Mouse.GetPosition(_gameCanvas);
                    offsetPosition.Offset(20, 25);
                    Vector direction = new Vector(cursorPosition.X - offsetPosition.X, cursorPosition.Y - offsetPosition.Y);
                    if (direction.Length > 0)
                        direction.Normalize();

                    Bullet bullet = new Bullet(_gameCanvas, playerPosition, direction);
                    bullets.Add(bullet);
                }
            }
        }

        private Enemy GetClosestEnemy()
        {
            double minDistance = double.MaxValue;
            Enemy closestEnemy = null;

            foreach (var enemy in _enemies)
            {
                double distance = CalculateDistance(playerPosition, enemy.enemyPosition);

                if (distance < minDistance && !enemy.isDead)
                {
                    minDistance = distance;
                    closestEnemy = enemy;
                }
            }

            return closestEnemy;
        }

        private double CalculateDistance(Point point1, Point point2)
        {
            double deltaX = point2.X - point1.X;
            double deltaY = point2.Y - point1.Y;

            // Use Pythagorean theorem to calculate the distance
            return Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
        }
        public void UpdateHP(double hp)
        {
            hpProgressBar.Value = hp;
        }
        public void PlayerDeath()
        {
            isFrozen = true;

            if (!isPlayerDead)
            {
                _gameCanvas.Children.Remove(playerContainer);
                _gameCanvas.Children.Remove(hpProgressBar);

                Uri imageUri = new Uri("assets/images/wizard_death.gif", UriKind.RelativeOrAbsolute);
                var image = new BitmapImage();
                image.BeginInit();
                image.UriSource = imageUri;
                image.EndInit();

                TransformGroup transformGroup = new TransformGroup();

                TranslateTransform deathOffset = new TranslateTransform(playerTransform.X, playerTransform.Y);
                deathOffset.X -= 115;
                deathOffset.Y -= 110;

                transformGroup.Children.Add(playerScaleTransform);
                transformGroup.Children.Add(deathOffset);

                playerSprite = new Image
                {
                    Width = 300,
                    Height = 300,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    RenderTransformOrigin = new Point(0.5, 0.5),
                    RenderTransform = transformGroup
                };

                ImageBehavior.SetAnimatedSource(playerSprite, image);
                _gameCanvas.Children.Add(playerSprite);
            }
            isPlayerDead = true;
        }

        public void GameOver()
        {
            DoubleAnimation deathAnimation = new DoubleAnimation
            {
                To = 0,
                Duration = TimeSpan.FromSeconds(2),
            };
            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(deathAnimation);
            Storyboard.SetTarget(deathAnimation, playerSprite);
            Storyboard.SetTargetProperty(deathAnimation, new PropertyPath(UIElement.OpacityProperty));

            storyboard.Completed += (sender, e) =>
            {
                _gameCanvas.Children.Remove(playerSprite);
                deathComplete = true;
            };
            storyboard.Begin();
        }

        // Инициализация хёртбокса игрока 
        private void AddPlayerHurtbox()
        {
            playerHurtbox = new Rectangle
            {
                Width = 40,
                Height = 50,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Bottom,
                // Невидимый хёртбокс, если откоментить,
                // то будет красный прямоугольник за спрайтом
                /*Fill = Brushes.Red,*/
                RenderTransformOrigin = new Point(0.5, 0.5),
                RenderTransform = playerTransform
            };
        }

        // Инициализация спрайта игрока
        public void AddPlayerSprite(int state)
        {
            Uri imageUri = new Uri("assets/images/wizard.gif", UriKind.RelativeOrAbsolute);
            var image = new BitmapImage();
            image.BeginInit();
            image.UriSource = imageUri;
            image.EndInit();

            TransformGroup transformGroup = new TransformGroup();
            transformGroup.Children.Add(playerScaleTransform);
            transformGroup.Children.Add(playerTransform);

            playerSprite = new Image
            {
                Width = 70,
                Height = 80,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                RenderTransformOrigin = new Point(0.5, 0.5),
                RenderTransform = transformGroup
            };
            if (state == 0)
            {
                ImageBehavior.SetAnimatedSource(playerSprite, image);
            }
            else
            {
                playerSprite.Source = image;
            }
        }

        private void AddHPBar(double initialHP)
        {
            hpProgressBar = new ProgressBar
            {
                Minimum = 0,
                Maximum = 100,
                Height = 10,
                Width = 50,
                Margin = new Thickness(10, 85, 0, 0),
                VerticalAlignment = VerticalAlignment.Bottom,
                Background = new SolidColorBrush(Colors.Gray),
                Foreground = new SolidColorBrush(Colors.Red),
                Value = initialHP,
                RenderTransformOrigin = new Point(0.5, 0.5),
                RenderTransform = playerTransform
            };
            _gameCanvas.Children.Add(hpProgressBar);
        }

        // Логика движения на WASD
        public void UpdatePlayerPosition(Canvas gameCanvas)
        {
            if (!isFrozen)
            {
                double moveX = 0;
                double moveY = 0;

                if (IsKeyDown(Key.W))
                    moveY -= 1;

                if (IsKeyDown(Key.S))
                    moveY += 1;

                if (IsKeyDown(Key.A))
                    moveX -= 1;

                if (IsKeyDown(Key.D))
                    moveX += 1;

                // Нормализование вектора
                // (чтобы диагональная скорость движения не была в ~1.4 больше)
                Vector movementVector = new Vector(moveX, moveY);
                if (movementVector.Length > 0)
                    movementVector.Normalize();

                double newPlayerX = playerPosition.X + movementVector.X * playerSpeed;
                double newPlayerY = playerPosition.Y + movementVector.Y * playerSpeed;

                // Не позволяет игроку выйти за пределы карты
                if (newPlayerX >= 0 && newPlayerX <= gameCanvas.ActualWidth)
                    playerPosition.X = newPlayerX;

                if (newPlayerY >= 0 && newPlayerY <= gameCanvas.ActualHeight)
                    playerPosition.Y = newPlayerY;

                playerTransform.X = playerPosition.X;
                playerTransform.Y = playerPosition.Y;

                UpdatePlayerSpriteFlipState(movementVector);
            }
        }

        // Логика поворота спрайта игрока
        private void UpdatePlayerSpriteFlipState(Vector movementVector)
        {
            if (movementVector.X < 0)
            {
                playerScaleTransform.ScaleX = -1;
            }
            else if (movementVector.X > 0)
            {
                playerScaleTransform.ScaleX = 1;
            }
        }

        private bool IsKeyDown(Key key)
        {
            return Keyboard.IsKeyDown(key);
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            KeyDown?.Invoke(sender, e);
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            KeyUp?.Invoke(sender, e);
        }
    }
}
