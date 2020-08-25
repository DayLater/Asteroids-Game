using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using AsteroidsEngine;
using AsteroidsEngine.Entities;
using AsteroidsGame.Views;

namespace AsteroidsGame
{
    public class Game : Form
    {
        private readonly Label scoreLabel = new Label();
        private readonly Label reloadLabel = new Label();
        private readonly Label gameOverLabel = new Label();
        private readonly Label StartLabel = new Label();
        private readonly Label configureLabel = new Label();

        private readonly GameModel game;
        private readonly Timer timer;

        private readonly IView polygonView;
        private readonly IView spriteView;
        private IView currentView;

        private SoundPlayer soundPlayer;
        private State currentState;

        public Game(int width, int height)
        {
            ConfigureWindow(width, height);

            game = new GameModel(width, height);
            timer = new Timer { Interval = 20 };

            polygonView = new PolygonView();
            spriteView = new SpriteView(width, height);
            polygonView.SetSettings();
            spriteView.SetSettings();
            soundPlayer = new SoundPlayer();

            timer.Tick += (sender, args) => TimerTick();
            game.GameOver += GameOver;
            game.OnEnemyDeath += ChangeScore;
            KeyDown += KeysDown;
            KeyUp += UpKeys;

            CreateAllLabels();
            currentView = polygonView;
            currentState = State.Menu;
            soundPlayer.PlayBackgroundMusic();
        }

        private void ConfigureWindow(int width, int height)
        {
            MaximumSize = new Size(width, height);
            MinimumSize = MaximumSize;
            Height = height;
            Width = width;
            DoubleBuffered = true;
        }

        private void Restart()
        {
            gameOverLabel.Visible = false;
            game.StartGame();
            currentState = State.Game;
            timer.Start();
        }

        #region Labels
        private void CreateAllLabels()
        {
            CreateStartLabel();
            CreateScoreLabel();
            CreateReloadLabel();
            CreateGameOverLabel();
            CreateConfigureLabel();
        }


        private void CreateLabel(Label label, Point location, Size size, Font font, string text, Color color)
        {
            label.Location = location;
            label.Size = size;
            label.Font = font;
            label.TextAlign = ContentAlignment.MiddleCenter;
            label.BackColor = color;
            label.Text = text;
            label.Visible = false;
            Controls.Add(label);
        }

        private void CreateConfigureLabel()
        {
            var text = "USE ARROWS TO TURN AND SPEED UP \n X - TO SHOOT \n Z - TO ACTIVATE LASER";
            var location = new Point(Width / 8, Height / 8);
            var size = new Size(Width / 4 * 3, Height / 4 * 3);
            CreateLabel(configureLabel, location, size, 
                new Font(FontFamily.GenericMonospace, 25), text, Color.AliceBlue);
        }

        private void CreateStartLabel()
        {
            var location = new Point(Width / 4, Height / 4);
            var size = new Size(Width / 2, Height / 2);
            var font = new Font(FontFamily.GenericMonospace, 30);
            var text = "ASTEROIDS\nPRESS SPACE TO\nSTART GAME";
            CreateLabel(StartLabel, location, size, font, text, Color.AliceBlue);
            StartLabel.Visible = true;
        }

        private void CreateScoreLabel()
        {
            CreateLabel(scoreLabel, new Point(0,0), new Size(120, 20), new Font(FontFamily.GenericMonospace, 10),
                "Score: 0", Color.Transparent);
            scoreLabel.TextAlign = ContentAlignment.TopLeft;
            scoreLabel.ForeColor = Color.White;
            scoreLabel.Visible = true;
        }

        private void CreateReloadLabel()
        {
            Laser.OnReady += () => reloadLabel.Text = "Ready!";
            Laser.OnReload += timeToReload => reloadLabel.Text = "Reload: " + Math.Round(timeToReload) + "%";
            CreateLabel(reloadLabel, new Point(Width - 120, 0), new Size(120, 20),
                new Font(FontFamily.GenericMonospace, 10), "Reload", Color.Transparent);
            reloadLabel.ForeColor = Color.White;
            reloadLabel.Visible = true;
        }

