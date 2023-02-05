namespace Krakjam
{
    using UnityEngine;

    public class MonsterSounds : MonoBehaviour
    {
        #region Unity Methods
        private void Start()
        {
            _AudioSource = GetComponent<AudioSource>();
            _AudioSource.clip = GameBalance.MonsterSound;
            _AudioSource.loop = true;
        }

        #endregion Unity Methods

        #region Private Variables
        private AudioSource _AudioSource;
        #endregion Private Variables
    }
}