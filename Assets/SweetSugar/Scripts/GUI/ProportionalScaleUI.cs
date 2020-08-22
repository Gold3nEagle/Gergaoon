using System.Linq;
using SweetSugar.Scripts.System;
using UnityEngine;
using UnityEngine.UI;

namespace SweetSugar.Scripts.GUI
{
    /// <summary>
    /// Proportional scale of icon
    /// </summary>
    [ExecuteInEditMode]
    public class ProportionalScaleUI : MonoBehaviour
    {
        float side = 200;
        public GridLayoutGroup rect;

        // Use this for initialization
        void OnEnable()
        {
            rect = GetComponent<GridLayoutGroup>();
        }

        // Update is called once per frame
        private void Update()
        {
            var count = transform.GetChildren().Where(i => i.gameObject.activeSelf).Count();
            if (count == 4) side = 150;
            else if (count > 4) side = 130;
            else if (count == 3) side = 150;
            else if (count < 3) side = 280f / count;

            rect.cellSize = Vector2.one * side;
        }
    }
}
