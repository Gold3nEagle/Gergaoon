using SweetSugar.Scripts.Core;
using SweetSugar.Scripts.Localization;
using TMPro;
using UnityEngine;

namespace SweetSugar.Scripts.GUI
{
    /// <summary>
    /// Moves / Time label in the game
    /// </summary>
    public class MovesLabel : MonoBehaviour
    {
        // Use this for initialization
        void OnEnable()
        {
            if(LevelManager.THIS?.levelData == null || !LevelManager.THIS.levelLoaded)
                LevelManager.OnLevelLoaded += Reset;
            else 
                Reset();
        }

        void OnDisable()
        {
            LevelManager.OnLevelLoaded -= Reset;
        }


    void Reset()
    {
        if (LevelManager.THIS != null && LevelManager.THIS.levelLoaded)
        {
            if (LevelManager.THIS.levelData.limitType == LIMIT.MOVES)
                GetComponent<TextMeshProUGUI>().text = LocalizationManager.GetText(41, GetComponent<TextMeshProUGUI>().text);
            else
                GetComponent<TextMeshProUGUI>().text = LocalizationManager.GetText(77, GetComponent<TextMeshProUGUI>().text);
        }

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
