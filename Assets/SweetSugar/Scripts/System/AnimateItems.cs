using System;
using System.Collections;
using SweetSugar.Scripts.Core;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SweetSugar.Scripts.System
{
    /// <summary>
    /// Blocks and ingredients animation, fly up to GUI
    /// </summary>
    public class AnimateItems : MonoBehaviour
    {
        public void InitAnimation(GameObject obj, Vector2 pos, Vector2 scale, Action callBack, Sprite sprite=null)
        {
            // item = CopyComp.Copy(obj.GetComponent<ISpriteRenderer>().GetSpriteRenderer(), item);
            var colorableComponent = obj.GetComponent<IColorableComponent>();
            SpriteRenderer spr = null;
            if(!sprite)
            {
                if (!colorableComponent) spr = obj.GetComponent<SpriteRenderer>();
                else spr = obj.GetComponent<IColorableComponent>().directSpriteRenderer;
                if (spr == null) spr = obj.GetComponentInChildren<SpriteRenderer>(); 
                sprite = spr.sprite;
            }
            var sprRenderer = gameObject.AddComponent<SpriteRenderer>();
            sprRenderer.sprite = sprite;
            sprRenderer.sortingLayerName = "UI";
            sprRenderer.sortingOrder = 10;

            gameObject.transform.position = obj.transform.position;
            gameObject.transform.localScale = scale;
            StartCoroutine(StartAnimateIngredient(gameObject, pos, () => { if (callBack != null) callBack(); }));
        }

        void OnFinish(GameObject item)
        {
            LevelManager.Destroy(item);
        }

        IEnumerator StartAnimateIngredient(GameObject item, Vector2 pos, Action callBack)
        {
            var curveX = new AnimationCurve(new Keyframe(0, item.transform.localPosition.x), new Keyframe(0.4f, pos.x));
            var curveY = new AnimationCurve(new Keyframe(0, item.transform.localPosition.y), new Keyframe(0.5f, pos.y));
            curveY.AddKey(0.2f, item.transform.localPosition.y + Random.Range(-2, 0.5f));
            var startTime = Time.time;
            var speed = Random.Range(0.4f, 0.6f);
            float distCovered = 0;
            while (distCovered < 0.5f)
            {
                distCovered = (Time.time - startTime) * speed;
                item.transform.localPosition = new Vector3(curveX.Evaluate(distCovered), curveY.Evaluate(distCovered), 0);
                item.transform.Rotate(Vector3.back, Time.deltaTime * 1000);
                item.transform.localScale = Vector3.Lerp(item.transform.localScale, item.transform.localScale / 2, Time.deltaTime * 1);
                yield return new WaitForFixedUpdate();
            }
            //     SoundBase.Instance.audio.PlayOneShot(SoundBase.Instance.getStarIngr);
            LevelManager.Destroy(item);
            if (callBack != null)
                callBack();
            Destroy(gameObject);

        }
    }
}