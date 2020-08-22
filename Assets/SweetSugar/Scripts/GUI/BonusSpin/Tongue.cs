using UnityEngine;

namespace SweetSugar.Scripts.GUI.BonusSpin
{
	/// <summary>
	/// Wheel tongue collision sound effect
	/// </summary>
	public class Tongue : MonoBehaviour
	{
		public AudioClip clip;

		private void OnCollisionEnter2D(Collision2D other)
		{
			SoundBase.Instance.PlayOneShot( clip );

		}
	}
}
