using UnityEngine;

namespace Backgammon
{
    public class BoardMaterialsManager : MonoBehaviour
    {
        [Header("BOARD RENDERERS")]
        [SerializeField] Renderer _boardLeft;
        [SerializeField] Renderer _boardRight;

        [Header("EDGE RENDERERS")]
        [SerializeField] Renderer[] _edgesLeft;
        [SerializeField] Renderer[] _edgesRight;

        [Header("HOME RENDERERS")]
        [SerializeField] Renderer _homeLeft;
        [SerializeField] Renderer _homeRight;

        internal void Init(BoardDesignSO boardDesignSO)
        {
            SetBoardMaterial(boardDesignSO.BoardMaterial);
            SetEdgeMaterial(boardDesignSO.EdgeMaterial);
            SetHomeMaterial(boardDesignSO.HomeMaterial);
            SetPointsBlackMaterial(boardDesignSO.PointBlackMaterial);
            SetPointsWhiteMaterial(boardDesignSO.PointWhiteMaterial);
            SetBlackCountersMaterial(boardDesignSO.CounterBlackMaterial);
            SetWhiteCountersMaterial(boardDesignSO.CounterWhiteMaterial);
        }

        internal void SetBoardMaterial(Material material)
        {
            _boardLeft.material = material;
            _boardRight.material = material;
        }

        internal void SetEdgeMaterial(Material material) 
        {
            foreach (Renderer edgeL in _edgesLeft) edgeL.material = material;
            foreach (Renderer edgeR in _edgesRight) edgeR.material = material;
        }

        internal void SetHomeMaterial(Material material)
        {
            _homeLeft.material = material;
            _homeRight.material = material;
        }

        internal void SetPointsBlackMaterial(Material material)
        {
            Game.Context.PointsManager.SetPointsColours(true, material);
        }

        internal void SetPointsWhiteMaterial(Material material)
        {
            Game.Context.PointsManager.SetPointsColours(false, material);
        }

        internal void SetBlackCountersMaterial(Material material)
        {
            Game.Context.CountersManager.SetCounterColours(true, material);
        }

        internal void SetWhiteCountersMaterial(Material material)
        {
            Game.Context.CountersManager.SetCounterColours(false, material);
        }
    }
}