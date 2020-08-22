using System;
using SweetSugar.Scripts.Core;

namespace SweetSugar.Scripts.AdsEvents
{
	public enum AdType
	{
		AdmobInterstitial,
		UnityAdsVideo,
		ChartboostInterstitial,
		Appodeal
	}
/// <summary>
/// Ad event
/// </summary>
	[Serializable]
	public class AdEvents
	{
		public GameState gameEvent;
		public AdType adType;
		public int everyLevel;
		//1.6
		public int calls;

	}
}