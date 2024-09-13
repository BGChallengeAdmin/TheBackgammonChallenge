using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Purchasing;

namespace Backgammon
{
    public class GameAssetManager : MonoBehaviour
    {
        private InAppPurchasing inAppPurchasing;

        private ProductCollection productCollection;
        private ProductCatalog productCatalog;
        private ICollection<ProductCatalogItem> validProductCatalogItemCollection;
        private ICollection<ProductCatalogItem> productCatalogItemCollection;

        private string assetBaseIDAsProduct;

        [Header("Asset Packs List")]
        public List<string> installtimeAssetPackNameList;
        public List<string> fastfollowAssetPackNameList;
        public List<string> ondemandAssetPackNameList;

        [Header("Match Bundles List")]
        public List<GameObject> tournamentMatchBundlesList;

        public string discreteAssetPackName;

        private Dictionary<string, AssetBundle> assetBundles;

        private bool inAppPurchaseResponse;

        private void Start()
        {
            DontDestroyOnLoad(this.gameObject);

            assetBundles = new Dictionary<string, AssetBundle>();

            inAppPurchasing = InAppPurchasing.Instance;
            inAppPurchasing.InitializePurchasing(GetAssetPackNameList());

            assetBaseIDAsProduct = inAppPurchasing.ProductBaseID;

            Debug.Log("*********** IAP INITIALIZED");
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void OnRuntimeMethodLoad()
        {
            Debug.Log("ANALYTICS DISABLED");

            UnityEngine.Analytics.Analytics.initializeOnStartup = false;
            UnityEngine.Analytics.Analytics.deviceStatsEnabled = false;
            UnityEngine.Analytics.Analytics.limitUserTracking = true;
            UnityEngine.Analytics.Analytics.enabled = false;
            UnityEngine.Analytics.PerformanceReporting.enabled = false;
        }

        public void AddAssetBundle(string assetBundleName, AssetBundle assetBundle)
        {
            if (!assetBundles.ContainsKey(assetBundleName))
                assetBundles.Add(assetBundleName, assetBundle);
            else
                assetBundles[assetBundleName] = assetBundle;
        }

        public List<string> GetAssetPackNameList()
        {
            var assetPacks = installtimeAssetPackNameList.Concat(
                fastfollowAssetPackNameList).Concat(
                    ondemandAssetPackNameList).ToList();
            return assetPacks;
        }

        public List<string> GetAssetBundleNameList()
        {
            return GetAssetPackNameList();
        }

        public bool IsSingleAssetBundleAssetPack(string assetPackName)
        {
            if (assetPackName.Equals(discreteAssetPackName))
            {
                return false;
            }
            return true;
        }

        public bool LoadAllUnlockedContent()
        {
            Debug.Log("LOAD ALL ASSETS FROM TITLE MENU");

            var contentUnlocked = false;

            foreach (string assetPackName in GetAssetPackNameList())
            {
                if (HasProductBeenPurchased(AssetBundleBaseID + assetPackName))
                {
                    UnlockAssetContent(assetPackName);
                    contentUnlocked = true;
                }
            }

            return contentUnlocked;
        }

        public void UnlockAssetContent(string assetPackName)
        {                
            foreach (var bundle in tournamentMatchBundlesList)
            {
                if (bundle != null && bundle.gameObject.name == assetPackName)
                {
                    Debug.Log($"G_A_M UNLOCK {assetPackName}");
                    bundle.gameObject.SetActive(true);
                    break;
                }
            }
        }

        // ----------------------------------------------- PRODUCT PURCHASING ------------------------------------------

        public void PurchaseProduct(string productPurchaseID)
        {
            inAppPurchasing.PurchaseProduct(productPurchaseID);

            StartCoroutine(LoadUnlockedContent(() => inAppPurchasing.PurchaseResponse));
        }

        IEnumerator LoadUnlockedContent(System.Func<bool> inAppPurchaseResponse)
        {
            yield return new WaitUntil(inAppPurchaseResponse);

            foreach (var content in UnlockedContent)
            {
                Debug.Log($"CONTENT {content}");
            }

            InAppPurchaseResponse = true;
        }

        public bool IsProductValidForPurchase(string productPurchaseID)
        {
            var valid = false;

            validProductCatalogItemCollection = inAppPurchasing.ValidProductCatalogItemCollection;
            var item = validProductCatalogItemCollection.FirstOrDefault<ProductCatalogItem>(item => item.id == productPurchaseID);
            if (productCatalogItemCollection.Contains(item)) valid = true;

            return valid;
        }

        public ICollection<ProductCatalogItem> ProductCatalogItemCollection 
        {
            get
            {
                productCatalogItemCollection = inAppPurchasing.ProductCatalogItemCollection;
                return productCatalogItemCollection;
            }
        }

        public ProductCollection ProductCollection
        {
            get 
            {
                productCollection = inAppPurchasing.ProductCollection;
                return productCollection;
            }
        }

        public bool IsProductAvailableForPurchase(string productPurchaseID)
        {
            return inAppPurchasing.IsProductAvailableForPurchase(productPurchaseID);
        }

        public string GetLocalisedPricingString(string productPurchaseID)
        {
            return inAppPurchasing.GetProductLocalPrice(productPurchaseID);
        }

        public decimal GetProductPrice(string productPurchaseID)
        {
            return inAppPurchasing.GetProductPrice(productPurchaseID);
        }

        public bool HasProductBeenPurchased(string productInAppPurchaseID)
        {
            return inAppPurchasing.HasProductBeenPurchased(productInAppPurchaseID);
        }

        public string GetProductDefinitionFromPurchasing(string productPurchaseID)
        {
            return inAppPurchasing.GetProductDefinition(productPurchaseID);
        }

        public InAppPurchaseDescriptions.PurchaseDescription GetProductDefinitionFromFile(string productPurchaseID)
        {
            return InAppPurchaseDescriptions.PurchaseDescriptionStruct(productPurchaseID);
        }

        public string AssetBundleBaseID { get => assetBaseIDAsProduct; }

        public Dictionary<string, AssetBundle> AssetBundles { get => assetBundles; }

        public ref Queue<string> UnlockedContent { get => ref inAppPurchasing.UnlockedContent; }

        public bool InAppPurchaseResponse
        {
            get => inAppPurchaseResponse;
            set
            {
                inAppPurchaseResponse = value;
                StopAllCoroutines();
            }
        }
    }
}