using System;
using System.Collections.Generic;
using SweetSugar.Scripts.Core;
using UnityEngine;
#if UNITY_PURCHASING && UNITY_INAPPS
using UnityEngine.Purchasing;

namespace SweetSugar.Scripts
{
    public class UnityInAppsIntegration : MonoBehaviour, IStoreListener
    {
        public static UnityInAppsIntegration THIS;
        public static IStoreController m_StoreController;                                                                  // Reference to the Purchasing system.
        private static IExtensionProvider m_StoreExtensionProvider;                                                         // Reference to store-specific Purchasing subsystems.
        private IAppleExtensions m_AppleExtensions;

        // Product identifiers for all products capable of being purchased: "convenience" general identifiers for use with Purchasing, and their store-specific identifier counterparts 
        // for use with and outside of Unity Purchasing. Define store-specific identifiers also on each platform's publisher dashboard (iTunes Connect, Google Play Developer Console, etc.)
        private static string[] kProductIDConsumableArray;                                                       // General handle for the consumable product.

        private static string kProductIDConsumable = "consumable";                                                         // General handle for the consumable product.
        private static string kProductIDNonConsumable = "nonconsumable";                                                  // General handle for the non-consumable product.
        private static string kProductIDSubscription = "subscription";                                                   // General handle for the subscription product.

        private static string kProductNameAppleConsumable = "com.unity3d.test.services.purchasing.consumable";             // Apple App Store identifier for the consumable product.
        private static string kProductNameAppleNonConsumable = "com.unity3d.test.services.purchasing.nonconsumable";      // Apple App Store identifier for the non-consumable product.
        private static string kProductNameAppleSubscription = "com.unity3d.test.services.purchasing.subscription";       // Apple App Store identifier for the subscription product.

        private static string kProductNameGooglePlayConsumable = "com.unity3d.test.services.purchasing.consumable";        // Google Play Store identifier for the consumable product.
        private static string kProductNameGooglePlayNonConsumable = "com.unity3d.test.services.purchasing.nonconsumable";     // Google Play Store identifier for the non-consumable product.
        private static string kProductNameGooglePlaySubscription = "com.unity3d.test.services.purchasing.subscription";  // Google Play Store identifier for the subscription product.

        void Start()
        {
            THIS = this;
            // If we haven't set up the Unity Purchasing reference
            if (m_StoreController == null)
            {
                // Begin to configure our connection to Purchasing
                InitializePurchasing();
            }
        }

        public void InitializePurchasing()
        {
            // If we have already connected to Purchasing ...
            if (IsInitialized())
            {
                // ... we are done here.
                return;
            }
            kProductIDConsumableArray = new string[LevelManager.THIS.InAppIDs.Length];
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            for (int i = 0; i < LevelManager.THIS.InAppIDs.Length; i++)
            {
                kProductIDConsumableArray[i] = LevelManager.THIS.InAppIDs[i];
                builder.AddProduct(kProductIDConsumableArray[i], ProductType.Consumable, new IDs { { kProductIDConsumableArray[i], AppleAppStore.Name }, { kProductIDConsumableArray[i], GooglePlay.Name } });
            }

            // Create a builder, first passing in a suite of Unity provided stores.

            // Add a product to sell / restore by way of its identifier, associating the general identifier with its store-specific identifiers.
            //builder.AddProduct(kProductIDConsumable, ProductType.Consumable, new IDs() { { kProductNameAppleConsumable, AppleAppStore.Name }, { kProductNameGooglePlayConsumable, GooglePlay.Name }, });// Continue adding the non-consumable product.
            //builder.AddProduct(kProductIDNonConsumable, ProductType.NonConsumable, new IDs() { { kProductNameAppleNonConsumable, AppleAppStore.Name }, { kProductNameGooglePlayNonConsumable, GooglePlay.Name }, });// And finish adding the subscription product.
            //builder.AddProduct(kProductIDSubscription, ProductType.Subscription, new IDs() { { kProductNameAppleSubscription, AppleAppStore.Name }, { kProductNameGooglePlaySubscription, GooglePlay.Name }, });// Kick off the remainder of the set-up with an asynchrounous call, passing the configuration and this class' instance. Expect a response either in OnInitialized or OnInitializeFailed.
            UnityPurchasing.Initialize(this, builder);
        }


