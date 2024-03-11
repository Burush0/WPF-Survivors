using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfAnimatedGif;
using Vector = System.Windows.Vector;

namespace ExamGame
{
    internal class Enemy
    {
        private readonly double enemySpeed = 2.0;
        private double spawnRadius = 300;
        private const double сollisionCorrectionSpeed = 0.1;
        private const double сloseEnoughDistance = 10.0;

        private bool isFrozen = false;
        public bool isDead = false;

        public Point enemyPosition;
        private TranslateTransform enemyTransform;
        private ScaleTransform enemyScaleTransform;
        public Image enemySprite;
        private Rectangle enemyHurtbox;
        public Grid enemyContainer;

        private Canvas _gameCanvas;
        private Player _player;

        public Enemy(Canvas gameCanvas, Player player)
        {
            _gameCanvas = gameCanvas;
            _player = player;
            InitializeEnemy();
        }

        private void InitializeEnemy()
        {
            enemyTransform = new TranslateTransform();
            enemyScaleTransform = new ScaleTransform();

            AddEnemySprite(0);
            AddEnemyHurtbox();

            // Контейнер для хёртбокса и спрайта
            enemyContainer = new Grid();
            enemyContainer.Children.Add(enemyHurtbox);
            enemyContainer.Children.Add(enemySprite);

            enemyPosition = GenerateRandomPointAroundPlayer(_player.playerPosition);

            enemyTransform.X = enemyPosition.X;
            enemyTransform.Y = enemyPosition.Y;

            enemyContainer.HorizontalAlignment = HorizontalAlignment.Center;
            enemyContainer.VerticalAlignment = VerticalAlignment.Center;
            _gameCanvas.Children.Add(enemyContainer);
        }

        // Рандомный спавн вокруг игрока в радиусе 300
        private Point GenerateRandomPointAroundPlayer(Point playerPosition)
        {
            Random random = new Random();

            double angle = random.NextDouble() * 2 * Math.PI;

            double offsetX = Math.Cos(angle) * spawnRadius;
            double offsetY = Math.Sin(angle) * spawnRadius;

            double enemyX = playerPosition.X + offsetX;
            double enemyY = playerPosition.Y + offsetY;

            return new Point(enemyX, enemyY);
        }

        public void EnemyDeath()
        {
            isFrozen = true;
            if (!isDead)
            {
                _gameCanvas.Children.Remove(enemyContainer);

                Uri imageUri = new Uri("assets/images/skeleton_death.gif", UriKind.RelativeOrAbsolute);
                var image = new BitmapImage();
                image.BeginInit();
                image.UriSource = imageUri;
                image.EndInit();

                TransformGroup transformGroup = new TransformGroup();
                transformGroup.Children.Add(enemyScaleTransform);
                transformGroup.Children.Add(enemyTransform);

                enemySprite = new Image
                {
                    Width = 90,
                    Height = 100,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    RenderTransformOrigin = new Point(0.5, 0.5),
                    RenderTransform = transformGroup
                };

                ImageBehavior.SetAnimatedSource(enemySprite, image);
                _gameCanvas.Children.Add(enemySprite);
            }
            isDead = true;
        }

        public void UnloadEnemy()
        {
            DoubleAnimation deathAnimation = new DoubleAnimation
            {
                To = 0,
                Duration = TimeSpan.FromSeconds(0.8),
            };
            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(deathAnimation);
            Storyboard.SetTarget(deathAnimation, enemySprite);
            Storyboard.SetTargetProperty(deathAnimation, new PropertyPath(UIElement.OpacityProperty));

            storyboard.Completed += (sender, e) =>
            {
                _gameCanvas.Children.Remove(enemySprite);
            };
            storyboard.Begin();
        }
        public void AddEnemySprite(int state)
        {
            Uri imageUri = new Uri("assets/images/skeleton.gif", UriKind.RelativeOrAbsolute);
            var image = new BitmapImage();
            image.BeginInit();
            image.UriSource = imageUri;
            image.EndInit();

            TransformGroup transformGroup = new TransformGroup();
            transformGroup.Children.Add(enemyScaleTransform);
            transformGroup.Children.Add(enemyTransform);

            enemySprite = new Image
            {
                Width = 90,
                Height = 100,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                RenderTransformOrigin = new Point(0.5, 0.5),
                RenderTransform = transformGroup
            };
            if (state == 0)
            {
                ImageBehavior.SetAnimatedSource(enemySprite, image);
            }
            else
            {
                enemySprite.Source = image;
            }
        }

