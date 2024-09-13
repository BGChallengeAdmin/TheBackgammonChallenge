using UnityEngine;

namespace Backgammon
{
    public class BoardMaterialsManager2D : MonoBehaviour
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

        [Header("TRANSFORMS")]
        [SerializeField] Transform _edgeLeft;
        [SerializeField] Transform _edgeRight;
        [SerializeField] Transform _edgeUpper;
        [SerializeField] Transform _edgeLower;
        [SerializeField] Transform _edgeBar;

        internal void ConfigureBoardDimsToScreenSize()
        {
            var screenDims = new Vector2(Screen.width, Screen.height);

            var upperEdgeRect = _edgeUpper.GetComponent<RectTransform>();
            var lowerEdgeRect = _edgeLower.GetComponent<RectTransform>();
            var leftEdgeRect =  _edgeLeft.GetComponent<RectTransform>();
            var rightEdgeRect =  _edgeRight.GetComponent<RectTransform>();
            var barEdge = _edgeBar.GetComponent<RectTransform>();

            var upperLowerScreenPercent = _upperLowedEdgeScreenPercentage * screenDims.y;
            var leftRightScreenPercent = _leftRightEdgeScreenPercent * screenDims.x;
            
            upperEdgeRect.anchoredPosition = new Vector2(0, -.5f * upperLowerScreenPercent);
            upperEdgeRect.offsetMin = new Vector2(0, -upperLowerScreenPercent);
            upperEdgeRect.offsetMax = new Vector2(0, 0);

            lowerEdgeRect.anchoredPosition = new Vector2(0, .5f * upperLowerScreenPercent);
            lowerEdgeRect.offsetMin = new Vector2(0, 0);
            lowerEdgeRect.offsetMax = new Vector2(0, upperLowerScreenPercent);

            leftEdgeRect.anchoredPosition = new Vector2(.5f * leftRightScreenPercent, 0);
            leftEdgeRect.offsetMin = new Vector2(0, upperLowerScreenPercent);
            leftEdgeRect.offsetMax = new Vector2(leftRightScreenPercent, -upperLowerScreenPercent);

            rightEdgeRect.anchoredPosition = new Vector2(-.5f * leftRightScreenPercent, 0);
            rightEdgeRect.offsetMin = new Vector2(-leftRightScreenPercent, upperLowerScreenPercent);
            rightEdgeRect.offsetMax = new Vector2(0, -upperLowerScreenPercent);

            barEdge.sizeDelta = new Vector2(leftRightScreenPercent, screenDims.y - 2 * upperLowerScreenPercent);
            barEdge.offsetMin = new Vector2(-.5f * leftRightScreenPercent, upperLowerScreenPercent);
            barEdge.offsetMax = new Vector2(.5f * leftRightScreenPercent, -upperLowerScreenPercent);
        }

        internal void SetBoardMaterials(BoardDesignSO boardDesignSO)
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

        // -------------------------------------------- FIELDS --------------------------------------------

        private float _upperLowedEdgeScreenPercentage = 0.075f;
        private float _leftRightEdgeScreenPercent = 0.0625f;
    }
}