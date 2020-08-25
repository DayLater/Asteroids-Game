using System;
using System.Windows.Media;

namespace AsteroidsGame
{
    public class SoundPlayer
    {
        private readonly MediaPlayer shootSound;
        private readonly MediaPlayer laserSound;
        private readonly MediaPlayer enemyDeathSound;
        private readonly MediaPlayer playerDeathSound;
        private readonly MediaPlayer background;

        public SoundPlayer()
        {
            shootSound = new MediaPlayer();
            shootSound.Open(new Uri("PlayerShoot.wav", UriKind.Relative));

            laserSound = new MediaPlayer();
            laserSound.Open(new Uri("PlayerLaser.wav", UriKind.Relative));

            playerDeathSound = new MediaPlayer();
            playerDeathSound.Open(new Uri("PlayerDeath.wav", UriKind.Relative));

            enemyDeathSound = new MediaPlayer();
            enemyDeathSound.Open(new Uri("EnemyDeath.wav", UriKind.Relative));

            background = new MediaPlayer();
            background.Open(new Uri("background.mp3", UriKind.Relative));
            background.MediaEnded += (sender, args) => PlayBackgroundMusic();
        }

        private void PlaySound(MediaPlayer player)
        {
            player.Position = TimeSpan.Zero;
            player.Play();
        }

        public void PlayBackgroundMusic() => PlaySound(background);

        public void PlayEnemyDeath() => PlaySound(enemyDeathSound);

        public void PlayShoot() => PlaySound(shootSound);

        public void PlayLaser() => PlaySound(laserSound);

        public void PlayPlayerDeath() => PlaySound(playerDeathSound);

    }
}