using System.Collections;
using UnityEngine;
//using Google.Play.AssetDelivery;

namespace Backgammon
{
    public class GameAssetPack : MonoBehaviour
    {
        private string assetPackName;
        private long assetPackSize;

        // Does the asset pack only contain a single, identically
        // named asset bundle? (created with the plugin interface)
        private bool isSingleAssetBundlePack;

        // Have we retrieved the asset pack size
        private bool isAssetPackSizeValid;

        // Are we requesting cellular data download
        private bool isCellularConfirmationActive;

        // Was the cellular confirmation affirmative
        private bool didApproveCellularData;

        // if isSingleAssetBundlePack is true we use a PlayAssetBundleRequest
        // otherwise we use a PlayAssetPackRequest
        //private PlayAssetBundleRequest assetBundleRequest;
        //private PlayAssetPackRequest assetPackRequest;

        public string AssetPackName
        {
            get { return assetPackName; }
            set
            {
                assetPackName = value;
            }
        }

        public long AssetPackSize
        {
            get { return assetPackSize; }
            private set { }
        }

        public bool IsSingleAssetBundlePack
        {
            get { return isSingleAssetBundlePack; }
            set { isSingleAssetBundlePack = value; }
        }

        public bool IsAssetPackSizeValid
        {
            get { return isAssetPackSizeValid; }
            private set { }
        }

        public bool IsCellularConfirmationActive
        {
            get { return isCellularConfirmationActive; }
            private set { }
        }

        public bool DidApproveCellularData
        {
            get { return didApproveCellularData; }
            private set { }
        }

        //public bool IsDownloaded()
        //{
        //    bool isDownloaded;

        //    if (isSingleAssetBundlePack && assetBundleRequest != null)
        //    {
        //        isDownloaded = assetBundleRequest.IsDone;
        //    }
        //    else
        //    {
        //        isDownloaded = PlayAssetDelivery.IsDownloaded(assetPackName);
        //    }
        //    return isDownloaded;
        //}

        //public void StartDownload()
        //{
        //    if (isSingleAssetBundlePack && assetBundleRequest == null)
        //    {
        //        assetBundleRequest = PlayAssetDelivery.RetrieveAssetBundleAsync(assetPackName);
        //    }
        //    else if (!isSingleAssetBundlePack && assetPackRequest == null)
        //    {
        //        assetPackRequest = PlayAssetDelivery.RetrieveAssetPackAsync(assetPackName);
        //    }
        //}

        //public AssetBundle FinishBundleDownload()
        //{
        //    AssetBundle assetBundle = assetBundleRequest.AssetBundle;
        //    assetBundleRequest = null;
        //    return assetBundle;
        //}

        //public PlayAssetPackRequest FinishPackDownload()
        //{
        //    PlayAssetPackRequest request = assetPackRequest;
        //    assetPackRequest = null;
        //    return request;
        //}

        //public bool IsDownloadActive()
        //{
        //    return assetBundleRequest != null || assetPackRequest != null;
        //}

        //public AssetDeliveryStatus GetStatus()
        //{
        //    if (assetBundleRequest != null)
        //    {
        //        return assetBundleRequest.Status;
        //    }
        //    if (assetPackRequest != null)
        //    {
        //        return assetPackRequest.Status;
        //    }
        //    return AssetDeliveryStatus.Pending;
        //}

        //public AssetDeliveryErrorCode GetError()
        //{
        //    if (assetBundleRequest != null)
        //    {
        //        return assetBundleRequest.Error;
        //    }
        //    if (assetPackRequest != null)
        //    {
        //        return assetPackRequest.Error;
        //    }
        //    return AssetDeliveryErrorCode.NoError;
        //}

        //public float GetDownloadProgress()
        //{
        //    if (assetBundleRequest != null)
        //    {
        //        return assetBundleRequest.DownloadProgress;
        //    }
        //    if (assetPackRequest != null)
        //    {
        //        return assetPackRequest.DownloadProgress;
        //    }
        //    return 0.0f;
        //}

        private void Awake()
        {
            assetPackSize = 0;
            isAssetPackSizeValid = false;
            isCellularConfirmationActive = false;
            didApproveCellularData = false;

            //assetBundleRequest = null;
            //assetPackRequest = null;
        }

        public void InitializeAssetPack(string newAssetPackName, bool isSingleBundlePack)
        {
            AssetPackName = newAssetPackName;
            IsSingleAssetBundlePack = isSingleBundlePack;
            IsAssetPackSizeValid = true;
            isCellularConfirmationActive = true;
            didApproveCellularData = true;
            //StartAssetPackSizeQuery();
        }

        //public void RequestCellularDataDownload()
        //{
        //    if (!isCellularConfirmationActive)
        //    {
        //        didApproveCellularData = false;
        //        isCellularConfirmationActive = true;
        //        StartCoroutine(AskForCellularDataPermission());
        //    }
        //}

        //IEnumerator AskForCellularDataPermission()
        //{
        //    var asyncOperation = PlayAssetDelivery.ShowCellularDataConfirmation();
        //    // Wait until user has confirmed or cancelled the dialog.        
        //    yield return asyncOperation;
        //    bool permissionAllow = asyncOperation.Error ==
        //        AssetDeliveryErrorCode.NoError && asyncOperation.GetResult() ==
        //        ConfirmationDialogResult.Accepted;
        //    if (permissionAllow)
        //    {
        //        yield return new WaitUntil(() =>
        //            GetStatus() != AssetDeliveryStatus.WaitingForWifi);
        //    }
        //    didApproveCellularData = permissionAllow;
        //    isCellularConfirmationActive = false;
        //}

        //private void StartAssetPackSizeQuery()
        //{
        //    var getSizeOperation = PlayAssetDelivery.GetDownloadSize(assetPackName);
        //    getSizeOperation.Completed += (operation) =>
        //    {
        //        if (operation.Error != AssetDeliveryErrorCode.NoError)
        //        {
        //            Debug.LogErrorFormat("Error getting download size for {0}: {1}",
        //                assetPackName, operation.Error);
        //            return;
        //        }

        //        assetPackSize = operation.GetResult();
        //        isAssetPackSizeValid = true;
        //    };
        //}
    }
}