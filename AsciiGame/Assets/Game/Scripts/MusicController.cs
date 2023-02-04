namespace Krakjam
{
    using UnityEngine;

    public class MusicController : MonoSingleton<MusicController>
    {
        public static void StartAmbinet()
        {
            Instance.PlayAmbinet();
        }
        public static void StartMusic()
        {
            Instance.PlayMusic();
        }

        [SerializeField] private AudioSource AmbientSource;
        [SerializeField] private AudioSource MusicSource;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void PlayAmbinet()
        {
            StopMusic();

            if (AmbientSource.isPlaying) { return; };
            AmbientSource.Play();
        }
        private void PlayMusic()
        {
            StopAmbient();

            if (MusicSource.isPlaying) { return; };
            MusicSource.Play();
        }

        private void StopAmbient()
        {
            AmbientSource.Pause();
        }
        private void StopMusic()
        {
            MusicSource.Pause();
        }
    }
}