using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SweetSugar.Scripts.Core;
using SweetSugar.Scripts.Level;
using SweetSugar.Scripts.TargetScripts.TargetSystem;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SweetSugar.Scripts.GUI
{
    /// <summary>
    /// Target icons GUI handler. Appears on the top game panel 
    /// </summary>
    public class TargetGUIGroup : MonoBehaviour
    {
        public HorizontalLayoutGroup hg;
        public List<TargetGUI> list = new List<TargetGUI>();
        public TextMeshProUGUI description;
        HorizontalLayoutGroup group;

        private bool levelLoaded;


        void OnEnable()
        {
            DisableImages();
            levelLoaded = false;
            StartCoroutine(WaitForTarget());
            LevelManager.OnLevelLoaded += OnLevelLoaded;
            if (LevelManager.GetGameStatus() > GameState.PrepareGame)
                OnLevelLoaded();
        }

        private void DisableImages()
        {
            ClearTargets();
            description.gameObject.SetActive(false);
            foreach (var item in list)
            {
                item.gameObject.SetActive(false);
            }
        }

        private void OnDisable()
        {

            LevelManager.OnLevelLoaded -= OnLevelLoaded;

        }

        private void OnLevelLoaded()
        {
            group = GetComponent<HorizontalLayoutGroup>();
            if (group != null)
            {
                if (LevelManager.THIS.levelData.target != null && LevelManager.THIS.levelData.target.name == "JellyBlock")
                { group.spacing = 50; /*description.gameObject.SetActive(true);*/ }
                else
                { group.spacing = 0; /*description.gameObject.SetActive(false);*/ }

            }
            levelLoaded = true;
        }

        IEnumerator WaitForTarget()
        {
            yield return new WaitUntil(() => LevelManager.THIS.levelLoaded);
            yield return new WaitUntil(() => LevelData.THIS.GetTargetSprites().Length > 0);

            ClearTargets();
            SetTargets();
        }

        void SetTargets()
        {

        var sprites = LevelManager.THIS.levelData.GetTargetSprites();
        list[0].SetSprite(sprites?[0]);
        SetDescription(LevelManager.THIS.levelData.target.GetDescription());

            if (sprites != null)
            {
                for (var i = 0; i < sprites.Length; i++)
                {
                    // var targetGUI = Instantiate(list[0].gameObject, gameObject.transform);
                    // list.Add(targetGUI.GetComponent<TargetGUI>());
                    list[i].SetSprite(sprites[i]);
                    if (LevelManager.THIS.levelData.subTargetsContainers.Any(x => x.extraObject != null && x.extraObject.name == sprites[i].name))
                        list[i].color = LevelManager.THIS.levelData.subTargetsContainers.First(x => x.extraObject.name == sprites[i].name).color;
                    list[i].gameObject.SetActive(true);
                }
            }
            // if (LevelManager.THIS.levelData.target.name == "JellyBlock")
            // { list[0].gameObject.SetActive(true); description.gameObject.SetActive(true); }
            // GetComponentInParent<TargetGUIGroup>().SetPadding();
        }

        private void SetDescription(string descr)
        {
            description.text = descr;
            if (descr != "")
            {
                // description.gameObject.SetActive(true);
                hg.padding.left = 58;
                hg.padding.right = 63;
            }
        }

        void ClearTargets()
        {
            hg.padding.left = 10;
            hg.padding.right = 10;

            description.gameObject.SetActive(false);
            // for (var i = 1; i < list.Count; i++)
            // {
            // Destroy(list[i].gameObject);
            // list.Remove(list[i]);
            // }
        }

        private void SetPadding()
        {
            if (list.Count == 2)
            {
                hg.padding.left = 150;
                hg.padding.right = 150;
            }

        }
    }
}
