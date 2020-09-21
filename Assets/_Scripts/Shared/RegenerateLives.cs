using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class RegenerateLives : MonoBehaviour
{

    public Text text, lifeText;

    ///life gaining timer
    public static float RestLifeTimer;

    ///date of exit for life timer
    public static string DateOfExit;

    ///amount of life
    public static int lifes { get; set; }

    //EDITOR: time for rest life
    public float TotalTimeForRestLifeHours;

    //EDITOR: time for rest life
    public float TotalTimeForRestLifeMin = 15;

    //EDITOR: time for rest life
    public float TotalTimeForRestLifeSec = 60;

    //EDITOR: max amount of life
    public int CapOfLife = 5;

    public DateTime serverTime;
    public bool dateReceived;
    public delegate void DateReceived();
    public static event DateReceived OnDateReceived;
    [Header("Test date example: 2019-08-27 09:12:29")]
    public string TestDate;

    static float TimeLeft;
    float TotalTimeForRestLife = 15f * 60;  //8 minutes for restore life
    bool startTimer;
    DateTime templateTime;
 
        // Start is called before the first frame update
        void Start()
    {
        RestLifeTimer = PlayerPrefs.GetFloat("RestLifeTimer");
        DateOfExit = PlayerPrefs.GetString("DateOfExit");
        if (DateOfExit == "" || DateOfExit == default(DateTime).ToString())
            DateOfExit = serverTime.ToString();

        lifes = PlayerPrefs.GetInt("totalLives");
        DisplayTotalLives();

        if (PlayerPrefs.GetInt("Lauched") == 0)
        {
            //First lauching
            lifes = CapOfLife;
            PlayerPrefs.SetInt("Lifes", lifes);

            PlayerPrefs.SetInt("Lauched", 1);
            PlayerPrefs.Save();
        }


        GetServerTime();
        TotalTimeForRestLife = TotalTimeForRestLifeHours * 60 * 60 + TotalTimeForRestLifeMin * 60 + TotalTimeForRestLifeSec;
    }

    private void OnEnable()
    {
        startTimer = false;
        GetServerTime(); 
    }


    void GetServerTime()
    {
        StartCoroutine(getTime());
    }

    IEnumerator getTime()
    {
#if UNITY_WEBGL
            serverTime = DateTime.Now;
#else
        WWW www = new WWW("https://candy-smith.info/gettime.php");
        yield return www;
        if (www.text != "")
            serverTime = DateTime.Parse(www.text);
        else
            serverTime = DateTime.Now;
        if (TestDate != "" && (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.LinuxEditor))
            serverTime = DateTime.Parse(TestDate);
#endif
        yield return null;
        dateReceived = true;
        OnDateReceived?.Invoke();
    }

    bool CheckPassedTime()
    {
        //print(InitScript.DateOfExit);
        if (DateOfExit == "" || DateOfExit == default(DateTime).ToString())
            DateOfExit = serverTime.ToString();

        var dateOfExit = DateTime.Parse(DateOfExit);
        if (serverTime.Subtract(dateOfExit).TotalSeconds > TotalTimeForRestLife * ( CapOfLife -  lifes))
        { 
            RestoreLifes();
            RestLifeTimer = 0;
            return false;    ///we dont need lifes
        }

        TimeCount((float)serverTime.Subtract(dateOfExit).TotalSeconds); 
        return true;     ///we need lifes
    }

    void TimeCount(float tick)
    {
        if (RestLifeTimer <= 0)
            ResetTimer();

        RestLifeTimer -= tick;
        if (RestLifeTimer <= 1 && lifes < CapOfLife)
        {
            AddLife(1);
            ResetTimer();
        } 
    }

    void ResetTimer()
    {
        RestLifeTimer = TotalTimeForRestLife;
    }


    public void RestoreLifes()
    {
        lifes = CapOfLife;
        lifeText.text = lifes.ToString();
        PlayerPrefs.SetInt("totalLives", lifes);
        PlayerPrefs.Save();
    }

    // Update is called once per frame
    void Update()
    {
        if (!startTimer && dateReceived && serverTime.Subtract(serverTime).Days == 0)
        {
            if (lifes <  CapOfLife)
            {
                if (CheckPassedTime())
                    startTimer = true; 
            }
        }

        if (startTimer)
            TimeCount(Time.deltaTime);

        if (gameObject.activeSelf)
        {
            if (lifes < CapOfLife)
            {
                if (TotalTimeForRestLifeHours > 0)
                {
                    var hours = Mathf.FloorToInt(RestLifeTimer / 3600);
                    var minutes = Mathf.FloorToInt((RestLifeTimer - hours * 3600) / 60);
                    var seconds = Mathf.FloorToInt((RestLifeTimer - hours * 3600) - minutes * 60);

                    text.enabled = true;
                    text.text = "" + string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
                }
                else
                {
                    var minutes = Mathf.FloorToInt(RestLifeTimer / 60F);
                    var seconds = Mathf.FloorToInt(RestLifeTimer - minutes * 60);

                    text.enabled = true;
                    text.text = "" + string.Format("{0:00}:{1:00}", minutes, seconds);

                } 
            }
            else
            {
                text.text = "-";
            }
        }
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        { 
            DateOfExit = serverTime.ToString();
            PlayerPrefs.SetString("DateOfExit", DateOfExit); 
        }
        else
        {
            startTimer = false; 
        }
    }
 
    void OnDisable()
    {
         DateOfExit = serverTime.ToString();
         PlayerPrefs.SetString("DateOfExit", DateOfExit);
        Debug.Log(serverTime.ToString());

    }


    void OnApplicationQuit()   
    {
        DateOfExit = serverTime.ToString();
        PlayerPrefs.SetString("DateOfExit", DateOfExit);
        Debug.Log(serverTime.ToString());
    }

    public void AddLife(int count)
    {
        lifes += count;
        lifeText.text = lifes.ToString();
        
        if (lifes > CapOfLife)
            lifes = CapOfLife;
        PlayerPrefs.SetInt("totalLives", lifes);
        PlayerPrefs.Save();
        DisplayTotalLives(); 
    }

    public int GetLife()
    {
        if (lifes > CapOfLife)
        {
            lifes = CapOfLife;
            PlayerPrefs.SetInt("totalLives", lifes);
            PlayerPrefs.Save();
        }

        return lifes;
    }

    public void DisplayTotalLives()
    {
        lifeText.text = PlayerPrefs.GetInt("totalLives").ToString();

    }

}
 