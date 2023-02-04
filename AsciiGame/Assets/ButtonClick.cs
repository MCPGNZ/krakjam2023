namespace Krakjam
{
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(AudioSource))]
    public class ButtonClick : MonoBehaviour
    {
        public AudioClip Hover;
        public AudioClip Click;

        private void Awake()
        {
            _Button = GetComponent<Button>();
            _Button.onClick.AddListener(MusicController.PlayButtonClick);

            _Source = GetComponent<AudioSource>();
            _Source.playOnAwake = false;
        }

        private Button _Button;
        private AudioSource _Source;
    }
}