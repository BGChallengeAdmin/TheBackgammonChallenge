using UnityEngine;

namespace Backgammon
{
    public class DoublingManager : MonoBehaviour
    {
        [Header("DOUBLING")]
        [SerializeField] Doubling _doublingPrefab = null;

        [Header("TRANSFORMS")]
        [SerializeField] Transform _doublingLeft = null;
        [SerializeField] Transform _doublingRight = null;

        internal void Init(bool playingFromLeft = true)
        {
            _cubeValue = 1;

            if (_doubling is null) _doubling = Instantiate(_doublingPrefab, this.transform);
            _doubling.name = "Doubling";

            Vector3 position;
            var offset = _doubling.GetCubeHeight() * 0.5f;

            if (playingFromLeft)
            {
                position = new Vector3(_doublingRight.position.x, _doublingRight.position.y + offset, 0);
            }
            else
            {
                position = new Vector3(_doublingLeft.position.x, _doublingLeft.position.y + offset, 0);
            }

            _doubling.transform.position = position;
            _doubling.Init();
        }

        internal void SetDoublingActive(bool active)
        {
            _doubling.SetActive(active);
        }

        internal void SetCubeToMove(bool upper)
        {
            _doubling.SetCubeToMove(upper);
        }

        internal void SetCubeValue(int value)
        {
            _cubeValue = value;
        }

        internal void SetCubeOwner(Game.PlayerColour owner)
        {
            _cubeOwnerColour = owner;
        }

        internal void SetCubeOwner(Game.PlayingAs owner)
        {
            _cubeOwnerPlayer = owner;
        }

        private Doubling _doubling = null;
        private int _cubeValue = 1;
        private Game.PlayerColour _cubeOwnerColour = Game.PlayerColour.NONE;
        private Game.PlayingAs _cubeOwnerPlayer = Game.PlayingAs.NONE;

        public int CubeValue { get => _cubeValue; }
        public Game.PlayerColour CubeOwnerColour { get => _cubeOwnerColour; }
        public Game.PlayingAs CubeOwner { get => _cubeOwnerPlayer; }
        internal bool DoublingWasClicked { get => _doubling.DoublingWasClicked; }
    }
}