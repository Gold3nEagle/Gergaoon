using System.Linq;
using SweetSugar.Scripts.Core;
using SweetSugar.Scripts.Level;
using SweetSugar.Scripts.System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SweetSugar.Scripts.GUI
{
    /// <summary>
    /// Target icon in the MenuPlay
    /// </summary>
    public class MenuTargetIcon : MonoBehaviour
    {
        public Image image;
        public TextMeshProUGUI description;
        private Image[] images;

        private void Awake()
        {
            images = transform.GetChildren().Select(i => i.GetComponent<Image>()).ToArray();
        }

    void OnEnable()
    {
        DisableImages();
        var levelData = new LevelData(Application.isPlaying, LevelManager.THIS.currentLevel);
        levelData = LoadingManager.LoadForPlay(PlayerPrefs.GetInt("OpenLevel"), levelData);
        var list = levelData.GetTargetSpritePrefab();
        description.text = levelData.target.GetDescription();
        for (int i = 0; i < list.Length; i++)
        {
            images[i].sprite = list[i];
            images[i].gameObject.SetActive(true);
        }
        // Debug.Log(list.Length);
        // if (list.Length > 0)
        // {
        //     if (list?[0] != null)
        //     {
        //         image.sprite = list[0];
        //         // image.SetNativeSize();
        //     }
        // }
        // if (list.Length > 1)
        // {
        //     if (list[1] != null)
        //     {
        //         var obj = Instantiate(image.gameObject);
        //         obj.transform.SetParent(image.transform.parent);
        //         obj.transform.localScale = Vector3.one;
        //         obj.GetComponent<Image>().sprite = list[1];
        //         // obj.GetComponent<Image>().SetNativeSize();
        //     }
        // }
    }

        private void DisableImages()
        {
            foreach (var item in images)
            {
                item.gameObject.SetActive(false);
            }
        }

        void OnDisable()
        {
            // for (var i = transform.childCount - 1; i >= 1; i--)
            // {
            //     Destroy(transform.GetChild(i).gameObject);
            // }
        }
    }
}
