using System.Collections.Generic;
using UnityEngine;

namespace SweetSugar.Scripts.TargetScripts.TargetSystem
{
    /// <summary>
    /// Target editor
    /// </summary>
    public class TargetEditorScriptable : ScriptableObject
    {
        public List<TargetContainer> targets = new List<TargetContainer>();

    }
}