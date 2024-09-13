using UnityEngine;

namespace Backgammon
{
    [CreateAssetMenu(fileName = "ScriptableObects/BoardDesign", menuName = "BoardDesignSO")]
    public class BoardDesignSO : ScriptableObject
    {
        public Material BoardMaterial;
        public Material EdgeMaterial;
        public Material HomeMaterial;
        public Material PointBlackMaterial;
        public Material PointWhiteMaterial;
        public Material CounterBlackMaterial;
        public Material CounterWhiteMaterial;
    }
}