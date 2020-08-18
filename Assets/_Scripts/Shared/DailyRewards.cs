using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DailyRewards : MonoBehaviour
{

    string url = "www.google.com";
    string urlDate = "http://worldclockapi.com/api/json/est/now";
    string startDate, startTime;
     

    public List<Button> rewardButton;
    public GameObject rewardsPanel;
    public Overworld overWorld;
    public NotificationsManager notificationsManager;
     
    public bool delete; 

    private void Start()
    {
        if (delete)
        {
            PlayerPrefs.DeleteAll();
        }
         
        StartCoroutine(CheckInternet());
    }

    private IEnumerator CheckInternet()
    {
        WWW www = new WWW(url);
        yield return www;

        if (string.IsNullOrEmpty(www.text))
        {
            Debug.Log("Not Connected to the Internet");
        }
        else
        {
            Debug.Log("Connected Successfully");
            StartCoroutine(CheckDate());
        }
    }

    private IEnumerator CheckDate()
    {
        WWW www = new WWW(urlDate);
        yield return www;

        string[] splitDate = www.text.Split(new string[] { "currentDateTime\":\"" }, StringSplitOptions.None);

        startDate = splitDate[1].Substring(0, 10);
        startTime = splitDate[1].Substring(11, 5);
        Debug.Log(startDate);
        Debug.Log(startTime); 
        DailyRewardCheck();

    }

    public void DailyRewardCheck()
    {

        string oldPlayDate = PlayerPrefs.GetString("OldPlayDate");

        if (string.IsNullOrEmpty(oldPlayDate) && !PlayerPrefs.HasKey("OldPlayDate"))
        {
            rewardsPanel.SetActive(true);
            Debug.Log("First Time Opening The Game.");
            rewardButton[0].interactable = true;
            PlayerPrefs.SetString("OldPlayDate", startDate);
            PlayerPrefs.SetInt("PlayGameCount", 1);
        }
        else
        {
            DateTime currentDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
            //DateTime _dateNow = Convert.ToDateTime(sDate);
            DateTime oldRecordedDate = Convert.ToDateTime(oldPlayDate);

            TimeSpan difference = currentDate.Subtract(oldRecordedDate);

            if (difference.Days >= 1 && difference.Days < 2)
            {
                rewardsPanel.SetActive(true);
                int gameCount = PlayerPrefs.GetInt("PlayGameCount");
                if (gameCount == 1)
                {
                    rewardButton[0].interactable = true;
                    PlayerPrefs.SetInt("PlayGameCount", 2);
                }
                else if (gameCount == 2)
                {
                    rewardsPanel.SetActive(true);
                    rewardButton[1].interactable = true;
                }
                Debug.Log("Other days whatever that means");
                PlayerPrefs.SetString("OldPlayDate", currentDate.ToString());

            }
            else if (difference.Days >= 2)
            {
                rewardsPanel.SetActive(true);
                rewardButton[2].interactable = true;
                PlayerPrefs.SetInt("PlayGameCount", 1);
                PlayerPrefs.SetString("OldPlayDate", currentDate.ToString());
            }
        }
    }

    public void RewardLives()
    {
        int totalLives = PlayerPrefs.GetInt("totalLives");
        totalLives += 2;
        PlayerPrefs.SetInt("totalLives", totalLives);
        notificationsManager.SendRewardNotification();
        Button clickButton = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        clickButton.interactable = false; 
        rewardsPanel.SetActive(false);
    }

    public void RewardCandies()
    {
        int totalCandy = PlayerPrefs.GetInt("totalCandy");
        totalCandy += 1000;
        PlayerPrefs.SetInt("totalCandy", totalCandy);
        notificationsManager.SendRewardNotification();
        Button clickButton = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        clickButton.interactable = false;
        overWorld.DisplayTotalCandy();
        rewardsPanel.SetActive(false);
    }

    public void RewardRainbow()
    {
        int colorBomb = PlayerPrefs.GetInt("ColorBombBoost");
        colorBomb += 3;
        PlayerPrefs.SetInt("ColorBombBoost", colorBomb);
        notificationsManager.SendRewardNotification();
        Button clickButton = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        clickButton.interactable = false;
        rewardsPanel.SetActive(false);
    }
}




//private IEnumerator CheckInternet()
//{
//    using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
//    {
//        yield return webRequest.SendWebRequest();
//        if (!webRequest.isNetworkError)
//        {
//            Debug.Log("Success Internet");
//            StartCoroutine(CheckDate());
//        }
//    }


//}
//private IEnumerator CheckDate()
//{
//    using (UnityWebRequest webRequest = UnityWebRequest.Get(urlDate))
//    {
//        yield return webRequest.SendWebRequest();
//        if (!webRequest.isNetworkError)
//        {
//            string[] splitDate = webRequest.downloadHandler.text.Split(new string[] { "currentDateTime\":\"" }, StringSplitOptions.None);
//            sDate = splitDate[1].Substring(0, 10);

//        }
//    }
//    Debug.Log(sDate);
//    dailyButton.interactable = true;
//}