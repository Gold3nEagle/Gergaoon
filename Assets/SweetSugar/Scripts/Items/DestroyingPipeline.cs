using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SweetSugar.Scripts.Core;
using UnityEngine;

namespace SweetSugar.Scripts.Items
{
    /// <summary>
    /// Destroy processing. Delayed destroying items from array
    /// </summary>
    public class DestroyingPipeline : MonoBehaviour
    {
        public static DestroyingPipeline THIS;
        public List<DestroyBunch> pipeline = new List<DestroyBunch>();

        private void Start()
        {
            if (THIS == null)
                THIS = this;
            else if (THIS != this)
                Destroy(gameObject);

            StartCoroutine(DestroyingPipelineCor());
        }

        public void DestroyItems(List<Item> items, Delays delays, Action callback)
        {
            if (LevelManager.THIS.DebugSettings.DestroyLog)
            {
                foreach (var item in items)
                {
                    DebugLogKeeper.Log("Add to pipeline " + item, DebugLogKeeper.LogType.Destroying);

                }
            }
            var bunch = new DestroyBunch();
            bunch.items = items.ToList();
            bunch.callback = callback;
            bunch.delays = delays;
            pipeline.Add(bunch);
        }

        private IEnumerator DestroyingPipelineCor()
        {
            while (true)
            {
                for (var i = 0; i < pipeline.Count; i++)
                {
                    var bunch = pipeline[i];
                    if (bunch.items.Any())
                        LevelManager.THIS.levelData.GetTargetObject().CheckSquares(bunch.items.Select(x => x.square).ToArray());

                    if (bunch.delays.before != null)
                        yield return bunch.delays.before;
                    for (var j = 0; j < bunch.items.Count; j++)
                    {
                        var item = bunch.items[j];
                        if (bunch.delays.beforeevery != null)
                            yield return Activator.CreateInstance(bunch.delays.beforeevery.GetType());
                        if (item != null) item.DestroyItem(true);
                        if (bunch.delays.afterevery != null)
                        {
                            yield return Activator.CreateInstance(bunch.delays.afterevery.GetType());
                        }
                    }
                    if (bunch.delays.after != null)
                        yield return bunch.delays.after;
                    pipeline.Remove(bunch);
                    if (bunch.callback != null)
                        bunch.callback();
                }
                yield return new WaitForFixedUpdate();

            }
        }

    }

    public class DestroyBunch
    {
        public List<Item> items;
        public Action callback;
        public Delays delays;
    }

    public struct Delays
    {
        public CustomYieldInstruction before, beforeevery, afterevery, after;

    }
}