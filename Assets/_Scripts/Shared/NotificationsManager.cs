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
        CreateNotificationChannel();
        if(SceneManager.GetActiveScene().buildIndex == 0)
        {
            RetentionNotification();
        }
    } 

    void CreateNotificationChannel()
    {
        var channel = new AndroidNotificationChannel()
        {
            Id = "default_channel",
            Name = "DailyNotification",
            Importance = Importance.High,
            Description = "For daily rewarded notifications",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);
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
        AndroidNotificationCenter.SendNotification(notification, "default_channel");
    }

    public void RetentionNotification()
    {
        var notification = new AndroidNotification();
        notification.Title = "قرقاعون";
        notification.Text = "تقدر تجمع حلوى أكثر اليوم؟";
        notification.LargeIcon = "icon_0";
        notification.FireTime = System.DateTime.Now.AddDays(5);

        AndroidNotificationCenter.SendNotification(notification, "default_channel");
    }
}
