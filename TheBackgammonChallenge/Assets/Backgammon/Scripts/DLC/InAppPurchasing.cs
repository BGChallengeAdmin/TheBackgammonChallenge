using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using UnityEngine.Purchasing.Security;

namespace Backgammon
{
    public class InAppPurchasing : IDetailedStoreListener
    {
        IStoreController m_StoreController = null;
        CrossPlatformValidator m_Validator = null;

        private ProductCatalog productCatalog;
        private static ICollection<ProductCatalogItem> validProductCatalogItemCollection;
        private static ICollection<ProductCatalogItem> productCatalogItemCollection;
        private List<string> assetBundles;

        private string requestingProductID;
        private string productIdBase = "com.asermet.thebackgammonchallenge.";
        private ProductType productType = ProductType.NonConsumable;
        private static Queue<string> unlockedContent;
        private bool purchaseResponse;

        private DebugPrefab debug_gameAssetManager;

        // ------------------------------------------------------- INITIALIZE ---------------------------------------------------

        public void SetDebugObject(DebugPrefab debugPrefab)
        {
            debug_gameAssetManager = debugPrefab;
        }

        public void InitializePurchasing(List<string> _assetBundles)
        {
            debug_gameAssetManager.DebugMessage($"UNITY IAP INITIALIZE");

            unlockedContent = new Queue<string>();
            assetBundles = new List<string>(_assetBundles);

            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            foreach (var asset in assetBundles)
            {
                var productId = productIdBase + asset;
                if (Application.platform == RuntimePlatform.Android)
                {
                    builder.AddProduct(productId, productType,
                    new IDs {
                        { productId, GooglePlay.Name }
                    });
                }
                else if (Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    builder.AddProduct(productId, productType,
                    new IDs {
                        { productId, AppleAppStore.Name }
                    });
                }
            }

            UnityPurchasing.Initialize(instance, builder);
            IAPConfigurationHelper.PopulateConfigurationBuilder(ref builder, ProductCatalog.LoadDefaultCatalog());
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            debug_gameAssetManager.DebugMessage("In-App Purchasing successfully initialized");
            m_StoreController = controller;
            InitializeValidator();

            productCatalog = ProductCatalog.LoadDefaultCatalog();
        }