        private bool IsInitialized()
        {
            // Only say we are initialized if both the Purchasing references are set.
            return m_StoreController != null && m_StoreExtensionProvider != null;
        }


        public void BuyConsumable()
        {
            // Buy the consumable product using its general identifier. Expect a response either through ProcessPurchase or OnPurchaseFailed asynchronously.
            BuyProductID(kProductIDConsumable);
        }


        public void BuyNonConsumable()
        {
            // Buy the non-consumable product using its general identifier. Expect a response either through ProcessPurchase or OnPurchaseFailed asynchronously.
            BuyProductID(kProductIDNonConsumable);
        }


        public void BuySubscription()
        {
            // Buy the subscription product using its the general identifier. Expect a response either through ProcessPurchase or OnPurchaseFailed asynchronously.
            BuyProductID(kProductIDSubscription);
        }


        public void BuyProductID(string productId)
        {
            // If the stores throw an unexpected exception, use try..catch to protect my logic here.
            try
            {
                // If Purchasing has been initialized ...
                if (IsInitialized())
                {
                    // ... look up the Product reference with the general product identifier and the Purchasing system's products collection.
                    Product product = m_StoreController.products.WithID(productId);

                    // If the look up found a product for this device's store and that product is ready to be sold ... 
                    if (product != null && product.availableToPurchase)
                    {
                        Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));// ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed asynchronously.
                        m_StoreController.InitiatePurchase(product);
                    }
                    // Otherwise ...
                    else
                    {
                        // ... report the product look-up failure situation  
                        Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                    }
                }
                // Otherwise ...
                else
                {
                    // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or retrying initiailization.
                    Debug.Log("BuyProductID FAIL. Not initialized.");
                }
            }
            // Complete the unexpected exception handling ...
            catch (Exception e)
            {
                // ... by reporting any unexpected exception for later diagnosis.
                Debug.Log("BuyProductID: FAIL. Exception during purchase. " + e);
            }
        }


        // Restore purchases previously made by this customer. Some platforms automatically restore purchases. Apple currently requires explicit purchase restoration for IAP.
        public void RestorePurchases()
        {
            // If Purchasing has not yet been set up ...
            if (!IsInitialized())
            {
                // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
                Debug.Log("RestorePurchases FAIL. Not initialized.");
                return;
            }

            // If we are running on an Apple device ... 
            if (Application.platform == RuntimePlatform.IPhonePlayer ||
                Application.platform == RuntimePlatform.OSXPlayer)
            {
                // ... begin restoring purchases
                Debug.Log("RestorePurchases started ...");

                // Fetch the Apple store-specific subsystem.
                var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
                // Begin the asynchronous process of restoring purchases. Expect a confirmation response in the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
                apple.RestoreTransactions(result =>
                {
                    // The first phase of restoration. If no more responses are received on ProcessPurchase then no purchases are available to be restored.
                    Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
                });
            }
            // Otherwise ...
            else
            {
                // We are not running on an Apple device. No work is necessary to restore purchases.
                Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
            }
        }


        //  
        // --- IStoreListener
        //

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            // Purchasing has succeeded initializing. Collect our Purchasing references.
            Debug.Log("OnInitialized: PASS");

            // Overall Purchasing system, configured with products for this application.
            m_StoreController = controller;
            m_AppleExtensions = extensions.GetExtension<IAppleExtensions>();

            // Store specific subsystem, for accessing device-specific store features.
            m_StoreExtensionProvider = extensions;
            m_AppleExtensions.RegisterPurchaseDeferredListener(OnDeferred);

            Dictionary<string, string> introductory_info_dict = m_AppleExtensions.GetIntroductoryPriceDictionary();

            foreach (var item in controller.products.all)
            {
                if (item.availableToPurchase)
                {
                    if (item.receipt != null)
                    {
                        if (item.definition.type == ProductType.Subscription)
                        {
                            if (checkIfProductIsAvailableForSubscriptionManager(item.receipt))
                            {
                                string intro_json = (introductory_info_dict == null || !introductory_info_dict.ContainsKey(item.definition.storeSpecificId))
                                    ? null
                                    : introductory_info_dict[item.definition.storeSpecificId];
                                SubscriptionManager p = new SubscriptionManager(item, intro_json);
                                SubscriptionInfo info = p.getSubscriptionInfo();  
                                Debug.Log("product id is: " + info.getProductId());
                                Debug.Log("purchase date is: " + info.getPurchaseDate());
                                Debug.Log("subscription next billing date is: " + info.getExpireDate());
                                Debug.Log("is subscribed? " + info.isSubscribed().ToString());
                                Debug.Log("is expired? " + info.isExpired().ToString());
                                Debug.Log("is cancelled? " + info.isCancelled());
                                Debug.Log("product is in free trial peroid? " + info.isFreeTrial());
                                Debug.Log("product is auto renewing? " + info.isAutoRenewing());
                                Debug.Log("subscription remaining valid time until next billing date is: " + info.getRemainingTime());
                                Debug.Log("is this product in introductory price period? " + info.isIntroductoryPricePeriod());
                                Debug.Log("the product introductory localized price is: " + info.getIntroductoryPrice());
                                Debug.Log("the product introductory price period is: " + info.getIntroductoryPricePeriod());
                                Debug.Log("the number of product introductory price period cycles is: " + info.getIntroductoryPricePeriodCycles());
                            }
                        }
                    }
                }
            }
        }
    
        private bool checkIfProductIsAvailableForSubscriptionManager(string receipt) {
            var receipt_wrapper = (Dictionary<string, object>)MiniJson.JsonDecode(receipt);
            if (!receipt_wrapper.ContainsKey("Store") || !receipt_wrapper.ContainsKey("Payload")) {
                Debug.Log("The product receipt does not contain enough information");
                return false;
            }
            var store = (string)receipt_wrapper ["Store"];
            var payload = (string)receipt_wrapper ["Payload"];

            if (payload != null ) {
                switch (store) {
                    case GooglePlay.Name:
                    {
                        var payload_wrapper = (Dictionary<string, object>)MiniJson.JsonDecode(payload);
                        if (!payload_wrapper.ContainsKey("json")) {
                            Debug.Log("The product receipt does not contain enough information, the 'json' field is missing");
                            return false;
                        }
                        var original_json_payload_wrapper = (Dictionary<string, object>)MiniJson.JsonDecode((string)payload_wrapper["json"]);
                        if (original_json_payload_wrapper == null || !original_json_payload_wrapper.ContainsKey("developerPayload")) {
                            Debug.Log("The product receipt does not contain enough information, the 'developerPayload' field is missing");
                            return false;
                        }
                        var developerPayloadJSON = (string)original_json_payload_wrapper["developerPayload"];
                        var developerPayload_wrapper = (Dictionary<string, object>)MiniJson.JsonDecode(developerPayloadJSON);
                        if (developerPayload_wrapper == null || !developerPayload_wrapper.ContainsKey("is_free_trial") || !developerPayload_wrapper.ContainsKey("has_introductory_price_trial")) {
                            Debug.Log("The product receipt does not contain enough information, the product is not purchased using 1.19 or later");
                            return false;
                        }
                        return true;
                    }
                    case AppleAppStore.Name:
                    case AmazonApps.Name:
                    case MacAppStore.Name:
                    {
                        return true;
                    }
                    default:
                    {
                        return false;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// iOS Specific.
        /// This is called as part of Apple's 'Ask to buy' functionality,
        /// when a purchase is requested by a minor and referred to a parent
        /// for approval.
        ///
        /// When the purchase is approved or rejected, the normal purchase events
        /// will fire.
        /// </summary>
        /// <param name="item">Item.</param>
        private void OnDeferred(Product item)
        {
            Debug.Log("Purchase deferred: " + item.definition.id);
        }
     
        public void OnInitializeFailed(InitializationFailureReason error)
        {
            // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
            Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
        }


        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            // A consumable product has been purchased by this user.
            if (Array.IndexOf(LevelManager.THIS.InAppIDs, args.purchasedProduct.definition.id) > -1)
            {
                Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));//If the consumable item has been successfully purchased, add 100 coins to the player's in-game score.
                InitScript.Instance.PurchaseSucceded();

            }

            return PurchaseProcessingResult.Complete;
        }


        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing this reason with the user.
            Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
        }
    }
}
#endif

