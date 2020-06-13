using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerUps : MonoBehaviour
{

    public bool toggleBoost, destroyBoost, colorBombBoost;
    public int destroyBoostNum, colorBombBoostNum;

    public Button destroyBoostButton, colorBombBoostButton;
    public Text destroyNumText, colorBombNumText;

    // Start is called before the first frame update
    void Start()
    {

        //  PlayerPrefs.SetInt("ColorBombBoost", 100);
        //  PlayerPrefs.SetInt("DestroyBoost", 100);
        DisplayColorBombNum();
        DisplayDestroyNum();

        destroyBoostNum = GetDestroyBoostNum();
        colorBombBoostNum = GetColorBombBoostNum();

        if(destroyBoostNum == 0)
            destroyBoostButton.interactable = false;

        if (colorBombBoostNum == 0)
            colorBombBoostButton.interactable = false; 
    } 


    public void ColorBombBoost()
    {
        if (GetColorBombBoostNum() > 0)
        {
            toggleBoost = true;
            colorBombBoost = true;
        }
    }

    public void DestroyBoost()
    {
        if(GetDestroyBoostNum() > 0) { 
        toggleBoost = true;
        destroyBoost = true;
        }
    }

    public int GetDestroyBoostNum()
    {
        destroyBoostNum = PlayerPrefs.GetInt("DestroyBoost");

        return destroyBoostNum;
    }

    public int GetColorBombBoostNum()
    {
        colorBombBoostNum = PlayerPrefs.GetInt("ColorBombBoost");

        return colorBombBoostNum;
    }

    public void DisplayDestroyNum()
    {
        destroyNumText.text = GetDestroyBoostNum().ToString();
    }

    public void DisplayColorBombNum()
    {
        colorBombNumText.text = GetColorBombBoostNum().ToString();
    }

    public void DecreaseDestroyNum()
    {
        destroyBoostNum = GetDestroyBoostNum() - 1;
        PlayerPrefs.SetInt("DestroyBoost", destroyBoostNum);
        DisplayDestroyNum();
    }

    public void DecreaseColorNum()
    {
        colorBombBoostNum = GetColorBombBoostNum() - 1;
        PlayerPrefs.SetInt("ColorBombBoost", colorBombBoostNum);
        DisplayColorBombNum();
    }

}
