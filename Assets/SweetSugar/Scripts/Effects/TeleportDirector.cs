using System;
using System.Collections;
using System.Linq;
using SweetSugar.Scripts.Items;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace SweetSugar.Scripts.Effects
{
    /// <summary>
    /// Teleport effect
    /// </summary>
    public class TeleportDirector : MonoBehaviour
    {
        public PlayableDirector director;
        public TimelineAsset timelineStartTeleport;
        public float endKey = 0.22f;
        public GameObject teleportEvent;
        private TrackAsset key;
        public GameObject mask;
        public Sprite[] sprites;

        // Use this for initialization
        void OnEnable()
        {
            var tracks = timelineStartTeleport.GetOutputTracks();
            key = tracks.First(x => x.name == "Item");
        }

        public void SetTeleport(bool enter)
        {
            if (enter) gameObject.GetComponent<SpriteRenderer>().sprite = sprites[0];
            else gameObject.GetComponent<SpriteRenderer>().sprite = sprites[1];

        }

        public void EnableMask(bool enable)
        {
            mask.SetActive(enable);
        }

        public void StartTeleport(Item item, Action callback)
        {
            // director.SetGenericBinding(key, item.itemAnimTransform.gameObject);
            // if (item.itemAnimTransform.gameObject.GetComponent<Animator>() == null)
            //     item.itemAnimTransform.gameObject.AddComponent<Animator>();
            // item.anim.enabled = false;
            // teleportEvent.SetActive(true);
            director.Play();
            StartCoroutine(Wait(item, callback));
        }

        IEnumerator Wait(Item item, Action callback)
        {
            yield return new WaitUntil(() => director.time >= endKey);
            if (item?.anim != null)
                item.anim.enabled = true;
            if (callback != null) callback();
            yield return new WaitUntil(() => director.time >= director.duration);
            // teleportEvent.SetActive(false);
        }
    }
}
