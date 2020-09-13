using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using AsteroidsEngine;
using AsteroidsEngine.Entities;

namespace AsteroidsGame.Views
{
    public class PolygonView : IView
    {
        private static readonly Random random = new Random();
        private readonly List<IEnumerable<Vector>> bigAsteroids;
        private readonly List<IEnumerable<Vector>> childAsteroids;

        public PolygonView()
        {
            bigAsteroids = new List<IEnumerable<Vector>>();
            childAsteroids = new List<IEnumerable<Vector>>();
            CreateAsteroids(bigAsteroids, 30, 5);
            CreateAsteroids(childAsteroids, 15, 5);
        }

        private void CreateAsteroids(ICollection<IEnumerable<Vector>> asteroidsPoints, int r, int count)
        {
            for (int i = 0; i < count; i++) 
                asteroidsPoints.Add(CreateAsteroidPoints(r));
        }

        private IEnumerable<Vector> CreateAsteroidPoints(int r)
        {
            var vectors = new Vector[12];
            for (var i = 0; i < 12; i++)
                vectors[i] = new Vector(0, random.Next(-3, 3) + r)
                    .Rotate(Vector.Zero, i * 30);
            return vectors;
        }

        private void DrawAsteroid(Asteroid asteroid, Graphics g)
        {
            var asteroids = asteroid.IsChild ? childAsteroids : bigAsteroids;
            var id = asteroid.GetHashCode();
            IEnumerable<Vector> points;
            if (id % 2 == 0)
                points = asteroids[0];
            else if (id % 3 == 0)
                points = asteroids[1];
            else if (id % 5 == 0)
                points = asteroids[2];
            else if (id % 7 == 0)
                points = asteroids[3];
            else points = asteroids[4];

            points.Select(v => (v + asteroid.Position).Rotate(asteroid.Position, asteroid.Angle))
                .Select(v => v.ToPointF)
                .DrawFigure(g, Pens.GhostWhite);
        }

        public void DrawFrame(Graphics g, GameModel game)
        {
            DrawPlayer(g, game);
            DrawEnemies(g, game);
        }

        private void DrawEnemies(Graphics g, GameModel game)
        {
            foreach (var enemy in game.EnemySpawner)
            {
                if (enemy is Asteroid asteroid)
                    DrawAsteroid(asteroid, g);
                else if (enemy is Ufo ufo)
                    DrawUfo(ufo, g);
            }
        }

        private void DrawPlayer(Graphics g, GameModel game)
        {
            //draw player
            game.Player.GetCoordinates().DrawFigure(g, Pens.White);

            //draw laser
            var laserPoints = game.Player.Laser.Vectors.ToList();
            if (laserPoints.Count != 0)
                laserPoints.Select(v => v.ToPointF)
                    .DrawFigure(g, Pens.CornflowerBlue);

            //Draw bullets
            foreach (var bullet in game.BulletsFolder)
                bullet.GetCoordinates().DrawFigure(g, Pens.Red);
        }

        private void DrawUfo(Ufo ufo, Graphics g)
        {
            var centerToDraw = ufo.GetCenterToDraw();
            var headToDraw = ufo.GetHeadToDraw();

            g.DrawEllipse(Pens.DarkSeaGreen, headToDraw.X, headToDraw.Y,
                ufo.Body.Ry * 2, ufo.Body.Ry * 2);

            g.DrawEllipse(Pens.DeepSkyBlue, centerToDraw.X, centerToDraw.Y,
                ufo.Body.Rx * 2, ufo.Body.Ry * 2);
        }
    }
}