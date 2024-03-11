using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ExamGame
{
    class Bullet
    {
        private double bulletSpeed = 8.0;
        public Point bulletPosition;
        public Ellipse bullet;
        private TranslateTransform bulletTransform;
        private Vector bulletDirection;

        public Bullet(Canvas gameCanvas, Point startPosition, Vector direction)
        {
            bulletPosition = startPosition;
            bulletPosition.Offset(20, 25);
            bulletDirection = direction;
            InitializeBullet(gameCanvas);
        }

        public void UpdateBulletPosition()
        {
            // Move the bullet in the specified direction
            bulletPosition.X += bulletDirection.X * bulletSpeed;
            bulletPosition.Y += bulletDirection.Y * bulletSpeed;

            // Update the bullet's actual position
            bulletTransform.X = bulletPosition.X;
            bulletTransform.Y = bulletPosition.Y;
        }

        public void KillEnemy(Enemy enemy)
        {
            enemy.EnemyDeath();
            enemy.UnloadEnemy();
        }

        private void InitializeBullet(Canvas gameCanvas)
        {
            bulletTransform = new TranslateTransform();
            bullet = new Ellipse
            {
                Width = 15,
                Height = 15,
                Fill = Brushes.Red,
                RenderTransform = bulletTransform
            };

            bulletTransform.X = bulletPosition.X;
            bulletTransform.Y = bulletPosition.Y;

            gameCanvas.Children.Add(bullet);
        }
    }
}

