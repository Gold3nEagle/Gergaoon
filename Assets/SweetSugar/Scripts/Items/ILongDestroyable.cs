using System;

namespace SweetSugar.Scripts.Items
{
	/// <summary>
	/// Delayed destroying methods. Using in items with difficult long animations
	/// </summary>
	public interface ILongDestroyable
	{
		void SecondPartDestroyAnimation(Action callback);
		bool CanBeStarted();
		bool IsAnimationFinished();
		int GetPriority();

	}
}