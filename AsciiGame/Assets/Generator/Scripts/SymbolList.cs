namespace Krakjam
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(menuName = "Krakjam/SymbolList")]
    public class SymbolList : ScriptableObject
    {
        public string Characters;
        public List<SymbolDefinition> Definitions;

        public void GenerateList()
        {
            throw new NotImplementedException();
        }
    }
}