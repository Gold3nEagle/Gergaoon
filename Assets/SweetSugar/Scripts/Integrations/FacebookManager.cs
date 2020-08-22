using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SweetSugar.Scripts.Core;
using SweetSugar.Scripts.Integrations.Network;
using SweetSugar.Scripts.MapScripts.StaticMap.Editor;
using UnityEngine;
using UnityEngine.SceneManagement;
#if FACEBOOK
using Facebook.Unity;
#endif
namespace SweetSugar.Scripts.Integrations
{

/// <summary>
/// Facebook manager
/// </summary>
	public class FacebookManager : MonoBehaviour
	{
		private bool LoginEnable;

		public static string userID;
		public static List<FriendData> Friends = new List<FriendData>();

		public string LastResponse { get; set; } = string.Empty;

		public string Status { get; set; } = "Ready";

		bool loginForSharing;
		public static FacebookManager THIS;
		bool loginOnce;
		//2.1.3

		void Awake()
		{
			if (THIS == null) THIS = this;
			else if (THIS != this) Destroy(gameObject);
			DontDestroyOnLoad(this);
		}

		void OnEnable()
		{
#if PLAYFAB
		NetworkManager.OnLoginEvent += GetUserName;

#endif
		}


		void OnDisable()
		{
#if PLAYFAB
		NetworkManager.OnLoginEvent -= GetUserName;

#endif
		}

		public void AddFriend(FriendData friend)
		{ //2.1.2
			FriendData friendIndex = Friends.Find(delegate (FriendData bk)
			{
				return bk.userID == friend.userID;
			});
			if (friendIndex == null)
				Friends.Add(friend);
		}

		public void SetPicture(string userID, Sprite sprite)
		{//2.1.2
			FriendData friendIndex = Friends.Find(delegate (FriendData bk)
			{
				return bk.userID == userID;
			});
			if (friendIndex != null)
				friendIndex.picture = sprite;
		}

#if PLAYFAB || GAMESPARKS
		public FriendData GetCurrentUserAsFriend()
		{
			FriendData friend = new FriendData
			{
				FacebookID = NetworkManager.facebookUserID,
				userID = NetworkManager.UserID,
				picture = NetworkManager.profilePic
			};
			//		print ("playefab id: " + friend.PlayFabID);
			return friend;
		}
#endif

		#region FaceBook_stuff

#if FACEBOOK
		public void CallFBInit()
		{
			Debug.Log("init facebook");
			if (!FB.IsInitialized)
			{
				FB.Init(OnInitComplete, OnHideUnity);
			}
			else
			{
				FB.ActivateApp();
			}
		}

		private void OnInitComplete()
		{
			Debug.Log("FB.Init completed: Is user logged in? " + FB.IsLoggedIn);
			if (FB.IsLoggedIn)
			{//1.3
				LoggedSuccefull();//2.1.3
			}

		}

		private void OnHideUnity(bool isGameShown)
		{
			Debug.Log("Is game showing? " + isGameShown);
		}

		void OnGUI()
		{
			if (LoginEnable)
			{
				CallFBLogin();
				LoginEnable = false;
			}
		}

		public void CallFBLogin()
		{
			if (!loginOnce)
			{//2.1.3
				loginOnce = true;
				Debug.Log("login");
				FB.LogInWithReadPermissions(new List<string> { "public_profile", "email", "user_friends" }, HandleResult);
			}
		}

		public void CallFBLogout()
		{
			FB.LogOut();

#if PLAYFAB || GAMESPARKS
			NetworkManager.profilePic = null;
			NetworkManager.THIS.IsLoggedIn = false;
#endif
			SceneManager.LoadScene(Resources.Load<MapSwitcher>("Scriptable/MapSwitcher").GetSceneName());
		}

		public void Share()
		{
			if (!FB.IsLoggedIn)
			{
				loginForSharing = true;
				LoginEnable = true;
				Debug.Log("not logged, logging");
			}
			else
			{
				FB.FeedShare(
					link: new Uri("https://apps.facebook.com/" + FB.AppId + "/?challenge_brag=" + (FB.IsLoggedIn ? AccessToken.CurrentAccessToken.UserId : "guest")),
					linkName: "Sweet Sugar",
					linkCaption: "I've got " + LevelManager.Score + " scores! Try to beat me!"
				);
			}
		}

