using System.Windows.Media;

namespace CapstoneProg3.utils
{
    public static class SoundManager
    {
        private static MediaPlayer backgroundMusicPlayer = new MediaPlayer();

        // Reproduzir música de fundo
        public static void PlayBackgroundMusic(string filePath)
        {
            backgroundMusicPlayer.Open(new Uri(filePath, UriKind.RelativeOrAbsolute));
            backgroundMusicPlayer.MediaEnded += (s, e) => backgroundMusicPlayer.Position = TimeSpan.Zero;
            backgroundMusicPlayer.Play();
        }

        // Reproduzir efeito sonoro
        public static void PlaySoundEffect(string filePath)
        {
            MediaPlayer soundEffectPlayer = new MediaPlayer();
            soundEffectPlayer.Open(new Uri(filePath, UriKind.RelativeOrAbsolute));
            soundEffectPlayer.Play();
        }

        // Parar a música de fundo
        public static void StopBackgroundMusic()
        {
            backgroundMusicPlayer.Stop();
        }
    }
}