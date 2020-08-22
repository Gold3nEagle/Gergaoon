using SweetSugar.Scripts.MapScripts.StaticMap.Editor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SweetSugar.Scripts.System
{
	public class EffectScene : MonoBehaviour
	{

		// Use this for initialization
		void Start()
		{
			SceneManager.LoadScene(Resources.Load<MapSwitcher>("Scriptable/MapSwitcher").GetSceneName());
		}

		// Update is called once per frame
		void Update()
		{

		}
	}
}
