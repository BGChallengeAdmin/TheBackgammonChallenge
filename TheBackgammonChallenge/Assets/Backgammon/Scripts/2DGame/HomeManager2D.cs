using UnityEngine;

namespace Backgammon
{
    public class HomeManager2D : MonoBehaviour
    {
        [Header("HOME")]
        [SerializeField] Home2DPrefab homePrefab = null;

        [Header("TRANSFORMS")]
        [SerializeField] Transform _boardEdgeLeft = null;
        [SerializeField] Transform _boardEdgeRight = null;

        [Header("COUNTER")]
        [SerializeField] Counter2DPrefab _counterPrefab;

        [Header("DIMENSIONS OF HOME TRAY")]
        [SerializeField] float verticalPosition = .48f;
        [SerializeField] float percentHeight = .40f;
        [SerializeField] float percentWidth = .70f;

        internal void Init(bool playingFromLeft = true)
        {
            // DEFAULT SETTINGS TO LEFT HAND SIDE
            if (_playerHome is null) _playerHome = Instantiate(homePrefab, this.transform);
            _playerHome.name = "PlayerHome";

            if (_opponenetHome is null) _opponenetHome = Instantiate(homePrefab, Vector3.zero, Quaternion.Euler(0f, 0f, 180f), this.transform);
            _opponenetHome.name = "OpponentHome";
            _opponenetHome.IsUpper = true;

            var homeEdgeDims = _boardEdgeLeft.gameObject.GetComponent<RectTransform>().rect;
            homeEdgeDims.x *= -2f;
            homeEdgeDims.y *= -2f;

            var pos = playingFromLeft ? _boardEdgeLeft.transform.localPosition : _boardEdgeRight.transform.localPosition;

            _playerHome.transform.localPosition = new Vector3(pos.x, -1f * verticalPosition * homeEdgeDims.y, 0);
            _playerHome.GetComponent<RectTransform>().sizeDelta = new Vector2(percentWidth * homeEdgeDims.x, 
                                                                              percentHeight * homeEdgeDims.y);

            _opponenetHome.transform.localPosition = new Vector3(pos.x, verticalPosition * homeEdgeDims.y, 0);
            _opponenetHome.GetComponent<RectTransform>().sizeDelta = new Vector2(percentWidth * homeEdgeDims.x, 
                                                                                 percentHeight * homeEdgeDims.y);
        }

        internal void SetScaledCounterSize(float scaledCounterSize)
        {
            _playerHome.Init(scaledCounterSize);
            _opponenetHome.Init(scaledCounterSize); 
        }

        // PUSH COUNTERS
        internal void PushCounterToPlayerHome(Counter2DPrefab counter)
        {
            _playerHome.PushCounter(counter);
        }

        internal void PushCounterToOpponentHome(Counter2DPrefab counter)
        {
            _opponenetHome.PushCounter(counter);
        }

        // RESET COUNTERS

        internal void ResetPlayerHome()
        {
            _playerHome.ResetCounters();
        }

        internal void ResetOpponentHome()
        {
            _opponenetHome.ResetCounters();
        }

        // ----------------------------------------- GETTERS && SETTERS -----------------------------------------

        private Home2DPrefab _playerHome = null;
        private Home2DPrefab _opponenetHome = null;

        public Home2DPrefab PlayerHome { get => _playerHome; }
        public Home2DPrefab OpponentHome { get => _opponenetHome; }
    }
}