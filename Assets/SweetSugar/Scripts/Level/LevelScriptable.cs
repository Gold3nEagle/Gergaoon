using System;
using System.Collections.Generic;
using SweetSugar.Scripts.Core;
using SweetSugar.Scripts.TargetScripts.TargetSystem;
using UnityEngine;

namespace SweetSugar.Scripts.Level
{
    public class LevelScriptable : ScriptableObject
    {
        public List<LevelData> levels = new List<LevelData>();

        [Serializable]
        public class LevelKeeper
        {
            public int levelNum;
            public List<FieldData> fields = new List<FieldData>();
            [SerializeField]        public TargetContainer target;
            [SerializeField]        public Target targetObject;
            public LIMIT limitType;
            public int limit = 25;
            public int colorLimit = 5;
            public int star1 = 100;
            public int star2 = 300;
            public int star3 = 500;
            public bool enableMarmalade;
            public int maxRows;
            public int maxCols;
            public int currentSublevelIndex;
            public List<SubTargetContainer> subTargetsContainers = new List<SubTargetContainer>();


        }
    }
}