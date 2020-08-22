using UnityEngine;

namespace SweetSugar.Scripts.Items
{
    /// <summary>
    /// Sorting order handler for time bomb
    /// </summary>
    public class BindSortingOrder : UnityEngine.MonoBehaviour
    {
        public SpriteRenderer sourceObject;
        public Canvas destObject;
        public SpriteRenderer destObjectSR;
        public int offset;
        private void Update()
        {
            if(destObject){
            destObject.sortingLayerID = sourceObject.sortingLayerID;
            destObject.sortingOrder = sourceObject.sortingOrder + offset;}
            if(destObjectSR){
            destObjectSR.sortingLayerID = sourceObject.sortingLayerID;
            destObjectSR.sortingOrder = sourceObject.sortingOrder + offset;}
        }
    }
}