		protected void HandleResult(IResult result)
		{
			loginOnce = false;//2.1.3
			if (result == null)
			{
				LastResponse = "Null Response\n";
				Debug.Log(LastResponse);
				return;
			}

			// Some platforms return the empty string instead of null.
			if (!string.IsNullOrEmpty(result.Error))
			{
				Status = "Error - Check log for details";
				LastResponse = "Error Response:\n" + result.Error;
				Debug.Log(result.Error);
			}
			else if (result.Cancelled)
			{
				Status = "Cancelled - Check log for details";
				LastResponse = "Cancelled Response:\n" + result.RawResult;
				Debug.Log(result.RawResult);
			}
			else if (!string.IsNullOrEmpty(result.RawResult))
			{
				Status = "Success - Check log for details";
				LastResponse = "Success Response:\n" + result.RawResult;
				LoggedSuccefull();//1.3
			}
			else
			{
				LastResponse = "Empty Response\n";
				Debug.Log(LastResponse);
			}
		}

		public void LoggedSuccefull()
		{
#if !PLAYFAB && !GAMESPARKS
		NetworkManager.THIS.IsLoggedIn = true;
#endif
			PlayerPrefs.SetInt("Facebook_Logged", 1);
			PlayerPrefs.Save();


			//Debug.Log(result.RawResult);
			userID = AccessToken.CurrentAccessToken.UserId;
			GetPicture(AccessToken.CurrentAccessToken.UserId);

#if PLAYFAB || GAMESPARKS
			NetworkManager.facebookUserID = AccessToken.CurrentAccessToken.UserId;
			NetworkManager.THIS.LoginWithFB(AccessToken.CurrentAccessToken.TokenString);
#endif
		}

		void GetUserName()
		{
			FB.API("/me?fields=first_name", HttpMethod.GET, GettingNameCallback);
		}

		private void GettingNameCallback(IGraphResult result)
		{
			if (string.IsNullOrEmpty(result.Error))
			{
				IDictionary dict = result.ResultDictionary as IDictionary;
				string fbname = dict["first_name"].ToString();

#if PLAYFAB || GAMESPARKS
				NetworkManager.THIS.UpdateName(fbname);
#endif

			}

		}

		IEnumerator loadPicture(string url)//2.1.4
		{
			WWW www = new WWW(url);
			yield return www;

			var texture = www.texture;

			var sprite = Sprite.Create(texture, new Rect(0, 0, 128, 128), new Vector2(0, 0), 1f);
			NetworkManager.profilePic = sprite;

#if PLAYFAB || GAMESPARKS
			SetPicture(NetworkManager.UserID, NetworkManager.profilePic);
			NetworkManager.PlayerPictureLoaded();

#endif
		}


		void GetPicture(string id)
		{
			FB.API("/" + id + "/picture?g&width=128&height=128&redirect=false", HttpMethod.GET, ProfilePhotoCallback);//2.1.4
		}

		private void ProfilePhotoCallback(IGraphResult result)
		{
			if (string.IsNullOrEmpty(result.Error))//2.1.4
			{
				var dic = result.ResultDictionary["data"] as Dictionary<string, object>;
				string url = dic.Where(i => i.Key == "url").First().Value as string;
				StartCoroutine(loadPicture(url));
			}
		}

		public void GetFriendsPicture()
		{
			FB.API("me/friends?fields=picture.width(128).height(128)", HttpMethod.GET, RequestFriendsCallback); //2.1.6
		}

		private void RequestFriendsCallback(IGraphResult result)
		{
			if (!string.IsNullOrEmpty(result.RawResult))
			{
				var resultDictionary = result.ResultDictionary;
				if (resultDictionary.ContainsKey("data"))
				{
					var dataArray = (List<object>)resultDictionary["data"];//2.1.4
					var dic = dataArray.Select(x => x as Dictionary<string, object>).ToArray();

					foreach (var item in dic)
					{
						string id = (string)item["id"];
						var url = item.Where(x => x.Key == "picture").SelectMany(x => x.Value as Dictionary<string, object>).Where(x => x.Key == "data").SelectMany(x => x.Value as Dictionary<string, object>).Where(i => i.Key == "url").First().Value;
						FriendData friend = Friends.Where(x => x.FacebookID == id).FirstOrDefault();
						if (friend != null)
							GetPictureByURL("" + url, friend);
					}
				}

				if (!string.IsNullOrEmpty(result.Error))
				{
					Debug.Log(result.Error);

				}
			}
		}

		public void GetPictureByURL(string url, FriendData friend)
		{
			StartCoroutine(GetPictureCor(url, friend));
		}

		IEnumerator GetPictureCor(string url, FriendData friend)
		{
			WWW www = new WWW(url);
			yield return www;
			var sprite = Sprite.Create(www.texture, new Rect(0, 0, 128, 128), new Vector2(0, 0), 1f);
			friend.picture = sprite;
			//		print ("get picture for " + url);
		}

		public void APICallBack(IGraphResult result)
		{
			Debug.Log(result);
		}

#endif
		#endregion

	}

	public class FriendData
	{
		public string userID;
		public string FacebookID;
		public Sprite picture;
		public int level;
		public GameObject avatar;
	}
}