using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using SweetSugar.Scripts.AdsEvents;
using SweetSugar.Scripts.Core;
using SweetSugar.Scripts.GUI.Boost;
using SweetSugar.Scripts.GUI.Purchasing;
using SweetSugar.Scripts.Level;
using SweetSugar.Scripts.MapScripts.StaticMap.Editor;
using SweetSugar.Scripts.System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#if UNITY_ADS
using UnityEngine.Advertisements;
#endif





namespace SweetSugar.Scripts.GUI
{
    /// <summary>
    /// Popups animation event manager
    /// </summary>
    public class AnimationEventManager : MonoBehaviour
    {
        public bool PlayOnEnable = true;
        bool WaitForPickupFriends;

        bool WaitForAksFriends;
        Dictionary<string, string> parameters;

        void OnEnable()
        {
            if (PlayOnEnable)
            {
                //            SoundBase.Instance.PlayOneShot(SoundBase.Instance.swish[0]);

            }
            if (name == "MenuPlay")
            {

            }

            if (name == "PrePlay")
            {
                // GameObject
            }
            if (name == "PreFailed")
            {
//            SoundBase.Instance.PlayOneShot(SoundBase.Instance.gameOver[0]);
                transform.Find("Banner/Buttons/Video").gameObject.SetActive(false);
                transform.Find("Banner/Buttons/Buy").GetComponent<Button>().interactable = true;

                GetComponent<Animation>().Play();
            }

            if (name == "Settings" || name == "MenuPause")
            {
                if (PlayerPrefs.GetInt("Sound") < 1)
                {
                    transform.Find("Sound/Sound/SoundOff").gameObject.SetActive(true);
                    transform.Find("Sound/Sound").GetComponent<Image>().enabled = false;
                }
                else
                {
                    transform.Find("Sound/Sound/SoundOff").gameObject.SetActive(false);
                    transform.Find("Sound/Sound").GetComponent<Image>().enabled = true;
                }

                if (PlayerPrefs.GetInt("Music") < 1)
                {
                    transform.Find("Music/Music/MusicOff").gameObject.SetActive(true);
                    transform.Find("Music/Music").GetComponent<Image>().enabled = false;
                }
                else
                {
                    transform.Find("Music/Music/MusicOff").gameObject.SetActive(false);
                    transform.Find("Music/Music").GetComponent<Image>().enabled = true;
                }

            }

            if (name == "GemsShop")
            {
                var tr = GetComponent<SweetSugarPacks>().packs;
                for (var i = 0; i < LevelManager.THIS.gemsProducts.Count; i++)
                {
                    var item = tr[i];
                    item.Find("Count").GetComponent<TextMeshProUGUI>().text = "" + LevelManager.THIS.gemsProducts[i].count;
                    item.Find("Buy/Price").GetComponent<TextMeshProUGUI>().text = "" + LevelManager.THIS.gemsProducts[i].price;
                }
            }
            if (name == "MenuComplete")
            {
                for (var i = 1; i <= 3; i++)
                {
                    transform.Find("Image").Find("Star" + i).gameObject.SetActive(false);
                }

            }
            if (transform.Find("Image/Video") != null)
            {
#if UNITY_ADS
            AdsManager.THIS.rewardedVideoZone = "rewardedVideo";

			if (!AdsManager.THIS.enableUnityAds || !AdsManager.THIS.GetRewardedUnityAdsReady ())
				transform.Find ("Image/Video").gameObject.SetActive (false);
#else
                transform.Find("Image/Video").gameObject.SetActive(false);
#endif
            }

        }

