using UnityEngine;

namespace Backgammon
{
    public class BarManager : MonoBehaviour
    {
        [Header("BAR")]
        [SerializeField] Bar _barPrefab = null;

        [Header("TRANSFORMS")]
        [SerializeField] Transform _barLeft = null;
        [SerializeField] Transform _barRight = null;

        [Header("COUNTER")]
        [SerializeField] Counter _counterPrefab = null;

        internal void Init()
        {
            // DEFAULT - PLAYER IS ALWAYS LOWER BUT ACTS THE SAME AS UPPER
            if (_playerBar is null) _playerBar = Instantiate(_barPrefab, Vector3.zero, Quaternion.Euler(0f, 180f, 0f), this.transform);
            _playerBar.name = "PlayerBar";
            _playerBar.IsUpper = true;
            _playerBar.ResetCounters();

            if (_opponentBar is null) _opponentBar = Instantiate(_barPrefab, this.transform);
            _opponentBar.name = "OpponentBar";
            _opponentBar.ResetCounters();

            var midPoint = (_barLeft.position + _barRight.position) / 2;

            Mesh homeMesh = _barLeft.GetComponentsInChildren<MeshFilter>()[0].sharedMesh;
            var barHeight = homeMesh.bounds.size.y;

            Mesh counterMesh = _counterPrefab.GetComponentsInChildren<MeshFilter>()[0].sharedMesh;
            var counterDepth = counterMesh.bounds.size.z;

            // PLAYER IS LOWER
            _playerBar.transform.position = new Vector3(midPoint.x, barHeight, midPoint.z - 1.5f * counterDepth);
            _opponentBar.transform.position = new Vector3(midPoint.x, barHeight, midPoint.z + 1.5f * counterDepth);
        }

        // PUSH COUNTERS

        internal void PushCounterToPlayerBar(Counter counter)
        {
            _playerBar.PushCounter(counter);
        }

        internal void PushCounterToOpponentBar(Counter counter)
        {
            _opponentBar.PushCounter(counter);
        }

        // POP COUNTERS

        internal Counter PopCounterFromPlayerBar()
        {
            return _playerBar.PopCounter();
        }

        internal Counter PopCounterFromOpponentBar()
        {
            return _opponentBar.PopCounter();
        }

        // RESET COUNTERS

        internal void ResetPlayerBar()
        {
            _playerBar.ResetCounters();
        }

        internal void ResetOpponentBar()
        {
            _opponentBar.ResetCounters();
        }

        // ----------------------------------------- GETTERS && SETTERS -----------------------------------------

        private Bar _playerBar = null;
        private Bar _opponentBar = null;

        public Bar PlayerBar { get => _playerBar; }
        public Bar OpponentBar { get => _opponentBar; }
    }
}