﻿using System;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class IAPManager : MonoBehaviour, IStoreListener
{

    public Text removeAdsPrice, candy50KPrice, candy100KPrice, candy5kAdsPrice; 


    public static IAPManager instance;

    private static IStoreController m_StoreController;
    private static IExtensionProvider m_StoreExtensionProvider;

    //Step 1 create your products
    private string removeAds = "remove_ads";
    private string candy50K = "candy_50k";
    private string candy100K = "candy_100k";
    private string candy5kAds = "candyads_5k";



    //************************** Adjust these methods **************************************
    public void InitializePurchasing()
    {
        if (IsInitialized()) { return; }
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        //Step 2 choose if your product is a consumable or non consumable
        builder.AddProduct(removeAds, ProductType.NonConsumable);
        builder.AddProduct(candy50K, ProductType.Consumable);
        builder.AddProduct(candy100K, ProductType.Consumable);
        builder.AddProduct(candy5kAds, ProductType.Consumable);
        UnityPurchasing.Initialize(this, builder);
    }


    private bool IsInitialized()
    {
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }


    //Step 3 Create methods

    public void Candy50Ads()
    {
        BuyProductID(candy5kAds);
    }

    public void RemoveAds()
    { 
        BuyProductID(removeAds);
    }

    public void Candy50()
    {
        BuyProductID(candy50K);
    }

    public void Candy100()
    {
        BuyProductID(candy100K);
    }


    //Step 4 modify purchasing
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        if (String.Equals(args.purchasedProduct.definition.id, removeAds, StringComparison.Ordinal))
        {
            Debug.Log("Remove Ads");
            PlayerPrefs.SetInt("IAPAds", 1);
        }

        else if (String.Equals(args.purchasedProduct.definition.id, candy50K, StringComparison.Ordinal))
        {
            Debug.Log("Add 50K candy");
            int totalCandy = PlayerPrefs.GetInt("totalCandy");
            totalCandy += 5000;
            PlayerPrefs.SetInt("totalCandy", totalCandy);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        else if (String.Equals(args.purchasedProduct.definition.id, candy100K, StringComparison.Ordinal))
        {
            Debug.Log("Add 100K candy");
            int totalCandy = PlayerPrefs.GetInt("totalCandy");
            totalCandy += 10000;
            PlayerPrefs.SetInt("totalCandy", totalCandy);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        else if (String.Equals(args.purchasedProduct.definition.id, candy5kAds, StringComparison.Ordinal))
        {
            Debug.Log("Add 5K candy & Remove Ads");
            int totalCandy = PlayerPrefs.GetInt("totalCandy");
            totalCandy += 5000;
            PlayerPrefs.SetInt("totalCandy", totalCandy);
            PlayerPrefs.SetInt("IAPAds", 1);

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        else
        {
            Debug.Log("Purchase Failed");
        }
        return PurchaseProcessingResult.Complete;
    }
     
    public void RetrievePrices()
    {
        SetTextPrice(removeAdsPrice, removeAds);
        SetTextPrice(candy50KPrice, candy50K);
        SetTextPrice(candy100KPrice, candy100K);
        SetTextPrice(candy5kAdsPrice, candy5kAds);
    }


    void SetTextPrice(Text priceText, string productID)
    {
        if (m_StoreController == null) InitializePurchasing();
        string priceInStore = m_StoreController.products.WithID(productID).metadata.localizedPriceString;

        priceText.text = priceInStore;
    } 


    //**************************** Dont worry about these methods ***********************************
    private void Awake()
    {
        //TestSingleton();
    }

    void Start()
    {
        if (m_StoreController == null) { InitializePurchasing(); }
    }

    private void TestSingleton()
    {
        if (instance != null) { Destroy(gameObject); return; }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void BuyProductID(string productId)
    {
        if (IsInitialized())
        {
            Product product = m_StoreController.products.WithID(productId);
            if (product != null && product.availableToPurchase)
            {
                Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                m_StoreController.InitiatePurchase(product);
            }
            else
            {
                Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
            }
        }
        else
        {
            Debug.Log("BuyProductID FAIL. Not initialized.");
        }
    }

    public void RestorePurchases()
    {
        if (!IsInitialized())
        {
            Debug.Log("RestorePurchases FAIL. Not initialized.");
            return;
        }

        if (Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.OSXPlayer)
        {
            Debug.Log("RestorePurchases started ...");

            var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
            apple.RestoreTransactions((result) => {
                Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
            });
        }
        else
        {
            Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
        }
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log("OnInitialized: PASS");
        m_StoreController = controller;
        m_StoreExtensionProvider = extensions;
    }


    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
    }
}