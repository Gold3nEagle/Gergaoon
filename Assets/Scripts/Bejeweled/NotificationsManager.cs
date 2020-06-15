using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Notifications.Android;

public class NotificationsManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CreateNotificationChannel()
    {
        var channel = new AndroidNotificationChannel()
        {
            Id = "RegainLife",
            Name = "Life Channel",
            Importance = Importance.High,
            Description = "Notifies the player when a life is regained",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);
    }

    void SendNotification()
    {
        var notification = new AndroidNotification();
        notification.Title = "Your Title";
        notification.Text = "Your Text";
        notification.FireTime = System.DateTime.Now.AddMinutes(1);

        AndroidNotificationCenter.SendNotification(notification, "channel_id");
    }
}
