using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CandyShop : MonoBehaviour
{

    public Overworld overWorld;
    int totalCandy;
    public int destroyBoostPrice, rainbowBoostPrice;
    public Text destroyBoostAmount, rainbowBoostAmount;

    // Start is called before the first frame update
    void Start()
    {
        totalCandy = overWorld.GetTotalCandy();
        DisplayBoostsAmount();
    }


    public void BuyDestroyBoost()
    {
        if(totalCandy >= destroyBoostPrice)
        { 
            int destroyBoostNum = PlayerPrefs.GetInt("DestroyBoost");
            destroyBoostNum++;
            PlayerPrefs.SetInt("DestroyBoost", destroyBoostNum);

            totalCandy -= destroyBoostPrice;
            PlayerPrefs.SetInt("totalCandy", totalCandy);
            overWorld.DisplayTotalCandy();
            DisplayBoostsAmount();
        }
    }

    public void BuyRainbowBoost()
    {
        if(totalCandy >= rainbowBoostPrice)
        {
            int rainbowBoostNum = PlayerPrefs.GetInt("ColorBombBoost");
            rainbowBoostNum++;
            PlayerPrefs.SetInt("ColorBombBoost", rainbowBoostNum);

            totalCandy -= rainbowBoostPrice;
            PlayerPrefs.SetInt("totalCandy", totalCandy);
            overWorld.DisplayTotalCandy();
            DisplayBoostsAmount();
        }
    } 

    void DisplayBoostsAmount()
    {
        int destroyBoostNum = PlayerPrefs.GetInt("DestroyBoost");
        int rainbowBoostNum = PlayerPrefs.GetInt("ColorBombBoost");

        destroyBoostAmount.text = destroyBoostNum.ToString();
        rainbowBoostAmount.text = rainbowBoostNum.ToString();
    }



}
