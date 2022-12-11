using System;
using System.Collections.Generic;

using UnityEngine;

namespace Tausi.RubiksCube
{
    [CreateAssetMenu(menuName = "Rubik's Cube/History Collection")]
    public class HistoryCollection : ScriptableObject
    {
        [Serializable]
        public class Entry
        {
            public string name;
            public History history = new();
        }

        public List<Entry> entries = new();
    }
}