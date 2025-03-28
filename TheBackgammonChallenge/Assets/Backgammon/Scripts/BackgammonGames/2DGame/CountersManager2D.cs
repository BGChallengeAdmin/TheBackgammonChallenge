using System.Collections.Generic;
using UnityEngine;

namespace Backgammon
{
    public class CountersManager2D : MonoBehaviour
    {
        [Header("DEBUG")]
        [SerializeField] DebugPrefab debug_counterManager = null;

        [Header("TRANSFORMS")]
        [SerializeField] Transform _blackCountersContainer = null;
        [SerializeField] Transform _whiteCountersContainer = null;
        [SerializeField] Transform _movingCountersContainer = null;

        [Header("COUNTER")]
        [SerializeField] Counter2DPrefab counterPrefab;

        [Header("MATERIALS")]
        [SerializeField] Material _defaultBlackMaterial = null;
        [SerializeField] Material _defaultWhiteMaterial = null;
        private List<MeshRenderer> _blackCountersRenderers;
        private List<MeshRenderer> _whiteCountersRenderers;

        [Header("SPRITES")]
        [SerializeField] Sprite _defaultBlackSpriteImage = null;
        [SerializeField] Sprite _defaultWhiteSpriteImage = null;
        [SerializeField] Sprite _defaultBlackSpriteEdgeImage = null;
        [SerializeField] Sprite _defaultWhiteSpriteEdgeImage = null;

        private void Awake()
        {
            _blackCountersArray = new Counter2DPrefab[15];
            _whiteCountersArray = new Counter2DPrefab[15];

            _blackCountersRenderers = new List<MeshRenderer>();
            _whiteCountersRenderers = new List<MeshRenderer>();
        }

        internal void Init(float scaledCounterSize)
        {
            ClearAllCountersPrefabs();

            var ownerBlack = Game2D.Context.PlayingAs == Game2D.PlayingAs.PLAYER_1 ?
                    (Game2D.Context.IfPlayerIsBlack ? Game2D.PlayingAs.PLAYER_1 : Game2D.PlayingAs.PLAYER_2) :
                    (Game2D.Context.IfPlayerIsBlack ? Game2D.PlayingAs.PLAYER_2 : Game2D.PlayingAs.PLAYER_1);

            var ownerWhite = Game2D.Context.PlayingAs == Game2D.PlayingAs.PLAYER_1 ?
                    (!Game2D.Context.IfPlayerIsBlack ? Game2D.PlayingAs.PLAYER_1 : Game2D.PlayingAs.PLAYER_2) :
                    (!Game2D.Context.IfPlayerIsBlack ? Game2D.PlayingAs.PLAYER_2 : Game2D.PlayingAs.PLAYER_1);

            // PROPORTION COUNTERS TO POINT SIZES
            var point = Game2D.Context.PointsManager.GetPointByID(1);
            _scaledCounterSize = scaledCounterSize;
            _scaledCounterSizeToHome = -1;
            
            for (int c = 0; c < 15; c++)
            {
                var black = Instantiate(counterPrefab, this.transform.localPosition, Quaternion.identity, _blackCountersContainer);
                black.InitCounter((c + 1), Game2D.PlayerColour.BLACK, ownerBlack, _movingCountersContainer, _blackCountersContainer);
                SetCounterSizeAndColour(black, true);

                _blackCountersArray[c] = black;
                _blackCountersRenderers.Add(black.GetComponentInChildren<MeshRenderer>());

                var white = Instantiate(counterPrefab, this.transform.localPosition, Quaternion.identity, _whiteCountersContainer);
                white.InitCounter((c + 1), Game2D.PlayerColour.WHITE, ownerWhite, _movingCountersContainer, _whiteCountersContainer);
                SetCounterSizeAndColour(white, true);
                
                _whiteCountersArray[c] = white;
                _whiteCountersRenderers.Add(white.GetComponentInChildren<MeshRenderer>());
            }
            
            ZeroCounterPositions();

            debug_counterManager.DebugMessage($"*** COUNTERS CREATED ***");
        }

        internal float ConfigureCountersDimsToHome()
        {
            var counter = _blackCountersArray[0];

            SetCounterSizeAndColour(counter, false);
            var edgeDims = counter.gameObject.GetComponent<RectTransform>().rect;

            SetCounterSizeAndColour(counter, true);
            var flatDims = counter.GetComponent<RectTransform>().rect;

            _scaledCounterSizeToHome= edgeDims.height / flatDims.height;

            return _scaledCounterSizeToHome;
        }

        internal Counter2DPrefab GetCounterByColourAndID(Counter2DPrefab counter)
        {
            if (counter.Colour == Game2D.PlayerColour.BLACK)
            {
                _activeCounter = _blackCountersArray[counter.ID];
                return _activeCounter;
            }
            else if (counter.Colour == Game2D.PlayerColour.WHITE)
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

        internal void SetCounterAsActive(Counter2DPrefab counter)
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

            foreach (Counter2DPrefab bC in _blackCountersArray)
            {
                if (bC.IsMoving)
                {
                    moving = true;
                    break;
                }
            }

            if (!moving)
            {
                foreach (Counter2DPrefab wC in _blackCountersArray)
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

        internal void SetCounterSizeAndColour(Counter2DPrefab counter, bool asPlayingOrHome)
        {
            var counterSprite = _defaultBlackSpriteImage;

            if (counter.Colour == Game2D.PlayerColour.BLACK)
                counterSprite = asPlayingOrHome ? _defaultBlackSpriteImage : _defaultBlackSpriteEdgeImage;
            else
                counterSprite = asPlayingOrHome ? _defaultWhiteSpriteImage : _defaultWhiteSpriteEdgeImage;

            counter.gameObject.GetComponent<RectTransform>().sizeDelta = 
                new Vector2(_scaledCounterSize, (asPlayingOrHome ? 1f : 
                           (_scaledCounterSizeToHome > 0 ? _scaledCounterSizeToHome : .2f)) * _scaledCounterSize);
            counter.SetSourceImage(counterSprite);
        }

        private void ClearAllCountersPrefabs()
        {
            _blackCountersRenderers.Clear();
            _whiteCountersRenderers.Clear();
            _activeCounter = null;
            
            foreach (Transform counter in _movingCountersContainer)
            {
                Destroy(counter.gameObject);
            }

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

        private Counter2DPrefab[] _blackCountersArray = null;
        private Counter2DPrefab[] _whiteCountersArray = null;
        private Counter2DPrefab _activeCounter = null;
        
        private float _scaledCounterSize = 1f;
        private float _scaledCounterSizeToHome = -1f;

        public Counter2DPrefab[] BlackCounters { get => _blackCountersArray; }
        public Counter2DPrefab[] WhiteCounters { get => _whiteCountersArray; }

        public Sprite DefaultBlackCounterImage { get => _defaultBlackSpriteImage; }
        public Sprite DefaultWhiteCounterImage { get => _defaultWhiteSpriteImage; }
    }
}