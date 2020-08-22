#if CHARTBOOST_ADS
using ChartboostSDK;
#endif
using System;
using System.Collections.Generic;
using SweetSugar.Scripts.Core;
using UnityEngine;
#if GOOGLE_MOBILE_ADS
using GoogleMobileAds.Api;

#endif
#if UNITY_ADS
using UnityEngine.Advertisements;
#endif
namespace SweetSugar.Scripts.AdsEvents
{
    /// <summary>
    /// Ads manager responsible for initialization and showing
    /// </summary>
    public class AdsManager : UnityEngine.MonoBehaviour
    {
        public static AdsManager THIS;
        //EDITOR: ads events
        public List<AdEvents> adsEvents = new List<AdEvents>();
        //is unity ads enabled
        public bool enableUnityAds;
        //is admob enabled
        public bool enableGoogleMobileAds;
        //is chartboost enabled
        public bool enableChartboostAds;
        //rewarded zone for Unity ads
        public string rewardedVideoZone;
        //admob stuff
#if GOOGLE_MOBILE_ADS
        public InterstitialAd interstitial;
    private AdRequest requestAdmob;
#endif
        public string admobUIDAndroid;
        public string admobRewardedUIDAndroid;
        public string admobUIDIOS;
        public string admobRewardedUIDIOS;
        private AdManagerScriptable adsSettings;

        private void Awake()
        {
            if (THIS == null) THIS = this;
            else if(THIS != this)
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(this);
            adsSettings = Resources.Load<AdManagerScriptable>("Scriptable/AdManagerScriptable");
            adsEvents = adsSettings.adsEvents;
            admobUIDAndroid = adsSettings.admobUIDAndroid;
            admobUIDIOS = adsSettings.admobUIDIOS;
#if UNITY_ADS//2.1.1
            enableUnityAds = true;
#else
        enableUnityAds = false;
#endif
#if CHARTBOOST_ADS//1.6.1
		enableChartboostAds = true;
#else
            enableChartboostAds = false;
#endif
#if GOOGLE_MOBILE_ADS
		enableGoogleMobileAds =true;//1.6.1
#if UNITY_ANDROID
        MobileAds.Initialize(admobUIDAndroid);//2.1.6
        interstitial = new InterstitialAd(admobUIDAndroid);
#elif UNITY_IOS
        MobileAds.Initialize(admobUIDIOS);//2.1.6
        interstitial = new InterstitialAd(admobUIDIOS);
#else
        MobileAds.Initialize(admobUIDAndroid);//2.1.6
		interstitial = new InterstitialAd (admobUIDAndroid);
#endif

        // Create an empty ad request.
        requestAdmob = new AdRequest.Builder().Build();
        // Load the interstitial with the request.
        interstitial.LoadAd(requestAdmob);
        interstitial.OnAdLoaded += HandleInterstitialLoaded;
        interstitial.OnAdFailedToLoad += HandleInterstitialFailedToLoad;
#else
            enableGoogleMobileAds = false;//1.6.1
#endif
        }
        
#if GOOGLE_MOBILE_ADS
	
    public void HandleInterstitialLoaded(object sender, EventArgs args)
    {
        print("HandleInterstitialLoaded event received.");
    }

    public void HandleInterstitialFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        print("HandleInterstitialFailedToLoad event received with message: " + args.Message);
    }
#endif
        
        public bool GetRewardedUnityAdsReady()
        {
#if APPODEAL
        return AppodealIntegration.THIS.IsRewardedLoaded();
#endif
#if UNITY_ADS

            rewardedVideoZone = "rewardedVideo";
            if (Advertisement.IsReady(rewardedVideoZone))
            {
                return true;
            }
            else
            {
                rewardedVideoZone = "rewardedVideoZone";
                if (Advertisement.IsReady(rewardedVideoZone))
                {
                    return true;
                }
            }
#endif

            return false;
        }

        private void OnDisable()
        {
#if GOOGLE_MOBILE_ADS
            if (interstitial != null)
            {
                interstitial.OnAdLoaded -= HandleInterstitialLoaded;
                interstitial.OnAdFailedToLoad -= HandleInterstitialFailedToLoad;
            }

#endif
        }

