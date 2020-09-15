using UnityEditor;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[ExecuteInEditMode]
public class Anchor : MonoBehaviour
{
    //public bool toggleAnchor;

    //private void Start()
    //{
    //    toggleAnchor = false;
    //}

    //private void Update()
    //{

    //    if (toggleAnchor == true)
    //    {
    //        uguianchoraroundobject();
    //        Debug.Log("Anchoring placements");
    //    }
    //}

    [MenuItem("UI/Anchor Around Object")]
    static void uguianchoraroundobject()
    {
        var o = Selection.activeGameObject;
        if (o != null && o.GetComponent<RectTransform>() != null)
        {
            var r = o.GetComponent<RectTransform>();
            var p = o.transform.parent.GetComponent<RectTransform>();

            var offsetmin = r.offsetMin;
            var offsetmax = r.offsetMax;
            var _anchormin = r.anchorMin;
            var _anchormax = r.anchorMax;

            var parent_width = p.rect.width;
            var parent_height = p.rect.height;

            var anchormin = new Vector2(_anchormin.x + (offsetmin.x / parent_width),
                                        _anchormin.y + (offsetmin.y / parent_height));
            var anchormax = new Vector2(_anchormax.x + (offsetmax.x / parent_width),
                                        _anchormax.y + (offsetmax.y / parent_height));

            r.anchorMin = anchormin;
            r.anchorMax = anchormax;

            r.offsetMin = new Vector2(0, 0);
            r.offsetMax = new Vector2(0, 0);
            r.pivot = new Vector2(0.5f, 0.5f);

        }
    }


}
