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

    public List<int> rewardCoin;
    public List<Button> rewardButton;

    public Text buttonText;
    public bool delete;
    int coins = 0;

    private void Start()
    {
        if (delete)
        {
            PlayerPrefs.DeleteAll();
        }

        buttonText.text = coins.ToString();
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

        string dateOld = PlayerPrefs.GetString("PlayDateOld");

        if (string.IsNullOrEmpty(dateOld) && !PlayerPrefs.HasKey("PlayDateOld"))
        {
            Debug.Log("First Time Opening The Game.");
            rewardButton[0].interactable = true;
            PlayerPrefs.SetString("PlayDateOld", startDate);
            PlayerPrefs.SetInt("PlayGameCount", 1);
        }
        else
        {
            DateTime currentDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
            //DateTime _dateNow = Convert.ToDateTime(sDate);
            DateTime oldRecordedDate = Convert.ToDateTime(dateOld);

            TimeSpan difference = currentDate.Subtract(oldRecordedDate);

            if (difference.Days >= 1 && difference.Days < 2)
            {
                int gameCount = PlayerPrefs.GetInt("PlayGameCount");
                if (gameCount == 1)
                {
                    rewardButton[1].interactable = true;
                    PlayerPrefs.SetInt("PlayGameCount", 2);
                }
                else if (gameCount == 2)
                {
                    rewardButton[2].interactable = true;
                }
                Debug.Log("Other days whatever that means");
                PlayerPrefs.SetString("PlayDateOld", currentDate.ToString());

            }
            else if (difference.Days >= 2)
            {
                rewardButton[0].interactable = true;
                PlayerPrefs.SetInt("PlayGameCount", 1);
                PlayerPrefs.SetString("PlayDateOld", currentDate.ToString());
            }
        }
    }

    public void Reward(int count)
    {
        coins += rewardCoin[count];
        buttonText.text = coins.ToString();
        Button clickButton = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        clickButton.interactable = false;
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