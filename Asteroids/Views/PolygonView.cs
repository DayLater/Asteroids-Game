using System.Drawing;
using System.Linq;
using AsteroidsEngine;
using AsteroidsEngine.Entities;

namespace AsteroidsGame.Views
{
    public class PolygonView : IView
    {
        public void DrawFrame(Graphics g, GameModel game)
        {
            DrawPlayer(g, game);
            DrawEnemies(g, game);
        }

        public void SetSettings() { }

        private void DrawEnemies(Graphics g, GameModel game)
        {
            foreach (var enemy in game.EnemySpawner)
            {
                if (enemy is Asteroid asteroid)
                    asteroid.GetCoordinates().DrawFigure(g, Pens.GhostWhite);
                else if (enemy is Ufo ufo)
                    ufo.Draw(g);
            }
        }

        private void DrawPlayer(Graphics g, GameModel game)
        {
            //draw player
            game.Player.GetCoordinates().DrawFigure(g, Pens.White);

            //draw laser
            var laserPoints = game.Player.Laser.GetPoints();
            if (laserPoints.Length != 0)
                laserPoints
                    .Select(v => v.ToPointF)
                    .ToArray()
                    .DrawFigure(g, Pens.CornflowerBlue);

            //Draw bullets
            foreach (var bullet in game.Bullets)
                bullet.GetCoordinates().DrawFigure(g, Pens.Red);
        }
    }
}