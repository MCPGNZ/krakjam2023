namespace Krakjam
{
    using UnityEngine;

    public class MusicController : MonoSingleton<MusicController>
    {
        public static void PlayAmbient()
        {
            Instance.Internal_PlayAmbinet();
        }
        public static void PlayMusic()
        {
            Instance.Internal_PlayMusic();
        }
        public static void PlayButtonClick()
        {
            Instance.Internal_PlayButtonClick();
        }

        [SerializeField] private AudioSource AmbientSource;
        [SerializeField] private AudioSource MusicSource;
        [SerializeField] private AudioSource ButtonClickSource;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void Internal_PlayAmbinet()
        {
            StopMusic();

            if (AmbientSource.isPlaying) { return; };
            AmbientSource.Play();
        }
        private void Internal_PlayMusic()
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

        private void Internal_PlayButtonClick()
        {
            ButtonClickSource.PlayOneShot(ButtonClickSource.clip);
        }
    }
}