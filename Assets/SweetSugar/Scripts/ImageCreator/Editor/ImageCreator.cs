using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace SweetSugar.Scripts.ImageCreator.Editor
{
    [InitializeOnLoad]
    public class ImageCreator : UnityEditor.Editor
    {
        static ImageCreator()
        {
            EditorApplication.hierarchyWindowChanged += OnChanged;
        }

        private static void OnChanged()
        {
            var obj = Selection.activeGameObject;
            if (obj == null || obj.transform.parent == null) return;
            if ((obj.transform.parent.GetComponent<CanvasRenderer>() != null || obj.transform.parent.GetComponent<Canvas>() != null || obj.transform.parent.GetComponent<RectTransform>() != null) && obj.GetComponent<SpriteRenderer>() != null)
            {
                var rectTransform = obj.AddComponent<RectTransform>();
                rectTransform.anchoredPosition3D = Vector3.zero;
                rectTransform.localScale = Vector3.one;
                var spr = obj.GetComponent<SpriteRenderer>().sprite;
                var img = obj.AddComponent<Image>();
                img.sprite = spr;
                img.SetNativeSize();
                DestroyImmediate(obj.GetComponent<SpriteRenderer>());
            }
        }
    }
}