        public void ShowRewardedAds()
    {
#if APPODEAL
        Debug.Log("show Rewarded ads video in " + LevelManager.THIS.gameStatus);

        if (GetRewardedUnityAdsReady())
        {
            AppodealIntegration.THIS.ShowRewardedAds();
        }
        else{
            #if UNITY_ADS
            Advertisement.Show(rewardedVideoZone, new ShowOptions
            {
                resultCallback = result =>
                {
                    if (result == ShowResult.Finished)
                    {
                        InitScript.Instance.ShowReward();
                    }
                }
            });
            #endif
        }
#elif UNITY_ADS
        Debug.Log("show Rewarded ads video in " + LevelManager.THIS.gameStatus);

        if (GetRewardedUnityAdsReady())
        {
            #if UNITY_ADS
            Advertisement.Show(rewardedVideoZone, new ShowOptions
            {
                resultCallback = result =>
                {
                    if (result == ShowResult.Finished)
                    {
                        InitScript.Instance.ShowReward();
                    }
                }
            });
            #endif
        }
//#elif GOOGLE_MOBILE_ADS//2.2
//        bool stillShow = true;
//#if UNITY_ADS
//        stillShow = !GetRewardedUnityAdsReady ();
//#endif
//        if(stillShow)
//        {
//            Debug.Log("show Admob Rewarded ads video in " + LevelManager.THIS.gameStatus);
//            RewAdmobManager.THIS.ShowRewardedAd(CheckRewardedAds);
//        }
#endif
    }

    public void CheckAdsEvents(GameState state)
    {    
        foreach (var item in adsEvents)
        {
            if (item.gameEvent == state)
            {
                item.calls++;  
                if (item.calls % item.everyLevel == 0)
                    ShowAdByType(item.adType);
            }
        }
    }

    void ShowAdByType(AdType adType)
    {
        if (adType == AdType.AdmobInterstitial && enableGoogleMobileAds)
            ShowAds(false);
        else if (adType == AdType.UnityAdsVideo && enableUnityAds)
            ShowVideo();
        else if (adType == AdType.ChartboostInterstitial && enableChartboostAds)
            ShowAds(true);
        else if(adType == AdType.Appodeal)
            ShowAds(false);
    }

    public void ShowVideo()
    {  
#if UNITY_ADS
        Debug.Log("show Unity ads video in " + LevelManager.THIS.gameStatus);

        if (Advertisement.IsReady("video"))
        {
            Advertisement.Show("video");
        }
        else
        {
            if (Advertisement.IsReady("defaultZone"))
            {
                Advertisement.Show("defaultZone");
            }
        }
#endif
    }


    public void ShowAds(bool chartboost = true)
    {
        #if APPODEAL
        if(AppodealIntegration.THIS.IsInterstitialLoaded())
        {
            Debug.Log("show  Interstitial in " + LevelManager.THIS.gameStatus);
            AppodealIntegration.THIS.ShowInterstitial();
        }
        #endif
        if (chartboost)
        {
#if CHARTBOOST_ADS
            Debug.Log("show Chartboost Interstitial in " + LevelManager.THIS.gameStatus);

            Chartboost.showInterstitial(CBLocation.Default);
            Chartboost.cacheInterstitial(CBLocation.Default);
#endif
        }
        else
        {
#if GOOGLE_MOBILE_ADS
            Debug.Log("show admob Interstitial in " + LevelManager.THIS.gameStatus);
            if (interstitial.IsLoaded())
            {
                interstitial.Show();
#if UNITY_ANDROID
                interstitial = new InterstitialAd(admobUIDAndroid);
#elif UNITY_IOS
                interstitial = new InterstitialAd(admobUIDIOS);
#else
				interstitial = new InterstitialAd (admobUIDAndroid);
#endif

                // Create an empty ad request.
                requestAdmob = new AdRequest.Builder().Build();
                // Load the interstitial with the request.
                interstitial.LoadAd(requestAdmob);
            }
#endif
        }
    }
    }
}