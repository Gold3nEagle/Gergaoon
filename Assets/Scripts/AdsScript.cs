using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;



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

    //Also add rewarded videos 


public class AdsScript : MonoBehaviour
{

    private InterstitialAd interstitial;

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
        this.RequestInterstitial();
         
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



    public void ShowInterstitialAd()
    {
        if (this.interstitial.IsLoaded())
        {
            this.interstitial.Show();
        }
    }


}
