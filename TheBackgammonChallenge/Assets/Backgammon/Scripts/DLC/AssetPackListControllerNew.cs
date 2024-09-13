using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

namespace Backgammon
{
    public class AssetPackListControllerNew : MonoBehaviour
    {
        [SerializeField] private GameObject appleLoadingPopup;

        public GameObject assetPackInfoPrefab;
        public GameObject assetPackListContent;

        private GameAssetManager gameAssetManager;
        private ProductCollection productCollection;
        private ICollection<ProductCatalogItem> productCatalogItemCollection;
        static ProductCatalogItem catalogItem;
        private List<AssetPackInfoControllerNew> assetPackList;
        public DownloadMatchInfoPopup assetBundleInfoPopup;
        public Text assetBundleInfoPopupText;

        private string inAppPurchaseBaseID;

        private void Awake()
        {
            gameAssetManager = GameObject.Find("GameAssetManager").GetComponent<GameAssetManager>();
            if (gameAssetManager == null)
            {
                Debug.LogError("Failed to find GameAssetManager");
            }

            inAppPurchaseBaseID = gameAssetManager.AssetBundleBaseID;
        }

        private void Start()
        {
            var assetPackNames = gameAssetManager.GetAssetPackNameList();
            productCollection = gameAssetManager.ProductCollection;
            productCatalogItemCollection = gameAssetManager.ProductCatalogItemCollection;
            assetPackList = new List<AssetPackInfoControllerNew>();

            Debug.Log($"POPULATE ASSETS PACK LIST");

            foreach (string assetPackName in assetPackNames)
            {
                if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
                    AddAssetPack(assetPackName);
            }

            // SET CONTENT LIST TO TOP
            assetPackListContent.gameObject.GetComponentInParent<ScrollRect>().verticalNormalizedPosition = 1f;
        }

        private void OnEnable()
        {
            StartCoroutine(UnlockAssetPacks(gameAssetManager.UnlockedContent));

            UpdateListContent();
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        private void AddAssetPack(string assetPackName)
        {
            var productInAppPurchaseID = inAppPurchaseBaseID + assetPackName;

            // ONLY ADD PRODUCTS TO THE LIST IF THEY ARE AVAILABLE FOR PURCHASE

            if (gameAssetManager.IsProductAvailableForPurchase(productInAppPurchaseID))
            {
                GameObject assetPackInfo = Instantiate(assetPackInfoPrefab) as GameObject;
                AssetPackInfoControllerNew infoController = assetPackInfo.GetComponent<AssetPackInfoControllerNew>();
                string localisedPricingString = gameAssetManager.GetLocalisedPricingString(productInAppPurchaseID);
                bool isAssetPurchased = gameAssetManager.HasProductBeenPurchased(productInAppPurchaseID);

                infoController.SetAssetPack(assetPackName, localisedPricingString, isAssetPurchased);
                infoController.transform.SetParent(assetPackListContent.transform, false);
                infoController.transform.localScale = Vector3.one;

                if (isAssetPurchased) gameAssetManager.UnlockAssetContent(assetPackName);

            infoController.SetInAppPurchaseID(productInAppPurchaseID);


            if (productCollection != null)
                {
                    foreach (var product in productCollection.all)
                    {
                        if (product.definition.id == productInAppPurchaseID)
                        {
                            // VALIDATE THAT THE BUNDLE IS A PRODUCT WHICH HAS BEEN FOUND IN THE BUNDLE MANAGER
                            Debug.Log("VALID PRODUCT ID " + productInAppPurchaseID);
                            infoController.SetInAppPurchaseID(productInAppPurchaseID);

                            break;
                        }
                    }
                }

                assetPackList.Add(infoController);
            }

            Debug.Log("ADDING ASSET PACK: " + assetPackName);
        }

        private void UpdateListContent(bool showMinimised = false)
        {
            //foreach (var asset in assetPackList)
            //{
            //    if (asset.IsDownloaded())
            //        asset.SetAsPostDownload(showMinimised);
            //}

            // RESIZE LIST CONTAINER FOR DOWNLOADED ITEMS
            //assetPackListContent.
        }

        public void OnDontShowPurchasedItemsToggleChanged(bool toggleOn)
        {
            UpdateListContent(toggleOn);
        }

        IEnumerator UnlockAssetPacks(Queue<string> unlockedAssetBundleQueue)
        {
            while (unlockedAssetBundleQueue.Count > 0)
            {
                yield return new WaitForSeconds(0);

                var unlockedAsset = unlockedAssetBundleQueue.Dequeue();

                Debug.Log("UNLOCKING ASSET " + unlockedAsset);

                foreach (var assetPack in assetPackList)
                {
                    if (assetPack.InAppPurchaseID == unlockedAsset)
                    {
                        Debug.Log(unlockedAsset + " WAS UNLOCKED");

                        assetPack.SetAssetPackDownloaded();
                        gameAssetManager.UnlockAssetContent(assetPack.AssetPackName);
                        break;
                    }
                }
            }

            StartCoroutine(ListenForInAppPurchaseResponse(() => gameAssetManager.InAppPurchaseResponse));
        }

        IEnumerator ListenForInAppPurchaseResponse(System.Func<bool> inAppPurchaseResponse)
        {
            yield return new WaitUntil(inAppPurchaseResponse);

            gameAssetManager.InAppPurchaseResponse = false;

            StartCoroutine(UnlockAssetPacks(gameAssetManager.UnlockedContent));
        }

        // -------------------------------------------------- BUNDLE INFO POPUP ----------------------------------------------

        public void OnClickDisplayAssetPackInfo(string inAppPurchaseID)
        {
            assetBundleInfoPopup.gameObject.SetActive(true);

            //var description = gameAssetManager.GetProductDefinitionFromPurchasing(inAppPurchaseID);

            var description = gameAssetManager.GetProductDefinitionFromFile(inAppPurchaseID);

            //assetBundleInfoPopupText.text = description == string.Empty ? "\n\n This Bundle is not available for download at the moment" : ("\n" + description);

            assetBundleInfoPopup.SetMatchInfo(0, description.match1, description.player11, description.player12, description.games1);
            assetBundleInfoPopup.SetMatchInfo(1, description.match2, description.player21, description.player22, description.games2);
            assetBundleInfoPopup.SetMatchInfo(2, description.match3, description.player31, description.player32, description.games3);
            assetBundleInfoPopup.SetMatchInfo(3, description.match4, description.player41, description.player42, description.games4);
        }

        public void OnClickInfoPopupBackButton()
        {
            assetBundleInfoPopup.gameObject.SetActive(false);
        }

        // -------------------------------------------------- APPLE CONNECT POPUP ----------------------------------------------

        public void StartAppleConnectPopup()
        {
            StartCoroutine(ApplePopupCoroutine());
        }

        private IEnumerator ApplePopupCoroutine()
        {
            var text = appleLoadingPopup.transform.Find("textBlock").GetComponentInChildren<Text>();
            text.text = "Connecting to App Store";
            appleLoadingPopup.gameObject.SetActive(true);

            yield return new WaitForSeconds(1.0f);
            text.text += ".";

            yield return new WaitForSeconds(1.0f);
            text.text += ".";

            yield return new WaitForSeconds(1.0f);
            text.text += ".";

            yield return new WaitForSeconds(1.0f);
            appleLoadingPopup.gameObject.SetActive(false);
        }
    }
}