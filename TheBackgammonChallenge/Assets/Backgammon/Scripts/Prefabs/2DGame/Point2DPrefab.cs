using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Backgammon
{
    public class Point2DPrefab : PlayablePosition2D
    {
        [Header("DEBUG")]
        [SerializeField] DebugPrefab debug_point;

        [Header("COUNTER")]
        [SerializeField] Counter2DPrefab _counterPrefab;

        [Header("MATERIALS")]
        [SerializeField] MeshRenderer _pointMesh;
        [SerializeField] MeshRenderer _pointHightlightMesh;
        [SerializeField] Material _pointHighlightMaterial;
        [SerializeField] Material _pointSelectedMaterial;

        [Header("TEXTURES")]
        [SerializeField] RawImage _pointImageTexture;
        [SerializeField] Image _pointImage;

        internal event Action<Point2DPrefab> OnPointClickedEvent;

        private void Awake()
        {
            //Mesh mesh = _counterPrefab.GetComponentsInChildren<MeshFilter>()[0].sharedMesh;
            //_counterDepth = mesh.bounds.size.z;
            //_counterHeight = mesh.bounds.size.y;

            // NOTE: HIGHLIGHTS NOT USED - CAUSES FLICKERING BETWEEN POINTS
            //_pointHightlightMesh.gameObject.SetActive(false);

            //_counterHeight = _counterPrefab.GetComponent<RectTransform>().rect.height;
        }

        internal void Init(int pointID, bool isUpper, Material pointMaterial, Texture texture, Color colour)
        {
            _pointID = pointID;
            _isUpper = isUpper;
            //_material = pointMaterial;
            //SetMaterial(_material);
            //SetTexture(texture);
            SetColour(colour);

            _isInteractive = false;
            _isSelected = false;
            _isHighlighted = false;
            SetIsHighlighted();

            ResetCounters();
        }

        public void OnClickPointButton()
        {
            if (!_isInteractive) return;

            _isSelected = !_isSelected;
            _isHighlighted = !_isHighlighted;
            SetIsHighlighted(_isHighlighted);

            if (OnPointClickedEvent != null) OnPointClickedEvent(this);
            else debug_point.DebugMessage($"POINT CLICK EVENT IS NULL");
        }

        // NOTE: NOT USED
        public void OnMouseDown()
        {
            if (!_isInteractive) return;

            _isSelected = !_isSelected;
            _isHighlighted = !_isHighlighted;
            SetIsHighlighted(_isHighlighted);

            if (OnPointClickedEvent != null) OnPointClickedEvent(this);
            else debug_point.DebugMessage($"POINT CLICK EVENT IS NULL");
        }

        // ------------------------------------------ COUNTER METHODS -------------------------------------------

        internal override void PushCounter(Counter2DPrefab counter)
        {
            counter.ParentID = ID;

            _owner = counter.Owner;
            _colour = counter.Colour;

            if (_countersOnPointStack is null)
            {
                debug_point.DebugMessage($"ERROR: {_pointID} STACK IS NULL");
            }
            else
            {
                _countersOnPointStack.Push(counter);
                _activeCounters += 1;
            }
        }

        internal override Counter2DPrefab PopCounter()
        {
            Counter2DPrefab counter = null;

            if (_countersOnPointStack is null)
            {
                debug_point.DebugMessage($"ERROR: {_pointID} STACK IS NULL");
            }
            else
            {
                if (Counters < 1)
                {
                    debug_point.DebugMessage($"ERROR: {Counters} COUNTERS ON POINT {_pointID} IS INVALID POP!!!");
                }

                counter = _countersOnPointStack.Pop();
                _activeCounters -= 1;

                if (Counters <= 0)
                {
                    _colour = Game2D.PlayerColour.NONE;
                    _owner = Game2D.PlayingAs.NONE;
                }
            }

            return counter;
        }

        internal override Vector3 GetCounterOffsetPosition()
        {
            // NOTE: IF '_isUpper' NEED TO USE POSITIVE OFFSET

            var up = (_isUpper ? -1f : 1f);

            // DEFAULT TO '1' POSITION
            var offsetTransform = new Vector3(gameObject.transform.localPosition.x,
                                              gameObject.transform.localPosition.y + .5f * CounterHeight * up,
                                              0);

            // NOTE: MUST BE ABLE TO STACK 15 COUNTERS ON SINGLE POINT
            switch (_activeCounters)
            {
                case < 5: // FIRST ROW - 5
                    {
                        // OFFSET STARTS AT COUNTER '1' - IF == 1 RETURNS DEFAULT
                        offsetTransform += new Vector3(0f, (_activeCounters) * CounterHeight * up, 0f);
                    }
                    break;
                case < 9: // SECOND ROW - 4
                    {
                        // PLACED HALF IN SECOND ROW + '0' OFFSET - ALREADY 5 COUNTERS PLACED
                        offsetTransform += new Vector3(0f, ((_activeCounters - 4) - 0.5f) * CounterHeight * up, 0f);
                    }
                    break;
                case < 12: // THIRD ROW - 3
                    {
                        // PLACED FULL IN THIRD ROW - ALREADY 9 COUNTERS PLACED
                        offsetTransform += new Vector3(0f, (_activeCounters - 8) * CounterHeight * up, 0f);
                    }
                    break;
                case < 14: // FOURTH ROW - 2
                    {
                        // PLACED HALF IN FOURTH ROW + '1' OFFSET - ALREADY 12 COUNTERS PLACED
                        offsetTransform += new Vector3(0f, ((_activeCounters - 11) - 0.5f + 1) * CounterHeight * up, 0f);
                    }
                    break; // TOP ROW - 1 - 15 TOTAL
                case 14:
                    {
                        // PLACED FULL IN TOP ROW + 1 OFFSET - ALREADY 14 COUNTERS PLACED
                        offsetTransform += new Vector3(0f, ((_activeCounters - 13) + 1) * CounterHeight * up, 0f);
                    }
                    break;
            }

            return offsetTransform;
        }

        internal Vector3 GetActiveCounterPosition(int counterNumber = 1)
        {
            var position = Vector3.zero;
            var offset = 0;

            if (Counters == 0) _activeCounters = counterNumber - 1;
            else _activeCounters -= counterNumber;

            if (_activeCounters < 0)
            {
                offset = _activeCounters;
                _activeCounters = 0;
            }

            position = GetCounterOffsetPosition();

            if (Counters == 0) _activeCounters = 0;
            else _activeCounters += counterNumber + offset;

            return position;
        }

        internal void ResetCounters()
        {
            _countersOnPointStack = new Stack<Counter2DPrefab>();
            _activeCounters = 0;

            _colour = Game2D.PlayerColour.NONE;
            _owner = Game2D.PlayingAs.NONE;
        }

        internal void SetCounterAsSelected(bool selected)
        {
            _countersOnPointStack.Peek().SetAsSelected(selected);
        }

        // ------------------------------------------- HELPER METHODS -------------------------------------------

        internal void SetMaterial(Material newMaterial)
        {
            _material = newMaterial;
            _pointMesh.material = _material;
            _pointHightlightMesh.material = _material;
        }

        internal void SetTexture(Texture newTexture)
        {
            _pointImageTexture.texture = newTexture;
        }

        internal void SetColour(Color colour)
        {
            _pointImage.color = colour;
        }

        internal void SetInteractive(bool interactive, bool highlight = false, bool selected = false)
        {
            _isInteractive = interactive;

            SetIsHighlighted(highlight, selected);

            if (!_isInteractive)
            {
                _isSelected = false;
                _isHighlighted = false;
                SetIsHighlighted();
            }
        }

        internal void SetIsHighlighted(bool isHighlighted = false, bool selected = false)
        {
            _isHighlighted = isHighlighted;
            // NOTE: NOT USING THE HIGHLIGHT OBJECT - USING DEBUG_SPHERE
            //_pointHightlightMesh.gameObject.SetActive(_isHighlighted);
        }

        internal override int GetID() { return ID; }

        // ----------------------------------------- GETTERS && SETTERS -----------------------------------------

        private int _pointID;
        private bool _isUpper = false;
        private Game2D.PlayerColour _colour = Game2D.PlayerColour.NONE;
        private Game2D.PlayingAs _owner = Game2D.PlayingAs.NONE;
        private bool _isInteractive = false;
        private bool _isHighlighted = false;
        private bool _isSelected = false;

        private Stack<Counter2DPrefab> _countersOnPointStack;
        private int _activeCounters = 0;
        public static float CounterHeight = 0f;

        private Material _material;

        public int ID { get => _pointID; }
        public Game2D.PlayerColour Colour { get => _colour; }
        public Game2D.PlayingAs Owner { get => _owner; }
        public int Counters { get => _countersOnPointStack.Count; }
        public bool IsInteractive { get => _isInteractive; }
        public bool IsSelected { get => _isSelected; }
        public MeshRenderer Renderer { get => _pointMesh; }
    }
}