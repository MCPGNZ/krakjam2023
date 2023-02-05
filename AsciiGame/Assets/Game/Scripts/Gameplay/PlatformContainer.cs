namespace Krakjam
{
    using System.Collections.Generic;
    using UnityEngine;
    using Random = UnityEngine.Random;

    [CreateAssetMenu(menuName = "Krakjam/Platform Container")]
    public sealed class PlatformContainer : ScriptableObject
    {
        public List<Platform> Default;
        public List<Platform> Advanced;

        public Platform Draw()
        {
            return Advanced[Random.Range(0, Advanced.Count)];
        }
    }
}