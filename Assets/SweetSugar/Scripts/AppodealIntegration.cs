using SweetSugar.Scripts.Core;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;
#if APPODEAL
using AppodealAds.Unity.Api;
using AppodealAds.Unity.Common;
#endif
namespace SweetSugar.Scripts
{
    public class AppodealIntegration : UnityEngine.MonoBehaviour
    {
        public static AppodealIntegration THIS;
        public string appKeyAdndroid = "31f94883e4829dbfb9a0d994b077bcf3ba4f04280cd5aec8";
        public string appKeyIOS = "31f94883e4829dbfb9a0d994b077bcf3ba4f04280cd5aec8";
        [SerializeField] private bool testing;

        private void Awake()
        {
            if (THIS == null) THIS = this;
            else if(THIS != this) Destroy(gameObject);
            DontDestroyOnLoad(this);
        }

#if APPODEAL
        private void Start()
        {
            var appKey = "";
            if (Application.platform == RuntimePlatform.Android) appKey = appKeyAdndroid;
            else appKey = appKeyIOS;
            Appodeal.initialize(appKey, Appodeal.REWARDED_VIDEO | Appodeal.INTERSTITIAL, false);
            Appodeal.setRewardedVideoCallbacks(new AppodealListener());
            if(testing)
            Appodeal.setTesting(true);
        }

        public bool IsInterstitialLoaded()
        {
            return Appodeal.isLoaded(Appodeal.INTERSTITIAL);
        }

        public void ShowInterstitial()
        {
            Appodeal.show(Appodeal.INTERSTITIAL);
        }

        public void ShowRewardedAds()
        {
            Appodeal.show(Appodeal.REWARDED_VIDEO);
        }

        public bool IsRewardedLoaded()
        {
            return Appodeal.isLoaded(Appodeal.REWARDED_VIDEO);
        }

//        private void OnGUI()
//        {
//            if (GUILayout.Button("Check Interstitial"))
//            {
//                if(IsInterstitialLoaded()) ShowInterstitial();
//            }
//
//        }


#endif
      
    }
#if APPODEAL

    public class AppodealListener : IRewardedVideoAdListener, IInterstitialAdListener
    {

        public delegate void RewardedShown();

        public static event RewardedShown OnRewardedShown;
        #region Rewarded Video callback handlers
        public void onRewardedVideoLoaded(bool isPrecache) { Debug.Log("Video loaded"); }
        public void onRewardedVideoFailedToLoad() { Debug.Log("Video failed"); }
        public void onRewardedVideoShowFailed()
        {
            { Debug.Log("Video failed"); }
        }

        public void onRewardedVideoShown() { Debug.Log("Video shown"); }
        public void onRewardedVideoClosed(bool finished) { Debug.Log("Video closed"); }

        public void onRewardedVideoFinished(double amount, string name)
        {
            Debug.Log("Reward: " + amount + " " + name);
            InitScript.Instance.ShowReward();
        }
        public void onRewardedVideoExpired() { Debug.Log("Video expired"); }
        public void onRewardedVideoClicked()
        {
            Debug.Log("ad clicked");
        }

        #endregion

        public void onInterstitialLoaded(bool isPrecache)
        {
            Debug.Log("ad loaded");
        }

        public void onInterstitialFailedToLoad()
        {
            Debug.Log("ad failed to load");

        }

        public void onInterstitialShowFailed()
        {
            { Debug.Log("ad failed"); }
        }

        public void onInterstitialShown()
        {
            Debug.Log("ad shown");

        }

        public void onInterstitialClosed()
        {
            Debug.Log("ad closed");

        }

        public void onInterstitialClicked()
        {
            Debug.Log("ad clicked");

        }

        public void onInterstitialExpired()
        {
            Debug.Log("ad expired");

        }
    }
#endif

}