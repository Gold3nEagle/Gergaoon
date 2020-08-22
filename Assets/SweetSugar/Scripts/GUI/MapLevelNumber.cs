using SweetSugar.Scripts.MapScripts;
using TMPro;
using UnityEngine;

namespace SweetSugar.Scripts.GUI
{
    /// <summary>
    /// Level number handler on the map
    /// </summary>
    public class MapLevelNumber : MonoBehaviour
    {
        private GameObject canvasMap;
        MapLevel mapLevel;
        Transform pin;
        // Use this for initialization
        void OnEnable()
        {
            canvasMap = GameObject.Find("WorldCanvas");
            mapLevel = transform.parent.parent.GetComponent<MapLevel>();
            if (transform.parent.gameObject == canvasMap) return;
            int num = mapLevel.Number;
            GetComponent<TextMeshProUGUI>().text = "" + num;
            pin = transform.parent;
            transform.SetParent(canvasMap.transform, true);
        }

        // Update is called once per frame
        void Update()
        {
            if (mapLevel != null && mapLevel.IsLocked && gameObject.activeSelf) gameObject.SetActive(false);
            else gameObject.SetActive(true);
        }

    }
}
