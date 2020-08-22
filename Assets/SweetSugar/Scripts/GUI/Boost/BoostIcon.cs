using System;
using JetBrains.Annotations;
using SweetSugar.Scripts.Core;
using SweetSugar.Scripts.System;
using TMPro;
using UnityEngine;

namespace SweetSugar.Scripts.GUI.Boost
{
    /// <summary>
    /// Boost icon handler on GUI
    /// </summary>
    public class BoostIcon : MonoBehaviour
    {
        public TextMeshProUGUI boostCount;
        public BoostType type;
        public GameObject plus;
        public GameObject counter;
        public GameObject check;
        private BoostProduct boostProduct;

        private bool checkOn;
        private GameObject Lock;
        private GameObject Indicator;
        private static BoostShop BoostShop;

        private void Awake()
        {
            Lock = transform.Find("Lock")?.gameObject;
            Indicator = transform.Find("Indicator")?.gameObject;
            BoostShop = MenuReference.THIS.BoostShop.gameObject.GetComponent<BoostShop>();
            //		if (check != null) return;
            //		check = Instantiate(Resources.Load("Prefabs/Check")) as GameObject;
            //		check.transform.SetParent(transform.Find("Indicator"));
            //		check.transform.localScale = Vector3.one;
            //		check.GetComponent<RectTransform>().anchoredPosition = new Vector2(2,-67);
            //		check.SetActive(false);
        }

        private void OnEnable()
        {
            if (name == "Main Camera") return;
            if (LevelManager.THIS == null) return;
            if (LevelManager.THIS.gameStatus == GameState.Map)
                check.SetActive(false);
            //if (!LevelManager.This.enableInApps)
            //gameObject.SetActive(false);
            FindBoostProduct();
            ShowPlus(BoostCount() <= 0);
            boostCount.text = "" + PlayerPrefs.GetInt("" + type);

        }

        private void FindBoostProduct()
        {
            boostProduct = BoostShop.boostProducts[(int)type];

        }

        [UsedImplicitly]
        public void ActivateBoost()
        {
            if (LevelManager.THIS.tutorialTime) return;
            SoundBase.Instance.PlayOneShot( SoundBase.Instance.click );
            //		if (LevelManager.This.ActivatedBoost == this)//TODO: check ingame boosts
            if (checkOn || LevelManager.THIS.ActivatedBoost == this)
            {
                UnCheckBoost();
                return;
            }
            if (IsLocked() || checkOn || (LevelManager.THIS.gameStatus != GameState.Playing && LevelManager.THIS.gameStatus != GameState.Map))
                return;
            if (BoostCount() > 0)
            {
                if (type != BoostType.MulticolorCandy && type != BoostType.Packages && type != BoostType.Stripes && type != BoostType.Marmalade && !LevelManager.THIS.DragBlocked)//for in-game boosts
                    LevelManager.THIS.ActivatedBoost = this;
                else
                    Check(true);

            }
            else
            {
                OpenBoostShop(boostProduct, ActivateBoost);
            }

            if (boostCount != null)
                boostCount.text = "" + BoostCount();
            ShowPlus(BoostCount() <= 0);
        }

        private void UnCheckBoost()
        {
            checkOn = false;
            if (LevelManager.THIS.gameStatus == GameState.Map)
                Check(false);
            else
            {
                LevelManager.THIS.activatedBoost = null;
                LevelManager.THIS.UnLockBoosts();//for in-game boosts
            }
        }

        public void InitBoost()
        {
            check.SetActive(false);
            plus.SetActive(true);
            LevelManager.THIS.BoostColorfullBomb = 0;
            LevelManager.THIS.BoostPackage = 0;
            LevelManager.THIS.BoostStriped = 0;
            if (boostCount != null)
                boostCount.text = "" + PlayerPrefs.GetInt("" + type);
            checkOn = false;
        }

        private void Check(bool checkIt)
        {
            switch (type)
            {
                case BoostType.MulticolorCandy:
                    LevelManager.THIS.BoostColorfullBomb = checkIt ? 1 : 0;
                    break;
                case BoostType.Packages:
                    LevelManager.THIS.BoostPackage = checkIt ? 2 : 0;
                    break;
                case BoostType.Stripes:
                    LevelManager.THIS.BoostStriped = checkIt ? 2 : 0;
                    break;
                case BoostType.Marmalade:
                    LevelManager.THIS.BoostMarmalade = checkIt ? 1 : 0;
                    break;
                case BoostType.ExtraMoves:
                    if (checkIt) checkIt = false;
                    break;
                case BoostType.ExtraTime:
                    break;
                case BoostType.Bomb:
                    break;
                case BoostType.FreeMove:
                    break;
                case BoostType.ExplodeArea:
                    break;
                case BoostType.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            checkOn = checkIt;
            check.SetActive(checkIt);
            plus.SetActive(!checkIt);
            //InitScript.Instance.SpendBoost(type);
        }

        public void LockBoost()
        {
            Lock?.SetActive(true);
            Indicator?.SetActive(false);
        }

        public void UnLockBoost()
        {
            Lock?.SetActive(false);
            Indicator?.SetActive(true);
            if (boostCount != null)
                boostCount.text = "" + BoostCount();
            ShowPlus(BoostCount() <= 0);

        }

        private bool IsLocked()
        {
            return Lock.activeSelf;
        }

        private int BoostCount()
        {
            // Debug.Log("boost count " + PlayerPrefs.GetInt("" + type));
            return PlayerPrefs.GetInt("" + type);
        }

        private static void OpenBoostShop(BoostProduct boost, Action callback)
        {
            BoostShop.SetBoost(boost, callback);
        }

        private void ShowPlus(bool show)
        {
            plus?.gameObject.SetActive(show);
            counter?.gameObject.SetActive(!show);

        }


    }
}
