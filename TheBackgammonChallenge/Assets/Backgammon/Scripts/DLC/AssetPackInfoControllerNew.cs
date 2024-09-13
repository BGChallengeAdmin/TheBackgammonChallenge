using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Backgammon
{
    public class AssetPackInfoControllerNew : MonoBehaviour
    {
        public GameObject assetPackContainer;
        public Text assetPackNameLabel;
        public Text assetPackPriceLabel;
        public GameObject assetPackDownloadButton;
        public Button assetPackInfoButton;

        [SerializeField] private Text _purchaseButtonText;

        private string inAppPurchaseID = string.Empty;
        private string assetPackName = string.Empty;
        private float assetPackInfoContainerDefaultWidth;
        private float assetPackInfoContainerDefaultHeight;

        private enum AssetPackInfoStatus
        {
            PACKSTATUS_PENDING,
            PACKSTATUS_NEEDS_DOWNLOAD,
            PACKSTATUS_NEEDS_PERMISSION,
            PACKSTATUS_DOWNLOADING,
            PACKSTATUS_READY,
            PACKSTATUS_ERROR
        };

        private GameAssetManager gameAssetManager;

        private void Awake()
        {
            gameAssetManager = GameObject.Find("GameAssetManager").GetComponent<GameAssetManager>();
            if (gameAssetManager == null)
            {
                Debug.LogError("Failed to find GameAssetManager");
            }

            assetPackInfoContainerDefaultWidth = assetPackContainer.gameObject.GetComponent<RectTransform>().rect.width;
            assetPackInfoContainerDefaultHeight = assetPackContainer.gameObject.GetComponent<RectTransform>().rect.height;

            // CHANGE COLOUR OF PURCHASE BUTTON

            if(Application.platform == RuntimePlatform.IPhonePlayer)
            {
                var button = assetPackDownloadButton.GetComponentInChildren<Button>();
                var colours = button.colors;
                colours.normalColor = new Color(0.1803922f, 0.4666667f, 0.9686275f, 1f);
                button.colors = colours;
                var text = button.GetComponentInChildren<Text>();
                text.color = Color.white;
            }
        }

        private void OnEnable()
        {
            LanguageScriptableObject languageSO = Main.Instance.WorldRegionObj.LanguageSO;

            if (languageSO != null)
            {
                _purchaseButtonText.text = languageSO.downloadsPurchase;
            }
        }

        // Update is called once per frame
        void Update()
        {
            UpdateUI();
        }

        public void SetAssetPack(string assetPackName, string localPrice, bool purchased)
        {
            AssetPackName = assetPackName;
            assetPackNameLabel.text = assetPackName;
            assetPackPriceLabel.text = localPrice;

            Debug.Log($"{assetPackName} {localPrice}");

            if (purchased) SetAssetPackDownloaded();

            UpdateUI();
        }

        public void SetInAppPurchaseID(string _id)
        {
            InAppPurchaseID = _id;

            Debug.Log($"ASSET PURCHASE ID {_id}");
        }

        public void OnClickInfoPanelButton()
        {
            GetComponentInParent<AssetPackListControllerNew>().OnClickDisplayAssetPackInfo(InAppPurchaseID);
        }

        public void SubmitPurchaseRequest()
        {
            gameAssetManager.PurchaseProduct(inAppPurchaseID);
            if (Application.platform == RuntimePlatform.IPhonePlayer)
                GetComponentInParent<AssetPackListControllerNew>().StartAppleConnectPopup();
        }

        public void SetAssetPackDownloaded()
        {
            assetPackPriceLabel.text = "PURCHASED";
            assetPackDownloadButton.gameObject.SetActive(false);

        }

        void UpdateUI()
        {
            //if (infoStatus == AssetPackInfoStatus.PACKSTATUS_NEEDS_PERMISSION)
            //{
            //    if (waitingOnPermissionResult)
            //    {
            //        if (!gameAssetPack.IsCellularConfirmationActive)
            //        {
            //            waitingOnPermissionResult = false;
            //            PermissionResult(gameAssetPack.DidApproveCellularData);
            //        }
            //    }
            //}
            //else if (gameAssetPack.IsDownloadActive())
            //{
            //    UpdateDownloading();
            //}
            //else if (gameAssetPack.IsDownloaded())
            //{
            //    if (infoStatus != AssetPackInfoStatus.PACKSTATUS_READY)
            //    {
            //        SetReady();
            //    }
            //}
            //else if (infoStatus == AssetPackInfoStatus.PACKSTATUS_PENDING)
            //{
            //    UpdatePending();
            //}
        }

        //public void StartDownload()
        //{
        //    if (infoStatus == AssetPackInfoStatus.PACKSTATUS_NEEDS_PERMISSION)
        //    {
        //        if (!waitingOnPermissionResult)
        //        {
        //            Debug.Log("Cellular data request retry");
        //            waitingOnPermissionResult = true;
        //            gameAssetPack.RequestCellularDataDownload();
        //        }
        //    }
        //    else if (infoStatus == AssetPackInfoStatus.PACKSTATUS_NEEDS_DOWNLOAD)
        //    {
        //        infoStatus = AssetPackInfoStatus.PACKSTATUS_DOWNLOADING;
        //        gameAssetPack.StartDownload();
        //        SetupDownloadUI();
        //    }
        //}

        //public void SetReady()
        //{
        //    infoStatus = AssetPackInfoStatus.PACKSTATUS_READY;
        //    assetPackDownloadButton.gameObject.SetActive(false);

        //    SetAsPostDownload();
        //}

        //public bool IsDownloaded()
        //{
        //    var ready = false;

        //    if (infoStatus == AssetPackInfoStatus.PACKSTATUS_READY)
        //        ready = true;

        //    return ready;
        //}

        //private void UpdateDownloading()
        //{
        //    if (infoStatus != AssetPackInfoStatus.PACKSTATUS_DOWNLOADING)
        //    {
        //        infoStatus = AssetPackInfoStatus.PACKSTATUS_DOWNLOADING;
        //        SetupDownloadUI();
        //    }

        //    AssetDeliveryStatus deliveryStatus = gameAssetPack.GetStatus();

        //    if (deliveryStatus == AssetDeliveryStatus.Failed)
        //    {
        //        SetupErrorUI();
        //    }
        //    else if (deliveryStatus == AssetDeliveryStatus.Loaded ||
        //                deliveryStatus == AssetDeliveryStatus.Available)
        //    {
        //        if (gameAssetPack.IsSingleAssetBundlePack)
        //        {
        //            //AssetBundle newBundle = gameAssetPack.FinishBundleDownload();
        //            //StartCoroutine(LoadAssetsFromBundle(newBundle));

        //            SetReady();
        //        }
        //    }
        //    else if (deliveryStatus == AssetDeliveryStatus.Loading)
        //    {
        //        assetPackStatusLabel.text = "Loading";
        //        assetPackProgressBar.PercentComplete = 1.0f;
        //    }
        //    else if (deliveryStatus == AssetDeliveryStatus.Retrieving)
        //    {
        //        assetPackProgressBar.PercentComplete = gameAssetPack.GetDownloadProgress();
        //    }
        //    else if (deliveryStatus == AssetDeliveryStatus.WaitingForWifi)
        //    {
        //        Debug.Log("Download status WaitingForWifi");
        //        infoStatus = AssetPackInfoStatus.PACKSTATUS_NEEDS_PERMISSION;
        //        waitingOnPermissionResult = true;
        //        //gameAssetPack.RequestCellularDataDownload();
        //    }
        //}

        //private void SetupDownloadUI()
        //{
        //    assetPackDownloadButton.gameObject.SetActive(false);
        //}

        //private void SetupErrorUI()
        //{
        //    assetPackStatusLabel.text = String.Format("Error : {0}", gameAssetPack.GetError().ToString());
        //    assetPackDownloadButton.gameObject.SetActive(false);
        //    assetPackProgressBar.gameObject.SetActive(false);
        //}

        //public void SetAsPostDownload(bool isObjectMinimised = false)
        //{
        //    var assetPackInfoPrefabHeight = 0.5f;

        //    // AFTER DOWNLOAD CHANGE SIZE OF OBJECT

        //    if (isObjectMinimised)
        //    {
        //        assetPackInfoPrefabHeight = 0.95f;
        //    }

        //    assetPackContainer.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(assetPackInfoContainerDefaultWidth, assetPackInfoPrefabHeight * 0.5f);
        //    assetPackLabelContainer.gameObject.transform.localPosition = new Vector3();

        //    assetPackContainer.GetComponent<RectTransform>().localScale = new Vector3(1.0f, assetPackInfoPrefabHeight, 1.0f);
        //}

        //public void PermissionResult(bool allow)
        //{
        //    if (allow)
        //    {
        //        infoStatus = AssetPackInfoStatus.PACKSTATUS_DOWNLOADING;
        //        SetupDownloadUI();
        //    }
        //    else
        //    {
        //        // If no permission granted, reset UI to allow asking again
        //        // by tapping Download
        //        UpdatePending();
        //        // But keep us in a needs permission state since we are keeping
        //        // the underlying asset pack request live
        //        infoStatus = AssetPackInfoStatus.PACKSTATUS_NEEDS_PERMISSION;
        //    }
        //}

        //private void UpdatePending()
        //{
        //    // We need to wait for the async size request to return the
        //    // size of the asset pack
        //    if (gameAssetPack.IsAssetPackSizeValid)
        //    {
        //        infoStatus = AssetPackInfoStatus.PACKSTATUS_NEEDS_DOWNLOAD;
        //        long assetPackSize = gameAssetPack.AssetPackSize;
        //        float assetPackSizeMB = (float)(((double)assetPackSize) / (1024.0 * 1024.0));
        //        string downloadText = string.Format("Download {0:0.#} MB", assetPackSizeMB);
        //        assetPackDownloadButton.gameObject.SetActive(true);
        //    }
        //}

        //IEnumerator LoadAssetsFromBundle(AssetBundle assetBundle)
        //{
        //    var bundleAssets = assetBundle.GetAllAssetNames();
        //    var assetQueue = new Queue<string>(bundleAssets);
        //    while (assetQueue.Count > 0)
        //    {
        //        string assetName = assetQueue.Dequeue();
        //        AssetBundleRequest request = assetBundle.LoadAssetAsync<Backgammon_Asset.MatchReplayDLC>(assetName);
        //        yield return request;

        //        Debug.Log(assetName + " WAS LOADED");
        //    }
        //    SetReady();
        //}

        // ---------------------------------------------------- GETTERS -------------------------------------------------

        public string InAppPurchaseID
        {
            get => inAppPurchaseID;
            private set => inAppPurchaseID = value;
        }

        public string AssetPackName
        {
            get => assetPackName;
            private set => assetPackName = value;
        }
    }
}