        void InitializeValidator()
        {
            if (IsCurrentStoreSupportedByValidator())
            {
#if UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX
                m_Validator = new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), Application.identifier);
#endif
            }
            else
            {
                debug_gameAssetManager.DebugMessage("CURRENT STORE NOT SUPPORTED: " + StandardPurchasingModule.Instance().appStore);
            }
        }

        // ----------------------------------------------------- VALIDATE STORES -------------------------------------------------

        static bool IsCurrentStoreSupportedByValidator()
        {
            //The CrossPlatform validator only supports the GooglePlayStore and Apple's App Stores.
            return IsGooglePlayStoreSelected() || IsAppleAppStoreSelected();
        }

        static bool IsGooglePlayStoreSelected()
        {
            var currentAppStore = StandardPurchasingModule.Instance().appStore;
            return currentAppStore == AppStore.GooglePlay;
        }

        static bool IsAppleAppStoreSelected()
        {
            var currentAppStore = StandardPurchasingModule.Instance().appStore;
            return currentAppStore == AppStore.AppleAppStore ||
                   currentAppStore == AppStore.MacAppStore;
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            debug_gameAssetManager.DebugMessage($"In-App Purchasing initialize failed: {error}");
        }

        // ------------------------------------------------------ PURCHASING ---------------------------------------------------

        public void PurchaseProduct(string productPurchaseID)
        {
            PurchaseResponse = false;

            requestingProductID = productPurchaseID;
            if (m_StoreController != null)
                m_StoreController.InitiatePurchase(productPurchaseID);
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            //Retrieve the purchased product
            var product = args.purchasedProduct;

            var isPurchaseValid = IsPurchaseValid(product);

            if (isPurchaseValid)
            {
                //Add the purchased product to the players inventory
                UnlockContent(product);
                debug_gameAssetManager.DebugMessage("Valid receipt, unlocking content.");
            }
            else
            {
                debug_gameAssetManager.DebugMessage("Invalid receipt, not unlocking content.");
                PurchaseResponse = true;
            }

            //We return Complete, informing Unity IAP that the processing on our side is done and the transaction can be closed.
            return PurchaseProcessingResult.Complete;
        }

        bool IsPurchaseValid(Product product)
        {
            //If we the validator doesn't support the current store, we assume the purchase is valid
            if (IsCurrentStoreSupportedByValidator())
            {
                try
                {
                    var result = m_Validator.Validate(product.receipt);
                    //The validator returns parsed receipts.
                    LogReceipts(result);
                }
                //If the purchase is deemed invalid, the validator throws an IAPSecurityException.
                catch (IAPSecurityException reason)
                {
                    debug_gameAssetManager.DebugMessage($"Invalid receipt: {reason}");
                    return false;
                }
            }

            return true;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            debug_gameAssetManager.DebugMessage($"Purchase failed - Product: '{product.definition.id}', PurchaseFailureReason: {failureReason}");
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
        {
            debug_gameAssetManager.DebugMessage($"Purchase failed - Product: '{product.definition.id}', PurchaseFailureReason: {failureDescription}");
        }

        // ---------------------------------------------------- DELIVER CONTENT -------------------------------------------------

        void UnlockContent(Product product)
        {
            unlockedContent.Enqueue(product.definition.id);

            debug_gameAssetManager.DebugMessage("UNLOCKED CONTENT COUNT " + unlockedContent.Count);

            PurchaseResponse = true;

            //var assetPackList = GameObject.FindObjectsOfType<AssetPackInfoController>();

            //foreach (var assetPack in assetPackList)
            //{
            //    if (assetPack.InAppPurchaseID == product.definition.id)
            //    {


            //        //assetPack.StartDownload();
            //        break;
            //    }
            //}
        }

        static void LogReceipts(IEnumerable<IPurchaseReceipt> receipts)
        {
            Debug.Log("Receipt is valid. Contents:");
            foreach (var receipt in receipts)
            {
                LogReceipt(receipt);
            }
        }

        static void LogReceipt(IPurchaseReceipt receipt)
        {
            Debug.Log($"Product ID: {receipt.productID}\n" +
                      $"Purchase Date: {receipt.purchaseDate}\n" +
                      $"Transaction ID: {receipt.transactionID}");

            if (receipt is GooglePlayReceipt googleReceipt)
            {
                Debug.Log($"Purchase State: {googleReceipt.purchaseState}\n" +
                          $"Purchase Token: {googleReceipt.purchaseToken}");
            }

            if (receipt is AppleInAppPurchaseReceipt appleReceipt)
            {
                Debug.Log($"Original Transaction ID: {appleReceipt.originalTransactionIdentifier}\n" +
                          $"Subscription Expiration Date: {appleReceipt.subscriptionExpirationDate}\n" +
                          $"Cancellation Date: {appleReceipt.cancellationDate}\n" +
                          $"Quantity: {appleReceipt.quantity}");
            }
        }

        // --------------------------------------------------- GETTERS ---------------------------------------------------

        public ICollection<ProductCatalogItem> ValidProductCatalogItemCollection
        {
            get
            {
                if (productCatalog != null)
                {
                    validProductCatalogItemCollection = productCatalog.allValidProducts;
                    return validProductCatalogItemCollection;
                }
                else return null;
            }
        }

        public ICollection<ProductCatalogItem> ProductCatalogItemCollection
        {
            get
            {

                if (productCatalog != null)
                {
                    productCatalogItemCollection = productCatalog.allProducts;
                    debug_gameAssetManager.DebugMessage("COLLECTION COUNT " + productCatalogItemCollection.Count);
                    return productCatalogItemCollection;
                }
                else return null;
            }
        }

        public ProductCollection ProductCollection
        {
            get
            {
                if (m_StoreController != null)
                    return m_StoreController.products;
                else return null;
            }
        }

        public bool IsProductAvailableForPurchase(string inAppPurchaseID)
        {
            var available = false;

            if (m_StoreController != null)
            {
                foreach (var product in m_StoreController.products.all)
                {
                    if (product.definition.id == inAppPurchaseID && product.availableToPurchase)
                    {
                        available = true;
                        break;
                    }
                }
            }

            return available;
        }

        public bool HasProductBeenPurchased(string inAppPurchaseID)
        {
            var purchased = false;

            if (m_StoreController != null)
            {
                foreach (var product in m_StoreController.products.all)
                {
                    if (product.definition.id == inAppPurchaseID)
                    {
                        if (product.hasReceipt) purchased = true;
                        break;
                    }
                }
            }

            return purchased;
        }

        public string GetProductLocalPrice(string inAppPurchaseID)
        {
            string price = string.Empty;

            if (m_StoreController != null)
            {
                foreach (var product in m_StoreController.products.all)
                {
                    if (product.definition.id == inAppPurchaseID)
                    {
                        price = product.metadata.localizedPriceString;
                        break;
                    }
                }
            }

            return price;
        }

        public decimal GetProductPrice(string inAppPurchaseID)
        {
            decimal price = 0;

            if (m_StoreController != null)
            {
                foreach (var product in m_StoreController.products.all)
                {
                    if (product.definition.id == inAppPurchaseID)
                    {
                        price = product.metadata.localizedPrice;
                        break;
                    }
                }
            }

            return price;
        }

        public string GetProductDefinition(string inAppPurchaseID)
        {
            var definition = string.Empty;

            if (m_StoreController != null)
            {
                foreach (var product in m_StoreController.products.all)
                {
                    if (product.definition.id == inAppPurchaseID)
                    {
                        definition = product.metadata.localizedDescription;
                        break;
                    }
                }
            }

            return definition;
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            debug_gameAssetManager.DebugMessage($"IAP INITIALIZATION FAILED");
        }

        public string ProductBaseID { get => productIdBase; }

        public ref Queue<string> UnlockedContent { get => ref unlockedContent; }

        public bool PurchaseResponse
        {
            get => purchaseResponse;
            private set => purchaseResponse = value;
        }

        // --------------------------------------------------- SINGLETON -------------------------------------------------

        private static InAppPurchasing instance = null;

        public static InAppPurchasing Instance
        {
            get { return instance ?? (instance = new InAppPurchasing()); }
        }

        private InAppPurchasing() { }
    }
}