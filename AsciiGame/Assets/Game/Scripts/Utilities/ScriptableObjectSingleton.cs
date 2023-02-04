namespace Krakjam
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using UnityEngine;

    public abstract class ScriptableObjectSingleton : ScriptableObject
    {
        #region Private Methods
        protected static Dictionary<Type, ScriptableObjectSingleton> _Cache;
        #endregion Private Methods
    }

    public class ScriptableObjectSingleton<T> : ScriptableObjectSingleton where T : ScriptableObjectSingleton
    {
        #region Public Variables
        public static T Instance
        {
            get
            {
                if (_Instance != null) { return _Instance; }

                _Instance = InitializeFromCache();
                return _Instance;
            }
        }
        #endregion Public Variables

        #region Private Variables
        private static T _Instance;
        #endregion Private Variables

        #region Private Methods
        private static T InitializeFromCache()
        {
            _Cache ??= CreateCache();
            if (_Cache.TryGetValue(typeof(T), out var result) == false)
            {
                throw new InvalidDataException(typeof(T).Name + " scriptable object not found");
            }

            return (T)result;
        }

        private static Dictionary<Type, ScriptableObjectSingleton> CreateCache()
        {
            var resources = Resources.LoadAll<ScriptableObjectSingleton>(string.Empty);
            var count = resources.Length;

            var result = new Dictionary<Type, ScriptableObjectSingleton>(count);
            for (int i = 0; i < count; ++i)
            {
                var current = resources[i];
                var type = current.GetType();

                if (result.ContainsKey(type))
                {
                    throw new InvalidDataException("More than one" + typeof(T).Name + " scriptable object found");
                }
                result.Add(current.GetType(), current);
            }

            return result;
        }
        #endregion Private Methods
    }
}