﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CandyShop : MonoBehaviour
{

    public Overworld overWorld;
    int totalCandy;
    public int extraMovesPrice, freeMovePrice, explodeAreaPrice, removeCandyPrice;
    public Text extraMovesAmount, freeMoveAmount, explodeAreaAmount, removeCandyAmount;
    public GameObject iapPanel;

    // Start is called before the first frame update
    void Start()
    {
        totalCandy = overWorld.GetTotalCandy();
        DisplayBoostsAmount();
    }


    public void BuyExtraMoves()
    {
        if (totalCandy >= extraMovesPrice)
        {
            int extraMovesBoostNum = PlayerPrefs.GetInt("ExtraMoves");
            extraMovesBoostNum++;
            PlayerPrefs.SetInt("ExtraMoves", extraMovesBoostNum);

            totalCandy -= extraMovesPrice;
            PlayerPrefs.SetInt("totalCandy", totalCandy);
            overWorld.DisplayTotalCandy();
            DisplayBoostsAmount();

            //Log Firebase Spend Coin Event
            Firebase.Analytics.FirebaseAnalytics.LogEvent(
            Firebase.Analytics.FirebaseAnalytics.EventSpendVirtualCurrency,
            new Firebase.Analytics.Parameter[] {
            new Firebase.Analytics.Parameter(
            Firebase.Analytics.FirebaseAnalytics.ParameterItemName, "Extra Moves"),
            new Firebase.Analytics.Parameter(
            Firebase.Analytics.FirebaseAnalytics.ParameterValue, totalCandy),
            new Firebase.Analytics.Parameter(
            Firebase.Analytics.FirebaseAnalytics.ParameterVirtualCurrencyName, "Total Candy"),
                        }
                );



        }
        else
        {
            gameObject.SetActive(false);
            iapPanel.SetActive(true);
        }
    }

    public void BuyFreeMove()
    {
        if (totalCandy >= freeMovePrice)
        {
            int freeMoveNum = PlayerPrefs.GetInt("FreeMove");
            freeMoveNum++;
            PlayerPrefs.SetInt("FreeMove", freeMoveNum);

            totalCandy -= freeMovePrice;
            PlayerPrefs.SetInt("totalCandy", totalCandy);
            overWorld.DisplayTotalCandy();
            DisplayBoostsAmount();


            //Log Firebase Spend Coin Event
            Firebase.Analytics.FirebaseAnalytics.LogEvent(
            Firebase.Analytics.FirebaseAnalytics.EventSpendVirtualCurrency,
            new Firebase.Analytics.Parameter[] {
            new Firebase.Analytics.Parameter(
            Firebase.Analytics.FirebaseAnalytics.ParameterItemName, "Free Moves"),
            new Firebase.Analytics.Parameter(
            Firebase.Analytics.FirebaseAnalytics.ParameterValue, totalCandy),
            new Firebase.Analytics.Parameter(
            Firebase.Analytics.FirebaseAnalytics.ParameterVirtualCurrencyName, "Total Candy"),
                        }
                );


        }
        else
        {
            gameObject.SetActive(false);
            iapPanel.SetActive(true);
        }
    }

    public void BuyExplodeArea()
    {
        if (totalCandy >= explodeAreaPrice)
        {
            int explodeAreaNum = PlayerPrefs.GetInt("ExplodeArea"); ;
            explodeAreaNum++;
            PlayerPrefs.SetInt("ExplodeArea", explodeAreaNum);

            totalCandy -= freeMovePrice;
            PlayerPrefs.SetInt("totalCandy", totalCandy);
            overWorld.DisplayTotalCandy();
            DisplayBoostsAmount();

            //Log Firebase Spend Coin Event
            Firebase.Analytics.FirebaseAnalytics.LogEvent(
            Firebase.Analytics.FirebaseAnalytics.EventSpendVirtualCurrency,
            new Firebase.Analytics.Parameter[] {
            new Firebase.Analytics.Parameter(
            Firebase.Analytics.FirebaseAnalytics.ParameterItemName, "Explode Area"),
            new Firebase.Analytics.Parameter(
            Firebase.Analytics.FirebaseAnalytics.ParameterValue, totalCandy),
            new Firebase.Analytics.Parameter(
            Firebase.Analytics.FirebaseAnalytics.ParameterVirtualCurrencyName, "Total Candy"),
                        }
                );



        }
        else
        {
            gameObject.SetActive(false);
            iapPanel.SetActive(true);
        }
    }

    public void BuyRemoveCandy()
    {
        if (totalCandy >= removeCandyPrice)
        {
            int removeCandyNum = PlayerPrefs.GetInt("Bomb"); ;
            removeCandyNum++;
            PlayerPrefs.SetInt("Bomb", removeCandyNum);

            totalCandy -= freeMovePrice;
            PlayerPrefs.SetInt("totalCandy", totalCandy);
            overWorld.DisplayTotalCandy();
            DisplayBoostsAmount();


            //Log Firebase Spend Coin Event
            Firebase.Analytics.FirebaseAnalytics.LogEvent(
            Firebase.Analytics.FirebaseAnalytics.EventSpendVirtualCurrency,
            new Firebase.Analytics.Parameter[] {
            new Firebase.Analytics.Parameter(
            Firebase.Analytics.FirebaseAnalytics.ParameterItemName, "Remove Candy"),
            new Firebase.Analytics.Parameter(
            Firebase.Analytics.FirebaseAnalytics.ParameterValue, totalCandy),
            new Firebase.Analytics.Parameter(
            Firebase.Analytics.FirebaseAnalytics.ParameterVirtualCurrencyName, "Total Candy"),
                        }
                );


        }
        else
        {
            gameObject.SetActive(false);
            iapPanel.SetActive(true);
        }
    }

    void DisplayBoostsAmount()
    {
        int extraMovesBoostNum = PlayerPrefs.GetInt("ExtraMoves");
        int freeMoveNum = PlayerPrefs.GetInt("FreeMove");
        int explodeAreaNum = PlayerPrefs.GetInt("ExplodeArea");
        int removeCandyNum = PlayerPrefs.GetInt("Bomb");


        extraMovesAmount.text = extraMovesBoostNum.ToString();
        freeMoveAmount.text = freeMoveNum.ToString();
        explodeAreaAmount.text = explodeAreaNum.ToString();
        removeCandyAmount.text = removeCandyNum.ToString();
    }

}
