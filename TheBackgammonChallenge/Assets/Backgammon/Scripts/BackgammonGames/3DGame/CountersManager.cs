using System.Collections.Generic;
using UnityEngine;

namespace Backgammon
{
	public class CountersManager : MonoBehaviour
	{
        [Header("DEBUG")]
        [SerializeField] DebugPrefab debug_counterManager = null;

        [Header("TRANSFORMS")]
        [SerializeField] Transform _blackCountersContainer = null;
        [SerializeField] Transform _whiteCountersContainer = null;

		[Header("COUNTER")]
		[SerializeField] Counter counterPrefab;

        [Header("MATERIALS")]
        [SerializeField] Material _defaultBlackMaterial = null;
        [SerializeField] Material _defaultWhiteMaterial = null;
        private List<MeshRenderer> _blackCountersRenderers;
        private List<MeshRenderer> _whiteCountersRenderers;

        private void Awake()
        {
            _blackCountersArray = new Counter[15];
            _whiteCountersArray = new Counter[15];

            _blackCountersRenderers = new List<MeshRenderer>();
            _whiteCountersRenderers = new List<MeshRenderer>();
        }

        internal void Init()
        {
            ClearAllCountersPrefabs();

            var ownerBlack = Game.Context.PlayingAs == Game.PlayingAs.PLAYER_1 ?
                    (Game.Context.IfPlayerIsBlack ? Game.PlayingAs.PLAYER_1 : Game.PlayingAs.PLAYER_2) :
                    (Game.Context.IfPlayerIsBlack ? Game.PlayingAs.PLAYER_2 : Game.PlayingAs.PLAYER_1);

            var ownerWhite = Game.Context.PlayingAs == Game.PlayingAs.PLAYER_1 ?
                    (!Game.Context.IfPlayerIsBlack ? Game.PlayingAs.PLAYER_1 : Game.PlayingAs.PLAYER_2) :
                    (!Game.Context.IfPlayerIsBlack ? Game.PlayingAs.PLAYER_2 : Game.PlayingAs.PLAYER_1);

            for (int c = 0; c < 15; c++)
            {
                var black = Instantiate(counterPrefab, _blackCountersContainer);
                black.InitCounter((c + 1), Game.PlayerColour.BLACK, ownerBlack);
                black.SetCounterColour(_defaultBlackMaterial);
                _blackCountersArray[c] = black;
                _blackCountersRenderers.Add(black.GetComponentInChildren<MeshRenderer>());

                var white = Instantiate(counterPrefab, _whiteCountersContainer);
                white.InitCounter((c + 1), Game.PlayerColour.WHITE, ownerWhite);
                white.SetCounterColour(_defaultWhiteMaterial);
                _whiteCountersArray[c] = white;
                _whiteCountersRenderers.Add(white.GetComponentInChildren<MeshRenderer>());
            }

            _activeCounter = null;

            debug_counterManager.DebugMessage($"*** COUNTERS CREATED ***");
        }

        internal Counter GetCounterByColourAndID(Counter counter)
        {
            if (counter.Colour == Game.PlayerColour.BLACK)
            {
                _activeCounter = _blackCountersArray[counter.ID];
                return _activeCounter;
            }
            else if (counter.Colour == Game.PlayerColour.WHITE)
            {
                _activeCounter = _whiteCountersArray[counter.ID];
                return _activeCounter;
            }
            else
            {
                debug_counterManager.DebugMessage($"ERROR: NO COUNTER ID: {counter.ID} -> FOUND IN: {counter.Colour.ToString()}");
                return null;
            }
        }

        internal void ZeroCounterPositions()
        {
            for (int b = 0; b < _blackCountersArray.Length; b++)
            {
                if (_blackCountersArray[b] is not null)
                {
                    _blackCountersArray[b].gameObject.transform.position = new Vector3(0f, .05f, .05f);
                    _blackCountersArray[b].gameObject.transform.rotation = Quaternion.identity;
                }
            }

            for (int w = 0; w < _whiteCountersArray.Length; w++)
            {
                if (_whiteCountersArray[w] is not null)
                {
                    _whiteCountersArray[w].gameObject.transform.position = new Vector3(0f, .05f, -.05f);
                    _whiteCountersArray[w].gameObject.transform.rotation = Quaternion.identity;
                }
            }
        }

        // ------------------------------------------- HELPER METHODS -------------------------------------------

        internal void SetCounterAsActive(Counter counter)
        {
            _activeCounter = counter;
        }

        internal bool TestIfActiveCounterMoving()
        {
            return _activeCounter is null ? false : _activeCounter.IsMoving;
        }

        internal bool TestIfAnyCountersMoving()
        {
            bool moving = false;

            foreach(Counter bC in _blackCountersArray)
            {
                if (bC.IsMoving)
                {
                    moving = true;
                    break;
                }
            }

            if (!moving)
            {
                foreach (Counter wC in _blackCountersArray)
                {
                    if (wC.IsMoving)
                    {
                        moving = true;
                        break;
                    }
                }
            }

            return moving;
        }

        internal void SetCounterColours(bool blackOrWhite, Material material)
        {
            if (blackOrWhite)
            {
                foreach (MeshRenderer mesh in _blackCountersRenderers)
                {
                    mesh.material = material;
                }
            }
            else
            {
                foreach (MeshRenderer mesh in _whiteCountersRenderers)
                {
                    mesh.material = material;
                }
            }
        }

        private void ClearAllCountersPrefabs()
        {
            _blackCountersRenderers.Clear();
            _whiteCountersRenderers.Clear();

            foreach (Transform counter in _blackCountersContainer)
            {
                Destroy(counter.gameObject);
            }

            foreach (Transform counter in _whiteCountersContainer)
            {
                Destroy(counter.gameObject);
            }
        }
        
        // --------------------------------------- GETTERS && SETTERS ----------------------------------------

        private Counter[] _blackCountersArray = null;
        private Counter[] _whiteCountersArray = null;
        private Counter _activeCounter = null;

        public Counter[] BlackCounters { get => _blackCountersArray; }
        public Counter[] WhiteCounters { get => _whiteCountersArray; }
    }
}