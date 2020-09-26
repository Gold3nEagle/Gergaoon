using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using UnityEngine.SceneManagement;
using SweetSugar.Scripts.GUI;
using UnityEngine.UI;




//https://developers.google.com/admob/unity/interstitial

// Called when an ad request has successfully loaded.
//this.interstitial.OnAdLoaded += HandleOnAdLoaded;
// Called when an ad request failed to load.
//  this.interstitial.OnAdFailedToLoad += HandleOnAdFailedToLoad;
// Called when an ad is shown.
//   this.interstitial.OnAdOpening += HandleOnAdOpened;
// Called when the ad is closed.
//  this.interstitial.OnAdClosed += HandleOnAdClosed;
// Called when the ad click caused the user to leave the application.
//  this.interstitial.OnAdLeavingApplication += HandleOnAdLeavingApplication;

public enum AdRewardType
{
    ExtraMoves,
    DoubleReward
}

public class AdsScript : MonoBehaviour
{

    public AdRewardType rewardType;
    private InterstitialAd interstitial;
    private RewardedAd rewardedAd;
    public Score gameScore;
    public string currentScene;
    public Overworld overWorld;
    UnityAds unityAds;
    int adsNum;
    bool adsEnabled = false;
    bool rewardedAdRequested = false;
    //public EndGameManager endGame;
    //public ScoreManager matchScore;

    public bool testRewardedBool = false;
    public Button rewardedAdButton01, rewardedAdButton02;

    // Start is called before the first frame update
    void Start()
    {
        unityAds = FindObjectOfType<UnityAds>();

        currentScene = SceneManager.GetActiveScene().name;
        adsNum = PlayerPrefs.GetInt("IAPAds");
        if (adsNum == 1)
        {
            adsEnabled = true;
        }


#if UNITY_ANDROID
        string appId = "ca-app-pub-8350868259993569~2254859297";
#elif UNITY_IPHONE
            string appId = "ca-app-pub-8350868259993569~9193603165";
#else
            string appId = "unexpected_platform";
#endif

        MobileAds.Initialize(appId);

        // Called when the user should be rewarded for interacting with the ad.  
        if (currentScene == "gameStatic")
        {
            this.RequestInterstitial();
            this.RequestRewardedAd();
        }

        if (currentScene == "ChatScene")
        {
            this.RequestRewardedAd();
        }

        if (currentScene == "GameScene")
            gameScore = GameObject.FindGameObjectWithTag("Hat").GetComponent<Score>();


        InvokeRepeating("CheckRewardedButtons", 5, 5);

    }

    public void RequestInterstitial()
    {
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-8350868259993569/2735927793";
#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-8350868259993569/4760863124";
#else
        string adUnitId = "unexpected_platform";
#endif

        if (adsEnabled == false)
        { 
            // Initialize an InterstitialAd.
            this.interstitial = new InterstitialAd(adUnitId);
            // Create an empty ad request.
            AdRequest request = new AdRequest.Builder().Build();
            // Load the interstitial with the request.
            this.interstitial.LoadAd(request);
        }
    } 

    public void RequestRewardedAd()
    {
#if UNITY_ANDROID
        string RadUnitId = "ca-app-pub-8350868259993569/9671077241";
#elif UNITY_IPHONE
        string RadUnitId = "unexpected_platform";
#else
        string RadUnitId = "unexpected_platform";
#endif

        if (adsEnabled == false)
        { 
            this.rewardedAd = new RewardedAd(RadUnitId);
            this.rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
            // Create an empty ad request.
            AdRequest request = new AdRequest.Builder().Build();
            // Load the rewarded ad with the request.
            this.rewardedAd.LoadAd(request);
            rewardedAdRequested = true;
        }
    }

    public void ShowInterstitialAd()
    {
        if (adsEnabled == false)
        {
            if (this.interstitial.IsLoaded())
            {
                this.interstitial.Show();
            } else
            {
                UnityAds unityAds = FindObjectOfType<UnityAds>();
                unityAds.ShowInterstitialAd();
            }
        }
    }

    public void WatchRewardedAd()
    {
        if (this.rewardedAd.IsLoaded())
        {
            this.rewardedAd.Show();
        } else
        {
            unityAds.ShowRewardedVideo();
        }
    }

    public void RewardedType(int type)
    {
        if (type == 1)
        {
            rewardType = AdRewardType.ExtraMoves;
        }

        else if (type == 2)
        {
            rewardType = AdRewardType.DoubleReward;
        }

        WatchRewardedAd();
    }

