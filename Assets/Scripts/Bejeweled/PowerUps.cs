using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerUps : MonoBehaviour
{

    public bool toggleBoost, destroyBoost, colorBombBoost, adjacentBombBoost;
    public int destroyBoostNum, colorBombBoostNum, adjacentBombBoostNum;

    public Button destroyBoostButton, colorBombBoostButton, adjacentBombBoostButton;
    public Text destroyNumText, colorBombNumText, adjacentBombNumText;
    public GameObject instructionText;

    // Start is called before the first frame update
    void Start()
    { 
        destroyBoostNum = GetDestroyBoostNum();
        colorBombBoostNum = GetColorBombBoostNum();
        adjacentBombBoostNum = GetAdjacentBoostNum();

        DisplayColorBombNum();
        DisplayDestroyNum();
        DisplayAdjacentBombNum();

        if (destroyBoostNum == 0)
            destroyBoostButton.interactable = false;

        if (colorBombBoostNum == 0)
            colorBombBoostButton.interactable = false;

        if(adjacentBombBoostNum == 0)
        {
            adjacentBombBoostButton.interactable = false;
        }
    }


    public void ColorBombBoost()
    {
        if (GetColorBombBoostNum() > 0)
        {
            instructionText.SetActive(true);
            toggleBoost = true;
            colorBombBoost = true;
        }
    }

    public void DestroyBoost()
    {
        if (GetDestroyBoostNum() > 0) {
            instructionText.SetActive(true);
            toggleBoost = true;
            destroyBoost = true;
        }
    }

    public void AdjacentBombBoost()
    {
        if(GetAdjacentBoostNum() > 0)
        {
            instructionText.SetActive(true);
            toggleBoost = true;
            adjacentBombBoost = true;
        }
    }


    public int GetAdjacentBoostNum()
    {
        adjacentBombBoostNum = PlayerPrefs.GetInt("AdjacentBoost");

        return adjacentBombBoostNum;
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

    public void DisplayAdjacentBombNum()
    {
        adjacentBombNumText.text = GetAdjacentBoostNum().ToString();
    }

    public void DecreaseDestroyNum()
    {
        destroyBoostNum = GetDestroyBoostNum() - 1;
        PlayerPrefs.SetInt("DestroyBoost", destroyBoostNum);
        instructionText.SetActive(false);
        DisplayDestroyNum();
    }

    public void DecreaseColorNum()
    {
        colorBombBoostNum = GetColorBombBoostNum() - 1;
        PlayerPrefs.SetInt("ColorBombBoost", colorBombBoostNum);
        instructionText.SetActive(false);
        DisplayColorBombNum();
    }

    public void DecreaseAdjacentNum()
    {
        adjacentBombBoostNum = GetAdjacentBoostNum() - 1;
        PlayerPrefs.SetInt("AdjacentBoost", adjacentBombBoostNum);
        instructionText.SetActive(false);
        DisplayAdjacentBombNum();
    }

}
