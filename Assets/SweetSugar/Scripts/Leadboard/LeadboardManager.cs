using System.Collections;
using System.Collections.Generic;
using SweetSugar.Scripts.Integrations.Network;
using UnityEngine;
using UnityEngine.UI;
#if FACEBOOK
using Facebook.Unity;

#endif


namespace SweetSugar.Scripts.Leadboard
{
    /// <summary>
    /// Leadboard manager. Creates player icons on the leadboard
    /// </summary>
    public class LeadboardManager : MonoBehaviour
    {
        public GameObject playerIconPrefab;
        public List<LeadboardObject> playerIconsList = new List<LeadboardObject>();

        void OnEnable()
        {
            GetComponent<Image>().enabled = false;
            ShowIcons(false);

#if PLAYFAB || GAMESPARKS
            //PlayFabManager.OnLevelLeadboardLoaded += ShowLeadboard;
            NetworkManager.friendsManager.GetLeadboardOnLevel();

            NetworkManager.leadboardList.Clear();
            StartCoroutine(WaitForLeadboard());
#endif
        }

        private void ShowIcons(bool show)
        {
            foreach (var icon in playerIconsList)
            {
                icon.gameObject.SetActive(show);
            }
        }

        void OnDisable()
        {
#if PLAYFAB || GAMESPARKS
            //PlayFabManager.OnLevelLeadboardLoaded -= ShowLeadboard;
#endif
            ResetLeadboard();
        }

        void ResetLeadboard()
        {
            // transform.localPosition = new Vector3(0, -40f, 0);
        }

#if PLAYFAB || GAMESPARKS
        IEnumerator WaitForLeadboard()
        {
            yield return new WaitForSeconds(0.5f);
            yield return new WaitUntil(() => NetworkManager.leadboardList.Count > 0);
            //		print (NetworkManager.leadboardList.Count);
            if (FB.IsLoggedIn)
            {
                GetComponent<Image>().enabled = true;
                ShowLeadboard();
            }
        }

        void ShowLeadboard()
        {
            GetComponent<Animation>().Play();
            float width = 158;
            NetworkManager.leadboardList.Sort(CompareByScore);
            Debug.Log("leadboard players count - " + NetworkManager.leadboardList.Count);
            for (int j = 0; j < NetworkManager.leadboardList.Count; j++)
            {
                var item = NetworkManager.leadboardList[j];
                if (item.score <= 0)
                    continue;
                //			GameObject gm = Instantiate (playerIconPrefab) as GameObject;
                LeadboardObject lo = playerIconsList[j];
                lo.gameObject.SetActive(true);
                item.position = j + 1;
                lo.PlayerData = item;
                Debug.Log("leadboard player data " + item);
                //			playerIconsList.Add (lo);
                //			gm.transform.SetParent (transform);
                //			gm.transform.localScale = Vector3.one;
                //			gm.GetComponent<RectTransform> ().anchoredPosition = leftPosition + Vector2.right * (width * i);
            }
        }


        private int CompareByScore(LeadboardPlayerData x, LeadboardPlayerData y)
        {
            int retval = y.score.CompareTo(x.score);

            if (retval != 0)
            {
                return retval;
            }

            return y.score.CompareTo(x.score);
        }
#endif
    }
}
