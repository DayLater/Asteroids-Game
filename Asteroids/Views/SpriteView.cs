using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using AsteroidsEngine;
using AsteroidsEngine.Entities;

namespace AsteroidsGame.Views
{
    public class SpriteView : IView
    {
        private Image playerImage;
        private Image asteroidImage;
        private Image smallAsteroidImage;
        private Image ufoImage;
        private Image bulletImage;
        private Image laserImage;
        private Image background;

        private readonly int width;
        private readonly int height;

        public SpriteView(int width, int height)
        {
            this.width = width;
            this.height = height;

            CreateAllSprites();
        }

        private void CreateAllSprites()
        {
            background = new Bitmap(Properties.Resources.background, width, height);
            playerImage = new Bitmap(Properties.Resources.PlayerShip, 30, 50);
            asteroidImage = new Bitmap(Properties.Resources.Meteor, 60, 60);
            smallAsteroidImage = new Bitmap(Properties.Resources.SmallMeteor, 30, 30);
            ufoImage = new Bitmap(Properties.Resources.ufo, 60, 40);
            bulletImage = new Bitmap(Properties.Resources.PlayerBullet, 2, 10);
            int maxLengthLaser = (int)Math.Sqrt(width * width + height * height);
            laserImage = new Bitmap(Properties.Resources.laser, 3, maxLengthLaser);
        }

        private void DrawEnemies(Graphics g, GameModel game, Matrix matrix)
        {
            foreach (var enemy in game.EnemySpawner)
            {
                if (enemy is Asteroid asteroid)
                    DrawObject(g, asteroid, asteroid.IsChild ? smallAsteroidImage : asteroidImage, matrix);
                else DrawObject(g, enemy, ufoImage, matrix);
            }
        }

        private void DrawObject(Graphics g, GameObject gameObject, Image image, Matrix matrix,
            float angle = 0)
        {
            g.Transform = matrix;
            g.TranslateTransform(gameObject.Position.X, gameObject.Position.Y);
            g.RotateTransform(angle);
            g.DrawImage(image, -image.Width / 2, -image.Height / 2);
        }

        private void DrawBullets(Graphics g, GameModel game, Matrix matrix)
        {
            foreach (var bullet in game.BulletsFolder)
                DrawObject(g, bullet, bulletImage, matrix, -bullet.Angle);
        }

        private void DrawPlayer(Graphics g, GameModel game, Matrix matrix)
        {
            DrawObject(g, game.Player, playerImage, matrix, -game.Player.Angle);
            DrawBullets(g, game, matrix);
            DrawLaser(g, game, matrix);
        }

        private void DrawLaser(Graphics g, GameModel game, Matrix matrix)
        {
            var laser = game.Player.Laser;
            var firstPoint = laser.Vectors.FirstOrDefault();
            if (firstPoint != null)
            {
                g.Transform = matrix;
                g.TranslateTransform(firstPoint.X, firstPoint.Y);
                g.RotateTransform(-game.Player.Angle);
                g.DrawImage(laserImage,
                    new Point(-laserImage.Width, -laserImage.Height));
            }
        }

        public void DrawFrame(Graphics g, GameModel game)
        {
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.DrawImage(background, new Point(0 , 0));
            var matrix = g.Transform;
            DrawPlayer(g, game, matrix);
            DrawEnemies(g, game, matrix);
        }
    }
}
