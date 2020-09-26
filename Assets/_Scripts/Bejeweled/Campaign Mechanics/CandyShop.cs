using System.Collections;
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
    IAPManager iapManager;

    // Start is called before the first frame update
    void Start()
    {
        totalCandy = overWorld.GetTotalCandy();
        DisplayBoostsAmount();

        iapManager = FindObjectOfType<IAPManager>();
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
              
        }
        else
        {
            gameObject.SetActive(false);
            iapPanel.SetActive(true);
            iapManager.RetrievePrices();
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
        }
        else
        {
            gameObject.SetActive(false);
            iapPanel.SetActive(true);
            iapManager.RetrievePrices();
        }
    }

    public void BuyExplodeArea()
    {
        if (totalCandy >= explodeAreaPrice)
        {
            int explodeAreaNum = PlayerPrefs.GetInt("ExplodeArea"); ;
            explodeAreaNum++;
            PlayerPrefs.SetInt("ExplodeArea", explodeAreaNum);

            totalCandy -= explodeAreaPrice;
            PlayerPrefs.SetInt("totalCandy", totalCandy);
            overWorld.DisplayTotalCandy();
            DisplayBoostsAmount();
             
        }
        else
        {
            gameObject.SetActive(false);
            iapPanel.SetActive(true);
            iapManager.RetrievePrices();
        }
    }

    public void BuyRemoveCandy()
    {
        if (totalCandy >= removeCandyPrice)
        {
            int removeCandyNum = PlayerPrefs.GetInt("Bomb"); ;
            removeCandyNum++;
            PlayerPrefs.SetInt("Bomb", removeCandyNum);

            totalCandy -= removeCandyPrice;
            PlayerPrefs.SetInt("totalCandy", totalCandy);
            overWorld.DisplayTotalCandy();
            DisplayBoostsAmount();
 
        }
        else
        {
            gameObject.SetActive(false);
            iapPanel.SetActive(true);
            iapManager.RetrievePrices();
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
