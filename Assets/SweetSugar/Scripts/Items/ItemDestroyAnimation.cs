using System.Collections;
using SweetSugar.Scripts.Blocks;
using SweetSugar.Scripts.Core;
using SweetSugar.Scripts.Effects;
using UnityEngine;

namespace SweetSugar.Scripts.Items
{
    /// <summary>
    /// Package destroy animation helper
    /// </summary>
    public class ItemDestroyAnimation : MonoBehaviour
    {
        //    public PlayableDirector director;
        //    public PlayableAsset[] timelines;
        private Item item;
        private bool started;

        private void Start()
        {
            item = GetComponent<Item>();
        }

        public void DestroyPackage(Item item1)
        {
            if (started) return;
            started = true;
            var thisItem = GetComponent<Item>();

            GameObject go = Instantiate(Resources.Load("Prefabs/Effects/_ExplosionAround") as GameObject);//LevelManager.THIS.GetExplAroundPool();
            if (go != null)
            {
                go.transform.position = transform.position;
                var explosionAround = go.GetComponent<ExplAround>();
                explosionAround.item = thisItem;
                go.SetActive(true);
            }

            var square = item1.square;
            square.Item = item1;

//        item.anim.enabled = true;
//        var audioBinding = item.director.playableAsset.outputs.Select(i => i.sourceObject).OfType<AudioTrack>().FirstOrDefault();
//        item.director.SetGenericBinding(audioBinding, SoundBase.Instance);
//        item.director.Play();
            StartCoroutine(OnPackageAnimationFinished(item1));
        }

        private IEnumerator OnPackageAnimationFinished(Item item1)
        {
            var square = item1.square;
//        yield return new WaitUntil(() => item.director.time >= .35f || item.director.state == PlayState.Paused);
            yield return new WaitForSeconds(.35f);
            DestroyItems(item1, square);
            yield return new WaitForSeconds(0.2f);
            item.HideSprites(true);
//        yield return new WaitUntil(() => item.director.time >= item.director.duration || item.director.state == PlayState.Paused);
            yield return new WaitForSeconds(0.5f);
            item.DestroyBehaviour();
            started = false;

        }

        private void DestroyItems(Item item1, Square square)
        {
            LevelManager.THIS.field.DestroyItemsAround(square, item);
            var sqList = LevelManager.THIS.GetSquaresAroundSquare(square);
            LevelManager.THIS.levelData.GetTargetObject().CheckSquares(sqList.ToArray());
            item1.destroying = false;
            item.square.Item = null;
        }
    }
}

