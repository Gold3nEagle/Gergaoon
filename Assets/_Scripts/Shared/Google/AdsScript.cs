using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using UnityEngine.SceneManagement;
using SweetSugar.Scripts.GUI;



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

    AdRewardType rewardType;
    private InterstitialAd interstitial;
    private RewardedAd rewardedAd;
    Score gameScore;
    string currentScene;
    Overworld overWorld;
    int adsNum;
    bool adsEnabled = false;
    //public EndGameManager endGame;
    //public ScoreManager matchScore;

    // Start is called before the first frame update
    void Start()
    {
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

        if (currentScene == "GameScene")
            gameScore = GameObject.FindGameObjectWithTag("Hat").GetComponent<Score>();
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

        // Initialize an InterstitialAd.
        this.interstitial = new InterstitialAd(adUnitId);
        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the interstitial with the request.
        this.interstitial.LoadAd(request);

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



        this.rewardedAd = new RewardedAd(RadUnitId);
        this.rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the rewarded ad with the request.
        this.rewardedAd.LoadAd(request);
    }

    public void ShowInterstitialAd()
    {
        if (adsEnabled == false)
        {
            if (this.interstitial.IsLoaded())
            {
                this.interstitial.Show();
            }
        }
    }

    public void WatchRewardedAd()
    {
        if (this.rewardedAd.IsLoaded())
        {
            this.rewardedAd.Show();
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



    public void HandleUserEarnedReward(object sender, Reward args)
    {
        if (currentScene == "GameScene")
        {
            int totalCandy = PlayerPrefs.GetInt("totalCandy");
            totalCandy += 500;
            PlayerPrefs.SetInt("totalCandy", totalCandy);
            gameScore.DisplayTotalCandy();
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

                //Log Firebase Event (How much candies do these dudes have?)
                Firebase.Analytics.FirebaseAnalytics.LogEvent(
                Firebase.Analytics.FirebaseAnalytics.EventEarnVirtualCurrency,
                new Firebase.Analytics.Parameter[] {
                new Firebase.Analytics.Parameter(
                Firebase.Analytics.FirebaseAnalytics.ParameterValue, totalCandy),
                new Firebase.Analytics.Parameter(
                Firebase.Analytics.FirebaseAnalytics.ParameterVirtualCurrencyName, "Total Candy"),
                                        }
                                        );

            }
        }

    }
}
