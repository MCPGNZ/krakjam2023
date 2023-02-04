namespace Krakjam
{
    using UnityEngine;

    public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        #region Public Variables
        public static T Instance
        {
            get
            {
                if (_Instance != null) { return _Instance; }

                _Instance = FindObjectOfType<T>(true);
                if (_Instance != null) { return _Instance; }

                if (_Quiting) { return null; }

                var obj = new GameObject("MonoSingleton<" + typeof(T).Name + ">")
                {
                };

                _Instance = obj.AddComponent<T>();

                return _Instance;
            }
        }
        #endregion Public Variables

        #region Unity Methods
        protected virtual void OnApplicationQuit()
        {
            _Quiting = true;
        }
        protected virtual void OnDestroy()
        {
            _Quiting = true;
        }
        #endregion Unity Methods

        #region Private Variables
        protected static T _Instance;
        private static bool _Quiting;
        #endregion Private Variables
    }
}