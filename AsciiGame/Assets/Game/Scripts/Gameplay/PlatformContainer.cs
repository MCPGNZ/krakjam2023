namespace Krakjam
{
    using System.Collections.Generic;
    using UnityEngine;
    using Random = UnityEngine.Random;

    [CreateAssetMenu(menuName = "Krakjam/Platform Container")]
    public sealed class PlatformContainer : ScriptableObject
    {
        public List<Platform> Platforms;
        public Platform Draw()
        {
            return Platforms[Random.Range(0, Platforms.Count)];
        }
    }
}