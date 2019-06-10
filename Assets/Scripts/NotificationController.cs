using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Assets.SimpleAndroidNotifications { 

public class NotificationController : MonoBehaviour
{

        int timesPlayed;
        int Score;

    // Start is called before the first frame update
    void Start()
    {
            timesPlayed = PlayerPrefs.GetInt("TP");
            Score = PlayerPrefs.GetInt("Score"); 
            NotificationSetup(timesPlayed);


        }


        public void NotificationSetup(int tp)
        {

            if(tp > 6) { tp = 0; PlayerPrefs.SetInt("TP", tp); }

            switch (tp)
            {
                case 0:
                    NotificationManager.SendWithAppIcon(TimeSpan.FromHours(2), "قدها والا لا؟", " أعلى نقطة حصلتها كانت " + Score + " تقدر تحصل أعلى منها؟ ", new Color(0, 0.6f, 0), NotificationIcon.Star); 
                    break;

                case 1:
                    NotificationManager.SendWithAppIcon(TimeSpan.FromHours(10), "متملل؟"  , "أتحداك تحصل أعلى نقطة في تاريخ اللعبة!" , new Color(0, 0.6f, 0), NotificationIcon.Star);
                    break;

                case 2:
                    NotificationManager.SendWithAppIcon(TimeSpan.FromDays(5), "تبغي جوائز؟", "لا تنسى تتابع حسابنا عالانستقرام للفوز بجوائز من خلال اللعبة!", new Color(0, 0.6f, 0), NotificationIcon.Star);
                    break;

                case 3:
                    NotificationManager.SendWithAppIcon(TimeSpan.FromDays(2), "مع اصحابك؟"  , "خلهم يحملون اللعبة معاك وعيشوا الطفولة", new Color(0, 0.6f, 0), NotificationIcon.Star);
                    break;

                case 4:
                    NotificationManager.SendWithAppIcon(TimeSpan.FromDays(2), "مرحبا!", "لو نافست اصحابك من بيجمع قرقاعون أكثر؟", new Color(0, 0.6f, 0), NotificationIcon.Star);
                    break;

                case 5:
                    NotificationManager.SendWithAppIcon(TimeSpan.FromDays(2), "مرحبا!", "لو نافست اصحابك من بيجمع قرقاعون أكثر؟", new Color(0, 0.6f, 0), NotificationIcon.Star);
                    break;

                case 6:
                    NotificationManager.SendWithAppIcon(TimeSpan.FromDays(2), "تبي تجوف دياية ترقص شرقي؟", "ادخل اللعبة وشغل لها اغنية قرقاعون وطالع بعينك!", new Color(0, 0.6f, 0), NotificationIcon.Star);
                    break;


            }


        }

 
}
}