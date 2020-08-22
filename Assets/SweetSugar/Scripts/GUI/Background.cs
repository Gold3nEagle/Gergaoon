using SweetSugar.Scripts.Core;
using UnityEngine;
using UnityEngine.UI;

namespace SweetSugar.Scripts.GUI
{
	/// <summary>
	/// Background selector. Select different level background for every 20 levels
	/// </summary>
	public class Background : MonoBehaviour
	{
		public Sprite[] pictures;

		// Use this for initialization
		void OnEnable ()
		{
			if (LevelManager.THIS != null)
			{
				var backgroundSpriteNum = (int) (PlayerPrefs.GetInt("OpenLevel") / 20f - 0.01f);
				if(pictures.Length > backgroundSpriteNum)
					GetComponent<Image>().sprite = pictures[backgroundSpriteNum];
			}


		}


	}
}
