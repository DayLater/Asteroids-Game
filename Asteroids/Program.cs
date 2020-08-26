using System;
using System.Windows.Forms;

namespace AsteroidsGame
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Game(800, 600));
        }
    }
}