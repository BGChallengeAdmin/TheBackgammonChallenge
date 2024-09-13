using UnityEngine;

namespace Backgammon
{
    public class HomeManager : MonoBehaviour
    {
        [Header("HOME")]
        [SerializeField] Home homePrefab = null;

        [Header("TRANSFORMS")]
        [SerializeField] Transform _homeLeft = null;
        [SerializeField] Transform _homeRight = null;

        [Header("COUNTER")]
        [SerializeField] Counter _counterPrefab;

        internal void Init(bool playingFromLeft = true)
        {
            // DEFAULT SETTINGS TO LEFT HAND SIDE

            if (_playerHome is null) _playerHome = Instantiate(homePrefab, this.transform);
            _playerHome.name = "PlayerHome";
            
            if (_opponenetHome is null) _opponenetHome = Instantiate(homePrefab, Vector3.zero, Quaternion.Euler(0f, 180f, 0f), this.transform);
            _opponenetHome.name = "OpponentHome";
            _opponenetHome.IsUpper = true;

            Mesh homeMesh = _homeLeft.GetComponentsInChildren<MeshFilter>()[0].sharedMesh;
            var homeBarDepth = homeMesh.bounds.size.z;
            var homeBarHeight = homeMesh.bounds.size.y;

            Mesh counterMesh = _counterPrefab.GetComponentsInChildren<MeshFilter>()[0].sharedMesh;
            var counterHeight = counterMesh.bounds.size.y;

            if (playingFromLeft)
            {
                var pos = _homeLeft.transform.position;

                _playerHome.transform.position = new Vector3(pos.x - 0.005f, 2 * homeBarHeight + (0.5f * counterHeight), -0.5f * homeBarDepth);
                _opponenetHome.transform.position = new Vector3(pos.x - 0.005f, 2 * homeBarHeight + (0.5f * counterHeight), 0.5f * homeBarDepth);
            }
            else
            {
                var pos = _homeRight.transform.position;

                _playerHome.transform.position = new Vector3(pos.x + 0.005f, 2 * homeBarHeight + (0.5f * counterHeight), -0.5f * homeBarDepth);
                _playerHome.transform.rotation = Quaternion.Euler(0f, 180f, 0f);

                _opponenetHome.transform.position = new Vector3(pos.x + 0.005f, 2 * homeBarHeight + (0.5f * counterHeight), 0.5f * homeBarDepth);
                _opponenetHome.transform.rotation = Quaternion.identity;
            }
        }

        // PUSH COUNTERS

        internal void PushCounterToPlayerHome(Counter counter)
        {
            _playerHome.PushCounter(counter);
        }

        internal void PushCounterToOpponentHome(Counter counter)
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

        private Home _playerHome = null;
        private Home _opponenetHome = null;

        public Home PlayerHome { get => _playerHome; }
        public Home OpponentHome { get => _opponenetHome; }
    }
}