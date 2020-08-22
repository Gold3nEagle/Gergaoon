
using SweetSugar.Scripts.Core;
using UnityEngine;
#if PLAYFAB || GAMESPARKS
#if GAMESPARKS
using SweetSugar.Scripts.Integrations.Network.Gamesparks;
#endif
#if PLAYFAB
using PlayFab.ClientModels;
using PlayFab;
#endif

namespace SweetSugar.Scripts.Integrations.Network
{
	/// <summary>
	/// Currency manager
	/// </summary>
	public class NetworkCurrencyManager {
		public static int currentBalance;
		ICurrencyManager currencyMananager;

		public NetworkCurrencyManager () {
			NetworkManager.OnLoginEvent += GetBalance;
			NetworkManager.OnLogoutEvent += Logout;
#if PLAYFAB
		currencyMananager = new PlayFabCurrencyManager ();
#elif GAMESPARKS 
			currencyMananager = new GamesparksCurrencyManager ();
#endif
		}

		void Logout () {
			NetworkManager.OnLoginEvent -= GetBalance;
			NetworkManager.OnLogoutEvent -= Logout;
		}

		/// <summary>
		/// Increment the balance
		/// </summary>
		/// <param name="amount"></param>
		public  void IncBalance (int amount) {
			if (!NetworkManager.THIS.IsLoggedIn)
				return;


			if (currencyMananager != null)
				currencyMananager.IncBalance (amount);
		}

		/// <summary>
		/// Decrement the balance
		/// </summary>
		/// <param name="amount"></param>
		public  void DecBalance (int amount) {
			if (!NetworkManager.THIS.IsLoggedIn)
				return;


			if (currencyMananager != null)
				currencyMananager.DecBalance (amount);
		}

		/// <summary>
		/// define the balance
		/// </summary>
		/// <param name="newbalance"></param>
		public  void SetBalance (int newbalance) {
			if (!NetworkManager.THIS.IsLoggedIn)
				return;

			//		GetBalance ();

			if (currencyMananager != null)
				currencyMananager.SetBalance (newbalance);
		}

		/// <summary>
		/// Get the balance
		/// </summary>
		public  void GetBalance () {
			if (!NetworkManager.THIS.IsLoggedIn)
				return;

			if (currencyMananager != null) {
				currencyMananager.GetBalance (balance => {
					Debug.Log (balance);
					currentBalance = balance;
					if (currentBalance >= InitScript.Gems)
						InitScript.Instance.SetGems (balance);
					else
						SetBalance (InitScript.Gems);
				
				});

			}
		}

	}
}

#endif