using UnityEngine;
using UnityEngine.UI;

namespace Backgammon
{
    public class DoublingManager2D : MonoBehaviour
    {
        [Header("DOUBLING")]
        [SerializeField] Doubling2DPrefab _doublingPrefab = null;

        [Header("TRANSFORMS")]
        [SerializeField] Transform _doublingLeft = null;
        [SerializeField] Transform _doublingRight = null;

        internal void Init(bool playingFromLeft = true)
        {
            ResetCube();

            if (_doubling is null) _doubling = Instantiate(_doublingPrefab, this.transform);
            _doubling.name = "Doubling";

            Vector3 position;
            if (playingFromLeft) position = new Vector3(_doublingRight.localPosition.x, 0, 0);
            else position = new Vector3(_doublingLeft.localPosition.x, 0, 0);
            _doubling.transform.localPosition = position;

            var homeEdgeDims = _doublingLeft.gameObject.GetComponent<RectTransform>().rect;
            homeEdgeDims.x *= -2f;
            homeEdgeDims.y *= -2f;

            var percentHeight = .5f;
            var percentWidth = .80f;

            _doubling.GetComponent<RectTransform>().sizeDelta = new Vector2(percentWidth * homeEdgeDims.x,
                                                                            percentHeight * homeEdgeDims.y);

            var scale = _doublingLeft.GetComponent<RectTransform>().rect.height;
            _doubling.Init(_doublingEdgeScreenPercentage * scale);
        }

        internal Image GetCubeImage()
        {
            return _doubling.GetCubeImage();
        }

        internal void SetDoublingActive(bool active)
        {
            _doubling.SetActive(active);
        }

        internal void TakesDoublingCube(Game2D.PlayingAs owner)
        {
            _cubeOwnerPlayer = owner;

            _cubeValueIndex += 1;
            _cubeValue = (int)Mathf.Pow(2, (_cubeValueIndex - 1));

            SetCubeToMove(owner == Game2D.Context.OpponentAs);
        }

        internal void SetCubeToMove(bool upper)
        {
            _doubling.SetCubeToMove(upper);
        }

        internal void SetCubeValue(int value)
        {
            _cubeValueIndex = value;
            _cubeValue = (int)Mathf.Pow(2, (_cubeValueIndex - 1));
        }

        internal void SetCubeOwner(Game2D.PlayerColour owner)
        {
            _cubeOwnerColour = owner;
        }

        internal void SetCubeOwner(Game2D.PlayingAs owner)
        {
            _cubeOwnerPlayer = owner;
        }

        private void ResetCube()
        {
            _cubeValue = 1;
            _cubeValueIndex = 1;
            _cubeOwnerColour = Game2D.PlayerColour.NONE;
            _cubeOwnerPlayer = Game2D.PlayingAs.NONE;
        }

        private Doubling2DPrefab _doubling = null;
        private float _doublingEdgeScreenPercentage = 0.15f;

        private int _cubeValue = 1;
        private int _cubeValueIndex = 1;
        private Game2D.PlayerColour _cubeOwnerColour = Game2D.PlayerColour.NONE;
        private Game2D.PlayingAs _cubeOwnerPlayer = Game2D.PlayingAs.NONE;

        public int CubeValue { get => _cubeValue; }
        public int CubeValueIndex { get => _cubeValueIndex; }
        public Game2D.PlayerColour CubeOwnerColour { get => _cubeOwnerColour; }
        public Game2D.PlayingAs CubeOwner { get => _cubeOwnerPlayer; }
        internal bool DoublingWasClicked { get => _doubling.DoublingWasClicked; }
    }
}