    void CheckRewardedButtons()
    {
        if (rewardedAdButton01 != null)
        {
            if (rewardedAdRequested == true)
            {
                if (rewardedAd.IsLoaded() == false && unityAds.UnityRewardedReady() == false)
                {
                    rewardedAdButton01.interactable = false;
                }
                else
                {
                    rewardedAdButton01.interactable = true;
                }
            }
        }

        if (rewardedAdButton02 != null)
        {
            if (rewardedAdRequested == true)
            {
                if (rewardedAd.IsLoaded() == false && unityAds.UnityRewardedReady() == false)
                {
                    rewardedAdButton02.interactable = false;
                }
                else
                {
                    rewardedAdButton02.interactable = true;
                }
            }
        }
    }


    public void HandleUserEarnedReward(object sender, Reward args)
    {
        if (currentScene == "GameScene")
        {
            int totalCandy = PlayerPrefs.GetInt("totalCandy");
            totalCandy += 250;
            PlayerPrefs.SetInt("totalCandy", totalCandy);
            gameScore.DisplayTotalCandy();
        }
        else if (currentScene == "ChatScene")
        {
            int totalCandy = PlayerPrefs.GetInt("totalCandy");
            totalCandy += 250;
            PlayerPrefs.SetInt("totalCandy", totalCandy);
            MainSceneHandler mHandler = FindObjectOfType<MainSceneHandler>();
            mHandler.DisplayTotalCandy();
        }

        else if (currentScene == "OverWorld")
        {
            overWorld = FindObjectOfType<Overworld>();
            int totalLives = PlayerPrefs.GetInt("totalLives");
            totalLives++;
            PlayerPrefs.SetInt("totalLives", totalLives);
            RegenerateLives lifeRegan = FindObjectOfType<RegenerateLives>();
            lifeRegan.DisplayTotalLives();
        }

        else if (currentScene == "gameStatic")
        {
            if (rewardType == AdRewardType.ExtraMoves)
            {
                AnimationEventManager animationEventManager = GameObject.Find("PreFailed").GetComponent<AnimationEventManager>();
                animationEventManager.GoOnFailed();
            }
            else if (rewardType == AdRewardType.DoubleReward)
            {
                int receivedCandy = PlayerPrefs.GetInt("DoubleReward");
                int totalCandy = PlayerPrefs.GetInt("totalCandy");

                totalCandy += receivedCandy;
                PlayerPrefs.SetInt("totalCandy", totalCandy);

                BackToMenu backToMenu = FindObjectOfType<BackToMenu>();
                backToMenu.SetCandyScoreText(receivedCandy * 2);
                 
            }
        }

    }

    public void HandleUnityAdsReward()
    {
        if (currentScene == "GameScene")
        {
            int totalCandy = PlayerPrefs.GetInt("totalCandy");
            totalCandy += 250;
            PlayerPrefs.SetInt("totalCandy", totalCandy);
            gameScore.DisplayTotalCandy();
        }
        else if (currentScene == "ChatScene")
        {
            int totalCandy = PlayerPrefs.GetInt("totalCandy");
            totalCandy += 250;
            PlayerPrefs.SetInt("totalCandy", totalCandy);
            MainSceneHandler mHandler = FindObjectOfType<MainSceneHandler>();
            mHandler.DisplayTotalCandy();
        }

        else if (currentScene == "OverWorld")
        {
            overWorld = FindObjectOfType<Overworld>();
            int totalLives = PlayerPrefs.GetInt("totalLives");
            totalLives++;
            PlayerPrefs.SetInt("totalLives", totalLives);
            RegenerateLives lifeRegan = FindObjectOfType<RegenerateLives>();
            lifeRegan.DisplayTotalLives();
        }

        else if (currentScene == "gameStatic")
        {
            if (rewardType == AdRewardType.ExtraMoves)
            {
                AnimationEventManager animationEventManager = GameObject.Find("PreFailed").GetComponent<AnimationEventManager>();
                animationEventManager.GoOnFailed();
            }
            else if (rewardType == AdRewardType.DoubleReward)
            {
                int receivedCandy = PlayerPrefs.GetInt("DoubleReward");
                int totalCandy = PlayerPrefs.GetInt("totalCandy");

                totalCandy += receivedCandy;
                PlayerPrefs.SetInt("totalCandy", totalCandy);

                BackToMenu backToMenu = FindObjectOfType<BackToMenu>();
                backToMenu.SetCandyScoreText(receivedCandy * 2);

            }
        }
    }
}
