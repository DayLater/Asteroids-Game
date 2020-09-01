using System.Drawing;
using AsteroidsEngine;

namespace AsteroidsGame.Views
{
    public interface IView
    {
        void DrawFrame(Graphics g, GameModel game);
    }
}