using UnityEngine;

namespace SweetSugar.Scripts.TargetScripts.TargetSystem
{
    /// <summary>
    /// Target component adding for object
    /// </summary>
    public class TargetComponent : MonoBehaviour
    {
        private bool quit;

        public delegate void DestroyDelegate(GameObject obj);
        public static event DestroyDelegate OnDestroyEvent;

        // void OnEnable()
        // {
        //     this.OnDestroyEvent += LevelManager.This.target.DestroyEvent;
        // }

        // void OnDisable()
        // {
        //     this.OnDestroyEvent -= LevelManager.This.target.DestroyEvent;
        // }

        void OnApplicationQuit()
        {
            quit = true;
        }

        void OnDestroy()
        {
            if (!quit) OnDestroyEvent(gameObject);
        }
    }
}