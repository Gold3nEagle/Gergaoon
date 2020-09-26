using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Notifications.Android;
using UnityEngine.SceneManagement;

public class NotificationsManager : MonoBehaviour
{
    SystemLanguage systemLanguage;

    // Start is called before the first frame update
    void Start()
    {

        systemLanguage = Application.systemLanguage;

        string language = Application.systemLanguage.ToString(); 
         
         
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            PlayerPrefs.SetInt("LifeNotification", 1);
        } 

        CreateNotificationChannel();

        int lifes = PlayerPrefs.GetInt("totalLives");
        int lifeNotification = PlayerPrefs.GetInt("LifeNotification");
        if(lifes < 5 && lifeNotification == 1)
        {
            SendLivesReplenishedNotification();
        }

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

        AndroidNotificationCenter.RegisterNotificationChannel(defaultChannel);
    }


    public void SendRewardNotification()
    {
        var notification = new AndroidNotification(); ;
         
            if (systemLanguage == SystemLanguage.Arabic)
            {
                notification.Title = "قرقاعون";
                notification.Text = "احصل على جائزتك اليومية المجانية الآن!";
                notification.LargeIcon = "icon_0";
                notification.FireTime = System.DateTime.Now.AddHours(24);
            }
            else
            {
                notification.Title = "Gergaoon";
                notification.Text = "Come Back And Get Your Daily Reward Now!";
                notification.LargeIcon = "icon_0";
                notification.FireTime = System.DateTime.Now.AddHours(24);
            }
        
        AndroidNotificationCenter.SendNotification(notification, "default_channel");
    }

    
    public void SendLivesReplenishedNotification()
    {
        int toggle = PlayerPrefs.GetInt("LifeNotification");
        if (toggle == 1)
        { 
            var notificationRestoreLives = new AndroidNotification();
             
                if (systemLanguage == SystemLanguage.Arabic)
                {
                    notificationRestoreLives.Title = "قرقاعون";
                    notificationRestoreLives.Text = "لقد امتلأ عداد المحاولات! حياك العب!";
                    notificationRestoreLives.LargeIcon = "icon_0";
                    notificationRestoreLives.FireTime = System.DateTime.Now.AddHours(1);

                }
                else
                {
                    notificationRestoreLives.Title = "Gergaoon";
                    notificationRestoreLives.Text = "Tries Replenished! Come Play!";
                    notificationRestoreLives.LargeIcon = "icon_0";
                    notificationRestoreLives.FireTime = System.DateTime.Now.AddHours(1);
                }

            
            AndroidNotificationCenter.SendNotification(notificationRestoreLives, "default_channel");
            PlayerPrefs.SetInt("LifeNotification", 0);
        }
    } 
}
