using System;
using UnityEngine;
using UnityEngine.Playables;

namespace SweetSugar.Scripts.Items
{
	[Serializable]
	public class testasset : PlayableAsset
	{
		// Factory method that generates a playable based on this asset
		public override Playable CreatePlayable(PlayableGraph graph, GameObject go) {
			return Playable.Create(graph);
		}
	}
}
