using System;
using SweetSugar.Scripts.Items;
using UnityEngine;

namespace SweetSugar.Scripts.System.Combiner
{
    /// <summary>
    /// item for combine matrix
    /// </summary>
    [Serializable]
    public class ItemTemplate
    {
        public Vector2 position;
        public bool item;

        public float angleToNode, distanceToNode;
        // public int color;

        public Item itemRef;

        public ItemTemplate(Vector2 vector, bool _item/* , int c, Item _item */)
        {
            position = vector;
            item = _item;
            // color = c;
            // item = _item;
        }

        public ItemTemplate DeepCopy()
        {
            var other = (ItemTemplate)MemberwiseClone();
            return other;
        }

    }
}