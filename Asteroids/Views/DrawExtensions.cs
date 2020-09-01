using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using AsteroidsEngine;
using AsteroidsEngine.Entities;

namespace AsteroidsGame.Views
{
    public static class DrawExtensions
    {
        public static void DrawFigure(this IEnumerable<PointF> pointsToDraw, Graphics g, Pen pen)
        {
            var points = pointsToDraw.ToArray();
            g.DrawLines(pen, points);
            g.DrawLine(pen, points[points.Length - 1], points[0]);
        }

        public static void Draw(this Ufo ufo, Graphics g)
        {
            var centerToDraw = ufo.GetCenterToDraw();
            var headToDraw = ufo.GetHeadToDraw();

            g.DrawEllipse(Pens.DarkSeaGreen, headToDraw.X, headToDraw.Y,
                ufo.Body.Ry * 2, ufo.Body.Ry * 2);

            g.DrawEllipse(Pens.DeepSkyBlue, centerToDraw.X, centerToDraw.Y,
                ufo.Body.Rx * 2, ufo.Body.Ry * 2);
        }

        public static Vector GetCenterToDraw(this Ufo ufo)
        {
            return ufo.Position - new Vector(ufo.Body.Rx, ufo.Body.Ry);
        }

        public static Vector GetHeadToDraw(this Ufo ufo)
        {
            var centerToDraw = ufo.GetCenterToDraw();
            return new Vector(centerToDraw.X + ufo.Body.Ry,
                centerToDraw.Y - ufo.Body.Ry * 0.7f);
        }
    }
}