        private void AddEnemyHurtbox()
        {
            enemyHurtbox = new Rectangle
            {
                Width = 40,
                Height = 80,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                // Невидимый хёртбокс, если откоментить,
                // то будет красный прямоугольник за спрайтом
                /*Fill = Brushes.Red,*/
                RenderTransform = enemyTransform
            };
        }

        // Враги бегут в сторону игрока
        public void UpdateEnemyPosition()
        {

            Vector movementVector = new Vector(_player.playerPosition.X - enemyPosition.X, _player.playerPosition.Y - enemyPosition.Y);

            // Перестаёт движение если достаточно близко к игроку
            if (movementVector.Length > сloseEnoughDistance && !isFrozen)
            {
                // Нормализование вектора
                // (чтобы диагональная скорость движения не была в ~1.4 больше)
                if (movementVector.Length > 0)
                    movementVector.Normalize();

                double newEnemyX = enemyPosition.X + movementVector.X * enemySpeed;
                double newEnemyY = enemyPosition.Y + movementVector.Y * enemySpeed;

                if (movementVector.X <= 3)
                {
                    enemyPosition.X = newEnemyX;
                }
                if (movementVector.Y <= 3)
                {
                    enemyPosition.Y = newEnemyY;
                }

                enemyTransform.X = enemyPosition.X;
                enemyTransform.Y = enemyPosition.Y;

                UpdateEnemySpriteFlipState(movementVector);
            }
        }

        // Проверка столкновения
        public bool IsCollidingWith(Enemy otherEnemy)
        {
            Rect enemyBounds = new Rect(enemyPosition.X, enemyPosition.Y, enemyHurtbox.Width, enemyHurtbox.Height);
            Rect otherBounds = new Rect(otherEnemy.enemyPosition.X, otherEnemy.enemyPosition.Y, otherEnemy.enemyHurtbox.Width, otherEnemy.enemyHurtbox.Height);

            return enemyBounds.IntersectsWith(otherBounds);
        }

        // Логика предотвращения столкновения
        public void ResolveCollisionWith(Enemy otherEnemy)
        {
            Vector collisionNormal = new Vector(otherEnemy.enemyPosition.X - enemyPosition.X, otherEnemy.enemyPosition.Y - enemyPosition.Y);
            if (collisionNormal.Length > 0)
                collisionNormal.Normalize();

            double overlap = (enemyHurtbox.Width + otherEnemy.enemyHurtbox.Width) / 2.0;
            double moveDistance = overlap / 2.0;

            enemyPosition.X -= collisionNormal.X * moveDistance * сollisionCorrectionSpeed;
            enemyPosition.Y -= collisionNormal.Y * moveDistance * сollisionCorrectionSpeed;

            otherEnemy.enemyPosition.X += collisionNormal.X * moveDistance * сollisionCorrectionSpeed;
            otherEnemy.enemyPosition.Y += collisionNormal.Y * moveDistance * сollisionCorrectionSpeed;
        }

        // Логика поворота спрайта врага
        private void UpdateEnemySpriteFlipState(Vector movementVector)
        {
            if (movementVector.X < 0)
            {
                enemyScaleTransform.ScaleX = -1;
            }
            else if (movementVector.X > 0)
            {
                enemyScaleTransform.ScaleX = 1;
            }
        }
    }
}
