using System.Linq;
using SweetSugar.Scripts.Integrations.Network;
using SweetSugar.Scripts.MapScripts;
using UnityEngine;
using UnityEngine.UI;

namespace SweetSugar.Scripts.GUI.Avatar
{
    /// <summary>
    /// Player avatar. Loading picture and restore it after back to map scene
    /// </summary>
    public class PlayerAvatar : MonoBehaviour, IAvatarLoader
    {
        public Image image;

        void Start()
        {
            image.enabled = false;
        }

#if PLAYFAB || GAMESPARKS
        void OnEnable () {
            NetworkManager.OnPlayerPictureLoaded += ShowPicture;
            LevelsMap.LevelReached += OnLevelReached;
            if(NetworkManager.profilePic != null) ShowPicture();
        }

        void OnDisable () {
            NetworkManager.OnPlayerPictureLoaded -= ShowPicture;
            LevelsMap.LevelReached -= OnLevelReached;

        }


#endif
        public void ShowPicture()
        {
            image.sprite = NetworkManager.profilePic;
            image.enabled = true;
        }

        private void OnLevelReached(object sender, LevelReachedEventArgs e)
        {
            Debug.Log(string.Format("Level {0} reached.", e.Number));
        }

    }
}
