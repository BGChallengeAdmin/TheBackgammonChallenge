using UnityEngine;

namespace Backgammon
{
    public class BarManager2D : MonoBehaviour
    {
        [Header("BAR")]
        [SerializeField] Bar2DPrefab _barPrefab = null;

        [Header("COUNTER")]
        [SerializeField] Counter2DPrefab _counterPrefab = null;

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

            var counterHeight = _counterPrefab.GetComponent<RectTransform>().rect.height;

            // PLAYER IS LOWER
            _playerBar.transform.localPosition = new Vector3(0, -1.5f * counterHeight, 0);
            _opponentBar.transform.localPosition = new Vector3(0, 1.5f * counterHeight, 0);
        }

        // PUSH COUNTERS

        internal void PushCounterToPlayerBar(Counter2DPrefab counter)
        {
            _playerBar.PushCounter(counter);
        }

        internal void PushCounterToOpponentBar(Counter2DPrefab counter)
        {
            _opponentBar.PushCounter(counter);
        }

        // POP COUNTERS

        internal Counter2DPrefab PopCounterFromPlayerBar()
        {
            return _playerBar.PopCounter();
        }

        internal Counter2DPrefab PopCounterFromOpponentBar()
        {
            return _opponentBar.PopCounter();
        }

        // COUNTER INTERACTIONS

        internal void SetPlayerCounterAsSelected(bool selected)
        {
            _playerBar.SetCounterAsSelected(selected);
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

        private Bar2DPrefab _playerBar = null;
        private Bar2DPrefab _opponentBar = null;

        public Bar2DPrefab PlayerBar { get => _playerBar; }
        public Bar2DPrefab OpponentBar { get => _opponentBar; }
    }
}