        private void CreateGameOverLabel()
        {
            CreateLabel(gameOverLabel, new Point(Width / 2 - 200, Height / 2 - 100), new Size(400, 200), 
                new Font(FontFamily.GenericMonospace, 30), "Game over \nYour Score: " + game.Score + "\nRestart?", Color.AliceBlue);
        }

        #endregion

        private void ChangeScore()
        {
            if (currentState == State.Game)
            {
                soundPlayer.PlayEnemyDeath();
                scoreLabel.Text = "Score: " + game.Score;
            }
        }

        private void GameOver()
        {
            soundPlayer.PlayPlayerDeath();
            timer.Stop();
            gameOverLabel.Visible = true;
            gameOverLabel.Text = "Game over\nYour Score: " + game.Score + "\nR to Restart";
            currentState = State.GameOver;
        }

        private void TimerTick()
        {
            if (isTurnLeft)
                game.Player.Turn(4);
            else if (isTurnRight)
                game.Player.Turn(-4);
            if (isSpeedUp)
                game.Player.SpeedUp();
            game.Update();
            Invalidate();
            Update();
        }


        #region GameControllKeys

        private bool isTurnLeft;
        private bool isTurnRight;
        private bool isSpeedUp;

        private void UpKeys(object sender, KeyEventArgs args)
        {
            if (args.KeyData == Keys.Left)
                isTurnLeft = false;
            if (args.KeyData == Keys.Right)
                isTurnRight = false;
            if (args.KeyData == Keys.Up)
                isSpeedUp = false;
            if (args.KeyData == Keys.X && currentState == State.Game)
            {
                soundPlayer.PlayShoot();
                game.MakeShoot();
            }
        }

        private void ControlKeysOnStartMenu(Keys key)
        {
            if (key == Keys.Space)
            {
                StartLabel.Visible = false;
                currentState = State.Configure;
                configureLabel.Visible = true;
            }
        }

        private void ControlKeysOnConfigureMenu(Keys key)
        {
            if (key == Keys.Space)
            {
                configureLabel.Visible = false;
                currentState = State.Game;
                timer.Start();
            }
        }

        private void ControlKeysOnGame(Keys key)
        {
            if (key == Keys.Left)
                isTurnLeft = true;
            else if (key == Keys.Right)
                isTurnRight = true;
            if (key == Keys.Up)
                isSpeedUp = true;
            if (key == Keys.Z)
            {
                var isActive = game.Player.TryActivateLaser();
                if (isActive)
                    soundPlayer.PlayLaser();
            }
            if (key == Keys.E)
                currentView = currentView == polygonView ? spriteView : polygonView;
        }

        private void KeysDown(object sender, KeyEventArgs args)
        {
            if (args.KeyData == Keys.Escape)
                Application.Exit();
            if (currentState == State.Menu)
                ControlKeysOnStartMenu(args.KeyData);
            else if (currentState == State.Configure)
                ControlKeysOnConfigureMenu(args.KeyData);
            else if (currentState == State.Game)
               ControlKeysOnGame(args.KeyData);
            else if (currentState == State.GameOver && args.KeyData == Keys.R)
                Restart();
        }

        #endregion

        protected override void OnPaint(PaintEventArgs e)
        {
            var image = new Bitmap(ClientRectangle.Width, ClientRectangle.Height, PixelFormat.Format32bppArgb);
            var g = Graphics.FromImage(image);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.FillRectangle(Brushes.Black, ClientRectangle);
            currentView.DrawFrame(g, game);
            e.Graphics.DrawImage(image, (ClientRectangle.Width - image.Width) / 2,
                (ClientRectangle.Height - image.Height) / 2);
        }
    }
}
