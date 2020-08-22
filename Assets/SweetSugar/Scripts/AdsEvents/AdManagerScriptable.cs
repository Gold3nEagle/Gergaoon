using System.Collections.Generic;
using UnityEngine;

namespace SweetSugar.Scripts.AdsEvents
{
/// <summary>
/// Ads manager settings
/// </summary>
    public class AdManagerScriptable : ScriptableObject
    {
        public List<AdEvents> adsEvents = new List<AdEvents>();
        public string admobUIDAndroid;
        public string admobUIDIOS;
        public string admobRewardedUIDAndroid;
        public string admobRewardedUIDIOS;
    }
}