using System.Collections;
using SweetSugar.Scripts.Integrations;
using SweetSugar.Scripts.MapScripts;
using UnityEngine;

namespace SweetSugar.Scripts.GUI.Avatar
{
	/// <summary>
	/// Friend avatar. 
	/// </summary>
	public class FriendAvatar : MonoBehaviour, IAvatarLoader {
		public SpriteRenderer image;
		private FriendData friendData;

		public FriendData FriendData {
			get {
				return friendData;
			}

			set {
				friendData = value;
				if (friendData != null)
					ShowPicture ();
			}
		}

		void OnEnable () {
			Hide ();
		}

		public void ShowPicture () {
			StartCoroutine (WaitForPicture ());
		}

		IEnumerator WaitForPicture () {
			yield return new WaitUntil (() => FriendData.picture != null);
//		GetComponent<SpriteRenderer> ().enabled = true;
			image.sprite = FriendData.picture;
			image.enabled = true;
			SetPosition (FriendData.level);
		}

		void SetPosition(int lvl) {
			var level = LevelsMap._instance.GetLevel (lvl);
			if (level != null)
			{
				transform.position = level.transform.position - GetFreePositionForFriend(level.transform.position);
			}
		}

		Vector3 GetFreePositionForFriend(Vector2 pos)
		{
			Vector2 newPos = Vector2.right;
			return newPos*1.2f;
		}

		void Hide () {
			GetComponent<SpriteRenderer> ().enabled = false;
			image.enabled = false;
		}

		void OnDisable () {
			Hide ();
		}

	}
}
