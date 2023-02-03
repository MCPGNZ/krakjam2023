namespace Krakjam
{
    using Sirenix.OdinInspector;
    using UnityEngine;

    public sealed class SymbolDefinition : ScriptableObject
    {
        public string Character;
        public Texture2D Texture;

        public float Left;
        public float Right;
        public float Top;
        public float Bottom;

        public float Average;

        [Button]
        public void Update()
        {
            SymbolGenerator.CalculateDefinition(this);
        }
    }
}