        void Update()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                if (name == "MenuPlay" || name == "Settings" || name == "BoostInfo" || name == "GemsShop" || name == "LiveShop" || name == "BoostShop" || name == "Reward")
                    CloseMenu();
            }
        }

        /// <summary>
        /// show rewarded ads
        /// </summary>
        public void ShowAds()
        {
            if (name == "GemsShop")
                InitScript.Instance.currentReward = RewardsType.GetGems;
            else if (name == "LiveShop")
                InitScript.Instance.currentReward = RewardsType.GetLifes;
            else if (name == "PreFailed")
                InitScript.Instance.currentReward = RewardsType.GetGoOn;
            AdsManager.THIS.ShowRewardedAds();
            CloseMenu();
        }

        /// <summary>
        /// Open rate store
        /// </summary>
        public void GoRate()
        {

#if UNITY_ANDROID
        Application.OpenURL(InitScript.Instance.RateURL);
#elif UNITY_IOS
        Application.OpenURL(InitScript.Instance.RateURLIOS);
#endif
            PlayerPrefs.SetInt("Rated", 1);
            PlayerPrefs.Save();
            CloseMenu();
        }

        void OnDisable()
        {
            if (transform.Find("Image/Video") != null)
            {
                transform.Find("Image/Video").gameObject.SetActive(true);
            }

            //if( PlayOnEnable )
            //{
            //    if( !GetComponent<SequencePlayer>().sequenceArray[0].isPlaying )
            //        GetComponent<SequencePlayer>().sequenceArray[0].Play
            //}
        }

        /// <summary>
        /// Event on finish animation
        /// </summary>
        public void OnFinished()
        {
            if (name == "MenuComplete")
            {
                StartCoroutine(MenuComplete());
                StartCoroutine(MenuCompleteScoring());
            }
            if (name == "MenuPlay")
            {
                //            InitScript.Instance.currentTarget = InitScript.Instance.targets[PlayerPrefs.GetInt( "OpenLevel" )];
                transform.Find("Image/Boosters/Boost1").GetComponent<BoostIcon>().InitBoost();
                transform.Find("Image/Boosters/Boost2").GetComponent<BoostIcon>().InitBoost();
                transform.Find("Image/Boosters/Boost3").GetComponent<BoostIcon>().InitBoost();

            }
            if (name == "MenuPause")
            {
                if (LevelManager.THIS.gameStatus == GameState.Playing)
                    LevelManager.THIS.gameStatus = GameState.Pause;
            }

            if (name == "MenuFailed")
            {
                if (LevelManager.Score < LevelManager.THIS.levelData.star1)
                {
                    TargetCheck(false, 2);
                }
                else
                {
                    TargetCheck(true, 2);
                }

            }
            if (name == "PrePlay")
            {
                CloseMenu();
                LevelManager.THIS.gameStatus = GameState.Tutorial;
                if(LevelManager.THIS.levelData.limitType == LIMIT.TIME) SoundBase.Instance.PlayOneShot(SoundBase.Instance.timeOut);

            }
            if (name == "PreFailed")
            {
                transform.Find("Banner/Buttons/Video").gameObject.SetActive(false);
                CloseMenu();
            }

            if (name.Contains("gratzWord"))
                gameObject.SetActive(false);
            if (name == "NoMoreMatches")
                gameObject.SetActive(false);
            // if (name == "CompleteLabel")
            //     gameObject.SetActive(false);

        }

        void TargetCheck(bool check, int n = 1)
        {
            var TargetCheck = transform.Find("Image/TargetCheck" + n);
            var TargetUnCheck = transform.Find("Image/TargetUnCheck" + n);
            TargetCheck.gameObject.SetActive(check);
            TargetUnCheck.gameObject.SetActive(!check);
        }

        /// <summary>
        /// Shows rewarded ad button in Prefailed popup 
        /// </summary>
        [UsedImplicitly]
        public void WaitForGiveUp()
        {
            if (name == "PreFailed" && LevelManager.THIS.gameStatus != GameState.Playing)
            {
                GetComponent<Animation>()["bannerFailed"].speed = 0;
#if UNITY_ADS

			if (AdsManager.THIS.enableUnityAds) {

				if (AdsManager.THIS.GetRewardedUnityAdsReady ()) {
					transform.Find ("Banner/Buttons/Video").gameObject.SetActive (true);
				}
			}
#endif
            }
        }

        /// <summary>
        /// Complete popup animation
        /// </summary>
        IEnumerator MenuComplete()
        {
            for (var i = 1; i <= LevelManager.THIS.stars; i++)
            {
                //  SoundBase.Instance.audio.PlayOneShot( SoundBase.Instance.scoringStar );
                transform.Find("Image").Find("Star" + i).gameObject.SetActive(true);
                SoundBase.Instance.PlayOneShot(SoundBase.Instance.star[i - 1]);
                yield return new WaitForSeconds(0.5f);
            }
        }

        /// <summary>
        /// Complete popup animation
        /// </summary>
        IEnumerator MenuCompleteScoring()
        {
            var scores = transform.Find("Image").Find("Score").GetComponent<TextMeshProUGUI>();
            for (var i = 0; i <= LevelManager.Score; i += 500)
            {
                scores.text = "" + i;
                // SoundBase.Instance.audio.PlayOneShot( SoundBase.Instance.scoring );
                yield return new WaitForSeconds(0.00001f);
            }
            scores.text = "" + LevelManager.Score;
        }

        /// <summary>
        /// SHows info popup
        /// </summary>
        public void Info()
        {
            MenuReference.THIS.Tutorials.gameObject.SetActive(false);
            MenuReference.THIS.Tutorials.gameObject.SetActive(true);
            OpneMenu(gameObject);
        }



        public void PlaySoundButton()
        {

        }

        public void OpneMenu(GameObject menu)
        {
            if (menu.activeSelf)
                menu.SetActive(false);
            else
                menu.SetActive(true);
        }

        public IEnumerator Close()
        {
            yield return new WaitForSeconds(0.5f);
        }

        public void CloseMenu()
        {
            if (gameObject.name == "MenuPreGameOver")
            {
                ShowGameOver();
            }
            if (gameObject.name == "MenuComplete")
            {
//            LevelManager.THIS.gameStatus = GameState.Map;
                PlayerPrefs.SetInt("OpenLevel", LevelManager.THIS.currentLevel + 1);
                CrosssceneData.openNextLevel = true;
                SceneManager.LoadScene(Resources.Load<MapSwitcher>("Scriptable/MapSwitcher").GetSceneName());
            }
            if (gameObject.name == "MenuFailed")
            {
                LevelManager.THIS.gameStatus = GameState.Map;
            }

            if (SceneManager.GetActiveScene().name == "game")
            {
                if (LevelManager.THIS.gameStatus == GameState.Pause)
                {
                    LevelManager.THIS.gameStatus = GameState.WaitAfterClose;

                }
            }

            if (gameObject.name == "Settings" && LevelManager.GetGameStatus() != GameState.Map)
            {
                BackToMap();
            }
            else if (gameObject.name == "Settings" && LevelManager.GetGameStatus() == GameState.Map)
                SceneManager.LoadScene("main");

            //        SoundBase.Instance.PlayOneShot(SoundBase.Instance.swish[1]);

            gameObject.SetActive(false);
        }

        public void SwishSound()
        {
            SoundBase.Instance.PlayOneShot(SoundBase.Instance.swish[1]);

        }

        public void ShowInfo()
        {
            GameObject.Find("CanvasGlobal").transform.Find("BoostInfo").gameObject.SetActive(true);

        }

        public void Play()
        {
            SoundBase.Instance.PlayOneShot(SoundBase.Instance.click);
            if (gameObject.name == "MenuPreGameOver")
            {
                if (InitScript.Gems >= 12)
                {
                    InitScript.Instance.SpendGems(12);
                    //                LevelData.LimitAmount += 12;
                    LevelManager.THIS.gameStatus = GameState.WaitAfterClose;
                    gameObject.SetActive(false);

                }
                else
                {
                    BuyGems();
                }
            }
            else if (gameObject.name == "MenuFailed")
            {
                LevelManager.THIS.gameStatus = GameState.Map;
            }
            else if (gameObject.name == "MenuPlay")
            {
                GUIUtils.THIS.StartGame();
                CloseMenu();
            }
            else if (gameObject.name == "MenuPause")
            {
                CloseMenu();
                LevelManager.THIS.gameStatus = GameState.Playing;
            }
        }

        public void PlayTutorial()
        {
            LevelManager.THIS.gameStatus = GameState.Playing;
            //    mainscript.Instance.dropDownTime = Time.time + 0.5f;
            //        CloseMenu();
        }

        public void BackToMap()
        {
            Time.timeScale = 1;
            // LevelManager.THIS.gameStatus = GameState.GameOver;
            // CloseMenu();
            gameObject.SetActive(false);
            LevelManager.THIS.gameStatus = GameState.Map;
            SceneManager.LoadScene(Resources.Load<MapSwitcher>("Scriptable/MapSwitcher").GetSceneName());
        }

        public void Next()
        {
            SoundBase.Instance.PlayOneShot(SoundBase.Instance.click);

            CloseMenu();
        }

        [UsedImplicitly]
        public void Again()
        {
            SoundBase.Instance.PlayOneShot(SoundBase.Instance.click);
            GameObject gm = new GameObject();
            gm.AddComponent<RestartLevel>();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        }

        public void BuyGems()
        {

            SoundBase.Instance.PlayOneShot(SoundBase.Instance.click);
            MenuReference.THIS.GemsShop.gameObject.SetActive(true);
        }

        [UsedImplicitly]
        public void Buy(GameObject pack)
        {
            var i = pack.transform.GetSiblingIndex();
            InitScript.waitedPurchaseGems = int.Parse(pack.transform.Find("Count").GetComponent<TextMeshProUGUI>().text.Replace("x ", ""));
#if UNITY_WEBPLAYER || UNITY_WEBGL
            InitScript.Instance.PurchaseSucceded();
            CloseMenu();
            return;
#endif
#if UNITY_PURCHASING && UNITY_INAPPS
            UnityInAppsIntegration.THIS.BuyProductID(LevelManager.THIS.InAppIDs[i]);
#endif

            CloseMenu();

        }

        public void BuyLifeShop()
        {

            SoundBase.Instance.PlayOneShot(SoundBase.Instance.click);
            if (InitScript.lifes < InitScript.Instance.CapOfLife)
                MenuReference.THIS.LiveShop.gameObject.SetActive(true);

        }

        public void BuyLife(GameObject button)
        {
            SoundBase.Instance.PlayOneShot(SoundBase.Instance.click);
            if (InitScript.Gems >= int.Parse(button.transform.Find("Price").GetComponent<TextMeshProUGUI>().text))
            {
                InitScript.Instance.SpendGems(int.Parse(button.transform.Find("Price").GetComponent<TextMeshProUGUI>().text));
                InitScript.Instance.RestoreLifes();
                CloseMenu();
            }
            else
            {
                MenuReference.THIS.GemsShop.gameObject.SetActive(true);
            }

        }

        public void BuyFailed(GameObject button)
        {
//        if (GetComponent<Animation>()["bannerFailed"].speed == 0)
            {
                if (InitScript.Gems >= LevelManager.THIS.FailedCost)
                {
                    InitScript.Instance.SpendGems(LevelManager.THIS.FailedCost);
                    button.GetComponent<Button>().interactable = false;
                    GoOnFailed();
                    GetComponent<Animation>()["bannerFailed"].speed = 1;  
                }
                else
                {
                    MenuReference.THIS.GemsShop.gameObject.SetActive(true);
                }
            }
        }

        public void GoOnFailed()
        {
            GetComponent<PreFailed>().Continue();
        }

        [UsedImplicitly]
        public void GiveUp()
        {
            GetComponent<PreFailed>().Close();
        }

        void ShowGameOver()
        {
            SoundBase.Instance.PlayOneShot(SoundBase.Instance.gameOver[1]);

            GameObject.Find("Canvas").transform.Find("MenuGameOver").gameObject.SetActive(true);
            gameObject.SetActive(false);

        }

        #region boosts

        public void BuyBoost(BoostType boostType, int price, int count, Action callback)
        {
            SoundBase.Instance.PlayOneShot(SoundBase.Instance.click);
            if (InitScript.Gems >= price)
            {
                InitScript.Instance.SpendGems(price);
                InitScript.Instance.BuyBoost(boostType, price, count);
                callback?.Invoke();
                //InitScript.Instance.SpendBoost(boostType);
                CloseMenu();
            }
            else
            {
                BuyGems();
            }
        }

        #endregion

        public void SoundOff(GameObject Off)
        {
            var on = Off.transform.parent;

            if (!Off.activeSelf)
            {
                //            SoundBase.Instance.volume = 0;
                SoundBase.Instance.audioMixer.SetFloat("SoundVolume", -80);
                on.GetComponent<Image>().enabled = false;
                Off.SetActive(true);
            }
            else
            {
                //            SoundBase.Instance.volume = 1;
                SoundBase.Instance.audioMixer.SetFloat("SoundVolume", 1);

                Off.SetActive(false);
                on.GetComponent<Image>().enabled = true;

            }

            float vol;
            SoundBase.Instance.audioMixer.GetFloat("SoundVolume", out vol);
            PlayerPrefs.SetInt("Sound", (int)vol);
            PlayerPrefs.Save();

        }

        public void MusicOff(GameObject Off)
        {
            var on = Off.transform.parent;
            if (!Off.activeSelf)
            {
                //            GameObject.Find("Music").GetComponent<AudioSource>().volume = 0;
                MusicBase.Instance.audioMixer.SetFloat("MusicVolume", -80);
                on.GetComponent<Image>().enabled = false;

                Off.SetActive(true);
            }
            else
            {
                //            GameObject.Find("Music").GetComponent<AudioSource>().volume = 1;
                MusicBase.Instance.audioMixer.SetFloat("MusicVolume", 1);

                Off.SetActive(false);
                on.GetComponent<Image>().enabled = true;

            }
            float vol;
            MusicBase.Instance.audioMixer.GetFloat("MusicVolume", out vol);
            PlayerPrefs.SetInt("Music", (int)vol);
            PlayerPrefs.Save();

        }

    }
}
