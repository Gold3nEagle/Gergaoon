using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using UnityEngine.SceneManagement;



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




public class AdsScript : MonoBehaviour
{

    private InterstitialAd interstitial;
    private RewardedAd rewardedAd;
    Score gameScore;
    public ScoreManager matchScore;
    Overworld overWorld;
    public EndGameManager endGame;

    // Start is called before the first frame update
    void Start()
    {
        

#if UNITY_ANDROID
        string appId = "ca-app-pub-8350868259993569~2254859297";
#elif UNITY_IPHONE
            string appId = "ca-app-pub-8350868259993569~9193603165";
#else
            string appId = "unexpected_platform";
#endif

        MobileAds.Initialize(appId);

        // Called when the user should be rewarded for interacting with the ad. 

        this.RequestInterstitial();
        this.RequestRewardedAd(); 
         
        if(SceneManager.GetActiveScene().buildIndex == 1)
        gameScore = GameObject.FindGameObjectWithTag("Hat").GetComponent<Score>();
    }

    private void RequestInterstitial()
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


    private void RequestRewardedAd()
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
        if (this.interstitial.IsLoaded())
        {
            this.interstitial.Show();
        }
    }

    public void HandleUserEarnedReward(object sender, Reward args)
    {
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene == "Bejeweled" || currentScene == "GameScene")
        {
            if (endGame.doubleRewards == true)
            {
                int totalCandy = PlayerPrefs.GetInt("totalCandy");
                int totalScore = matchScore.GetScore();
                totalCandy += totalScore;
                PlayerPrefs.SetInt("totalCandy", totalCandy);
                endGame.WatchedRewarded();
            }
            else
            {
                int totalCandy = PlayerPrefs.GetInt("totalCandy");
                totalCandy += 250;
                PlayerPrefs.SetInt("totalCandy", totalCandy);
                if (currentScene == "GameScene")
                {
                    gameScore.DisplayTotalCandy();
                }
                else
                {
                    overWorld = FindObjectOfType<Overworld>();
                    overWorld.DisplayTotalCandy();
                }
            }
        }
        else if (currentScene == "OverWorld")
        {
            overWorld = FindObjectOfType<Overworld>();
            int totalLives = PlayerPrefs.GetInt("totalLives");
            totalLives++;
            PlayerPrefs.SetInt("totalLives", totalLives);
            overWorld.DisplayTotalLives();
        }

    }

    public void WatchRewardedAd()
    {
        if (this.rewardedAd.IsLoaded())
        {
            this.rewardedAd.Show();
        }
    }

}
