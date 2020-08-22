using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SweetSugar.Scripts.Localization;
using UnityEngine;

namespace SweetSugar.Scripts.TargetScripts.TargetSystem
{
    /// <summary>
    /// target container keeps the object should be collected, its count, sprite, color
    /// </summary>
    [Serializable]
    [CreateAssetMenu(fileName = "TargetContainer", menuName = "TargetContainer", order = 1)]
    public class TargetContainer
    {
        public string name = "";

        // public TargetContainer target;
        public string Description = "";
        public int descriptionLocalizationRefrence;
        public string FailedDescription = "";
        public int failedLocalizationRefrence;

        public List<GameObject> prefabs = new List<GameObject>();

        public TargetContainer DeepCopy()
        {
            var other = (TargetContainer) MemberwiseClone();

            return other;
        }

        public string GetDescription()
        {
            return LocalizationManager.GetText(descriptionLocalizationRefrence, Description);
        }

        public string GetFailedDescription()
        {
            return LocalizationManager.GetText(failedLocalizationRefrence, FailedDescription);
        }
    }
}