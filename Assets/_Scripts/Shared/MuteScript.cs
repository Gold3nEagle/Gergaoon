using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;

public class MuteScript : MonoBehaviour
{

    private bool toggle = false;
    public Sprite sound, mute;
    public Image buttonImage;

    // Start is called before the first frame update
    void Start()
    {
        buttonImage = GetComponent<Image>();

    }

    public void MuteAll()
    {

        if (toggle == false)
        {
            AudioListener.volume = 0f;
            buttonImage.sprite = mute;
            toggle = true;
        }
        else
        {
            AudioListener.volume = 1f;
            buttonImage.sprite = sound;
            toggle = false;
        }
    }


    public void PPAccess()
    {
        Application.OpenURL("https://www.geagle.tech/gergaoon-privacy-policy");
    } 

    public void RateGame()
    {
        Application.OpenURL("market://details?id=com.goldeneagle.gergaoon");
        PlayerPrefs.SetInt("BeenRated", 1); 
    } 

    public void ShareGame()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            AnalyticsResult analyticsResult = AnalyticsEvent.SocialShare(ShareType.Image, SocialNetwork.None);
            Debug.Log("Analytics Result: " + analyticsResult);
        } else
        {
            AnalyticsResult analyticsResult = AnalyticsEvent.SocialShare(ShareType.Achievement, SocialNetwork.None);
            Debug.Log("Analytics Result: " + analyticsResult);
        }
        StartCoroutine(TakeSSAndShare());
    }

    private IEnumerator TakeSSAndShare()
    {
        yield return new WaitForEndOfFrame();

        Texture2D ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        ss.Apply();

        string filePath = Path.Combine(Application.temporaryCachePath, "shared img.png");
        File.WriteAllBytes(filePath, ss.EncodeToPNG());

        // To avoid memory leaks
        Destroy(ss);
         
            if (Application.systemLanguage == SystemLanguage.Arabic)
            {

                new NativeShare().AddFile(filePath).SetSubject("Subject").SetText(" حمل لعبة قرقاعون على الأندرويد والآيفون! " +
                        " Android:  https://play.google.com/store/apps/details?id=com.goldeneagle.gergaoon " +
                        " iOS:  https://apps.apple.com/us/app/%D9%82%D8%B1%D9%82%D8%A7%D8%B9%D9%88%D9%86/id1485903162?ls=1 ").Share();

                // Share on WhatsApp only, if installed (Android only)
                if (NativeShare.TargetExists("com.whatsapp"))
                    new NativeShare().AddFile(filePath).SetText(" حمل لعبة قرقاعون على الأندرويد والآيفون! " +
                        " Android:  https://play.google.com/store/apps/details?id=com.goldeneagle.gergaoon " +
                        " iOS:  https://apps.apple.com/us/app/%D9%82%D8%B1%D9%82%D8%A7%D8%B9%D9%88%D9%86/id1485903162?ls=1 ").SetTarget("com.whatsapp").Share();
            }
            else
            {
                new NativeShare().AddFile(filePath).SetSubject("Subject").SetText("Download Gergaoon Now On Android & iOS!" +
                        " Android:  https://play.google.com/store/apps/details?id=com.goldeneagle.gergaoon " +
                        " iOS:  https://apps.apple.com/us/app/%D9%82%D8%B1%D9%82%D8%A7%D8%B9%D9%88%D9%86/id1485903162?ls=1 ").Share();

                // Share on WhatsApp only, if installed (Android only)
                if (NativeShare.TargetExists("com.whatsapp"))
                    new NativeShare().AddFile(filePath).SetText("Download Gergaoon Now On Android & iOS!" +
                        " Android:  https://play.google.com/store/apps/details?id=com.goldeneagle.gergaoon " +
                        " iOS:  https://apps.apple.com/us/app/%D9%82%D8%B1%D9%82%D8%A7%D8%B9%D9%88%D9%86/id1485903162?ls=1 ").SetTarget("com.whatsapp").Share();
            }
        }  

    public void AccessWebsite()
    {
        Application.OpenURL("https://geagle.tech/");
    }

    public void ResetLanguage()
    {
        PlayerPrefs.SetInt("LanguageNum", 0);
        SceneManager.LoadScene(0);
    }

    public void Feedback()
    {
        Application.OpenURL("https://geagle.tech/contact");
    
    }

}
