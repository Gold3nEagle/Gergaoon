using System.Collections.Generic;
using UnityEngine;

namespace SweetSugar.Scripts.System
{
	public static class TransformExtensions
	{
		public static void GetComponentAtPath<T>(
			this Transform transform, 
			string path, 
			out T foundComponent) where T : Component
		{
			Transform t = null;
			if (path == null) 
			{
				// Return the component of the first child that have that type of component
				foreach (Transform child in transform)
				{
					var comp = child.GetComponent<T>();
					if (comp != null)
					{
						foundComponent = comp;
						return;
					}
				}	
			}
			else
				t = transform.Find(path);
		
			if (t == null)
				foundComponent = default(T);
			else
				foundComponent = t.GetComponent<T>();
		}

		public static T GetComponentAtPath<T>(
			this Transform transform,
			string path) where T : Component
		{
			T foundComponent;
			transform.GetComponentAtPath(path, out foundComponent);

			return foundComponent;
		}

		public static Transform[] GetChildren(this Transform tr)
		{
			var childCount = tr.childCount;
			var result = new Transform[childCount];
			for (var i = 0; i < childCount; ++i)
				result[i] = tr.GetChild(i);

			return result;
		}

		/// <summary>
		/// Returns <array.Length> children
		/// </summary>
		/// <param name="tr"></param>
		/// <param name="array"></param>
		public static void GetEnoughChildrenToFitInArray(this Transform tr, Transform[] array)
		{
			var numToReturn = array.Length;
			for (var i = 0; i < numToReturn; ++i)
				array[i] = tr.GetChild(i);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="tr"> the root to use; it'll be excluded from results</param>
		/// <returns>the entire hierarchy</returns>
		public static List<Transform> GetDescendants(this Transform tr)
		{
			Transform[] children = tr.GetChildren();

			var hierarchy = new List<Transform>();
			hierarchy.AddRange(children);

			var childCount = children.Length;
			for (var i = 0; i < childCount; ++i)
				hierarchy.AddRange(children[i].GetDescendants());

			return hierarchy;
		}

		public static void GetDescendantsAndRelativePaths(this Transform tr, ref Dictionary<Transform, string> mapDescendantToPath)
		{
			tr.GetDescendantsAndRelativePaths("", ref mapDescendantToPath);
		}

		static void GetDescendantsAndRelativePaths(this Transform tr, string currentPath, ref Dictionary<Transform, string> mapDescendantToPath)
		{
			Transform[] children = tr.GetChildren();
        

			var childCount = children.Length;
			string path;
			for (var i = 0; i < childCount; ++i)
			{
				var ch = children[i];
				path = currentPath + "/" + ch.name;
				mapDescendantToPath[ch] = path;
				ch.GetDescendantsAndRelativePaths(path, ref mapDescendantToPath);
			}
		}


		public static int GetNumberOfAncestors(this Transform tr)
		{
			var num = 0;
			while (tr = tr.parent)
				++num;

			return num;
		}
	}
}

