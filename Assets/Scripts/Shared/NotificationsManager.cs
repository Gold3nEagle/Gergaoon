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
        var notification = new AndroidNotification();
        notification.Title = "قرقاعون";
        notification.Text = "احصل على جائزتك اليومية المجانية الآن!";
        notification.LargeIcon = "largeIcon";
        notification.FireTime = System.DateTime.Now.AddDays(1);

        AndroidNotificationCenter.SendNotification(notification, "default_channel");
    }

    public void RetentionNotification()
    {
        var notification = new AndroidNotification();
        notification.Title = "قرقاعون";
        notification.Text = "تقدر تجمع حلوى أكثر اليوم؟";
        notification.LargeIcon = "largeIcon";
        notification.FireTime = System.DateTime.Now.AddDays(5);

        AndroidNotificationCenter.SendNotification(notification, "default_channel");
    }
}
