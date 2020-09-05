using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Notifications.Android;
using UnityEngine.SceneManagement;

public class NotificationsManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if(SceneManager.GetActiveScene().name == "MainMenu")
        {
            PlayerPrefs.SetInt("LifeNotification", 1);
        } 
        CreateNotificationChannel();
        SendLivesReplenishedNotification();
    } 

    void CreateNotificationChannel()
    {
        var defaultChannel = new AndroidNotificationChannel()
        {
            Id = "default_channel",
            Name = "HourlyNotification",
            Importance = Importance.High,
            Description = "For Lives Regenerated",
        };

        var DailyRewardsChannel = new AndroidNotificationChannel()
        {
            Id = "daily_channel",
            Name = "DailyNotification",
            Importance = Importance.High,
            Description = "For daily rewarded notifications",
        }; 
         
        AndroidNotificationCenter.RegisterNotificationChannel(DailyRewardsChannel);
        AndroidNotificationCenter.RegisterNotificationChannel(defaultChannel);
    }


    public void SendRewardNotification()
    {
        var notification = new AndroidNotification(); ;

        if (PlayerPrefs.HasKey("LanguageNum"))
        {
            int language = PlayerPrefs.GetInt("LanguageNum");
            if (language == 1)
            {
                notification.Title = "قرقاعون";
                notification.Text = "احصل على جائزتك اليومية المجانية الآن!";
                notification.LargeIcon = "icon_0";
                notification.FireTime = System.DateTime.Now.AddDays(1);
            }
            else if (language == 2)
            {
                notification.Title = "Gergaoon";
                notification.Text = "Come Back And Get Your Daily Reward Now!";
                notification.LargeIcon = "icon_0";
                notification.FireTime = System.DateTime.Now.AddDays(1);
            }
        }  
        AndroidNotificationCenter.SendNotification(notification, "daily_channel");
    }

    
    public void SendLivesReplenishedNotification()
    {
        int toggle = PlayerPrefs.GetInt("LifeNotification");
        if (toggle == 1)
        { 
            var notificationRestoreLives = new AndroidNotification();

            if (PlayerPrefs.HasKey("LanguageNum"))
            {
                int language = PlayerPrefs.GetInt("LanguageNum");
                if (language == 1)
                {
                    notificationRestoreLives.Title = "قرقاعون";
                    notificationRestoreLives.Text = "لقد امتلأ عداد المحاولات! حياك العب!";
                    notificationRestoreLives.LargeIcon = "icon_0";
                    notificationRestoreLives.FireTime = System.DateTime.Now.AddHours(2);

                }
                else if (language == 2)
                {
                    notificationRestoreLives.Title = "Gergaoon";
                    notificationRestoreLives.Text = "Tries Replenished! Come Play!";
                    notificationRestoreLives.LargeIcon = "icon_0";
                    notificationRestoreLives.FireTime = System.DateTime.Now.AddHours(2);
                }

            }
            AndroidNotificationCenter.SendNotification(notificationRestoreLives, "default_channel");
            PlayerPrefs.SetInt("LifeNotification", 0);
        }
    }


    private void OnApplicationQuit()
    { 
        SendLivesReplenishedNotification();
    }

}
