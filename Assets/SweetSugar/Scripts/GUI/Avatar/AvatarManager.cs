using System.Collections.Generic;
using SweetSugar.Scripts.Integrations;
using SweetSugar.Scripts.Integrations.Network;
using UnityEngine;

namespace SweetSugar.Scripts.GUI.Avatar
{
	/// <summary>
	/// Handles friend avatars on the map
	/// </summary>
	public class AvatarManager : MonoBehaviour {
		public List<GameObject> avatars = new List<GameObject> ();

		void OnEnable () {//1.3.3
#if PLAYFAB || GAMESPARKS
			NetworkManager.OnFriendsOnMapLoaded += CheckFriendsList;

#endif
		}

		void OnDisable () {//1.3.3
#if PLAYFAB || GAMESPARKS
			NetworkManager.OnFriendsOnMapLoaded -= CheckFriendsList;
#endif
		}

		void CheckFriendsList () {
			var Friends = FacebookManager.Friends;

			for (var i = 0; i < Friends.Count; i++) {
				CreateAvatar (Friends [i]);
			}
		}

		/// <summary>
		/// Creates the friend's avatar.
		/// </summary>
		void CreateAvatar (FriendData friendData) {
			var friendAvatar = friendData.avatar;
			if (friendAvatar == null) {
				friendAvatar = Instantiate (Resources.Load ("Prefabs/FriendAvatar")) as GameObject;
				avatars.Add (friendAvatar);
				friendData.avatar = friendAvatar;
				friendAvatar.transform.SetParent (transform);
			}
			friendAvatar.GetComponent<FriendAvatar> ().FriendData = friendData;
		}

	}
}
