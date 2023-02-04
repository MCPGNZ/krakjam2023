namespace Krakjam
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(menuName = "Krakjam/SymbolList")]
    public class SymbolList : ScriptableObject
    {
        public string Characters;
        public int Resolution;
        public List<SymbolDefinition> Definitions;

        public void GenerateList()
        {
            throw new NotImplementedException();
        }
    }
}