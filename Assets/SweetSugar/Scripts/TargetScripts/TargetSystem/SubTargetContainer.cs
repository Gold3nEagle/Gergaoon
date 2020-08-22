using System;
using SweetSugar.Scripts.Blocks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SweetSugar.Scripts.TargetScripts.TargetSystem
{
    /// <summary>
    /// target container keeps the object should be collected, its count, sprite, color
    /// </summary>
    [Serializable]
    public class SubTargetContainer
    {
        ///using to keep count of targets
        public GameObject targetPrefab;
        public int count;
        public int preCount;
        public Object extraObject;
        public int color;
        public SubTargetContainer(GameObject _target, int _count, Object _extraObject)
        {
            targetPrefab = _target;
            count = _count;
            preCount = count;
            extraObject = _extraObject;
        }

        public void changeCount(int i)
        {
            count += i;
            if (count < 0) count = 0;
            preCount = count;
        }

        public int GetCount()
        {
            return count;
        }

        public void SetCount(int i)
        {
            count = i;
        }

        public bool IsTargetSquare()
        {
            return targetPrefab.GetComponent<Square>() != null || targetPrefab.GetComponentInChildren<Square>() != null;
        }

        public SubTargetContainer DeepCopy()
        {
            // SubTargetContainer other = (SubTargetContainer)this.MemberwiseClone();
            var other = new SubTargetContainer(targetPrefab, count, extraObject);


            return other;
        }
    }
}