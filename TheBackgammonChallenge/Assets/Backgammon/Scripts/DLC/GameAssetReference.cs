using UnityEngine;

namespace Backgammon
{
    public class GameAssetReference
    {
        private string assetName;
        private string assetLocation;
        private AssetBundle assetBundle;
        private long assetSize;
        private long assetOffset;

        public string AssetName
        {
            get { return assetName; }
            private set { }
        }

        public string AssetLocation
        {
            get { return assetLocation; }
            private set { }
        }

        public AssetBundle AssetBundle
        {
            get { return assetBundle; }
            private set { }
        }

        public long AssetSize
        {
            get { return assetSize; }
            private set { }
        }

        public long AssetOffset
        {
            get { return assetOffset; }
            private set { }
        }

        public GameAssetReference(string aName, string aLocation,
            AssetBundle aBundle, long aSize, long aOffset)
        {
            assetName = aName;
            assetLocation = aLocation;
            assetBundle = aBundle;
            assetSize = aSize;
            assetOffset = aOffset;
        }
    }
}