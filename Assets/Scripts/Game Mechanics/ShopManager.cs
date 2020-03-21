using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ShopManager : MonoBehaviour
{

    public Animator animator;
    int totalCandy;
    public GameObject[] shopHats, equippedStatusImage, buyButtons, equipButtons;
    Score gameScore;

    private void Awake()
    {

        totalCandy = PlayerPrefs.GetInt("totalCandy");
        CheckBought();
        CheckEquipped();
    }

    // Start is called before the first frame update
    void Start()
    {
        gameScore = GameObject.FindGameObjectWithTag("Hat").GetComponent<Score>();
    }  

    public void ShowShop()
    {
        animator.SetBool("ShopShown", true);
    }

    public void HideShop()
    {
        animator.SetBool("ShopShown", false);
    }

    public void PurchaseHat(int hatBought) { 

        switch (hatBought)
        {
            case 1:
                
                if(totalCandy >= 200) { totalCandy -= 200; PurchaseProcessing(hatBought);
                    PlayerPrefs.SetInt("hat1Bought", 1); Debug.Log("Bought!");
                }
                //Display Updated TotalCandy
                break;

            case 2: 
                if (totalCandy >= 200) { totalCandy -= 200; PurchaseProcessing(hatBought);
                    PlayerPrefs.SetInt("hat2Bought", 1);
                }
                break;

            case 3: 
                if (totalCandy >= 400) { totalCandy -= 400; PurchaseProcessing(hatBought);
                    PlayerPrefs.SetInt("hat3Bought", 1);
                }
                break;

                 
        } 
    }

    void PurchaseProcessing(int hatNum) {
         
        buyButtons[hatNum - 1].SetActive(false);
        equipButtons[hatNum].SetActive(true);
        PlayerPrefs.SetInt("totalCandy", totalCandy);
        gameScore.DisplayTotalCandy(); 
    }

    public void EquipHat(int hatEquipped) {

        PlayerPrefs.SetInt("equippedHat", hatEquipped);
        SceneManager.LoadScene(0);
    }
     

    void CheckBought()
    {
        int hat1 = PlayerPrefs.GetInt("hat1Bought");
        int hat2 = PlayerPrefs.GetInt("hat2Bought");
        int hat3 = PlayerPrefs.GetInt("hat3Bought");

        if(hat1 == 1) {
            buyButtons[0].SetActive(false);
            equipButtons[1].SetActive(true);
        }
        if(hat2 == 1) {
            buyButtons[1].SetActive(false);
            equipButtons[2].SetActive(true);
        }
        if(hat3 == 1) {
            buyButtons[2].SetActive(false);
            equipButtons[3].SetActive(true);
        }

    }

    void CheckEquipped()
    {
        int equippedHat = PlayerPrefs.GetInt("equippedHat");
        equippedStatusImage[equippedHat].SetActive(true);
        equipButtons[equippedHat].GetComponent<Button>().interactable = false;

        Instantiate(shopHats[equippedHat], new Vector3(-2.8f, 0.85f, 0), Quaternion.identity );
    }

}
