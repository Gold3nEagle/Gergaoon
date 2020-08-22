using SweetSugar.Scripts.Core;
using SweetSugar.Scripts.MapScripts;
using UnityEngine;
using UnityEngine.UI;

namespace SweetSugar.Scripts.GUI
{
    /// <summary>
    /// Background selector. Select different level background for every 20 levels
    /// </summary>
    public class BackgroundsForStaticMap : MonoBehaviour
    {
        public Sprite[] pictures;

        // Use this for initialization
        void OnEnable ()
        {
            {
                var backgroundSpriteNum = (int) (LevelsMap._instance.GetLastestReachedLevel() / 20f - 0.01f);
                if(pictures.Length > backgroundSpriteNum)
                    GetComponent<Image>().sprite = pictures[backgroundSpriteNum];
            }


        }


    }
}