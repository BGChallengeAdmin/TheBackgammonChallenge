using System;
using System.Collections.Generic;
using UnityEngine;

namespace Backgammon
{
    public class Point : PlayablePosition
    {
        [Header("DEBUG")]
        [SerializeField] DebugPrefab debug_point;
        [SerializeField] Transform debug_Sphere;

        [Header("COUNTER")]
        [SerializeField] Counter _counterPrefab;

        [Header("MATERIALS")]
        [SerializeField] MeshRenderer _pointMesh;
        [SerializeField] MeshRenderer _pointHightlightMesh;
        [SerializeField] Material _pointHighlightMaterial;
        [SerializeField] Material _pointSelectedMaterial;

        internal event Action<Point> OnPointClickedEvent;

        private void Awake()
        {
            Mesh mesh = _counterPrefab.GetComponentsInChildren<MeshFilter>()[0].sharedMesh;
            _counterDepth = mesh.bounds.size.z;
            _counterHeight = mesh.bounds.size.y;

            // NOTE: HIGHLIGHTS NOT USED - CAUSES FLICKERING BETWEEN POINTS
            _pointHightlightMesh.gameObject.SetActive(false);

            // DEBUGGING - POINTS ALWAYS SHOW ERRORS
            debug_point.ShowMesssage = true;
            debug_Sphere.gameObject.SetActive(false);
        }

        internal void Init(int pointID, bool isUpper, Material pointMaterial)
        {
            _pointID = pointID;
            _isUpper = isUpper;
            _material = pointMaterial;
            SetMaterial(_material);

            _isInteractive = false;
            _isSelected = false;
            _isHighlighted = false;
            SetIsHighlighted();

            ResetCounters();
        }

        private void OnDestroy()
        {
            debug_point.ShowMesssage = false;
        }

        public void OnMouseDown()
        {
            if (!_isInteractive) return;

            _isSelected = !_isSelected;
            _isHighlighted = !_isHighlighted;
            SetIsHighlighted(_isHighlighted);

            if (OnPointClickedEvent != null) OnPointClickedEvent(this);
        }

        // ------------------------------------------ COUNTER METHODS -------------------------------------------

        internal override void PushCounter(Counter counter)
        {
            counter.ParentID = ID;

            _owner = counter.Owner;
            _colour = counter.Colour;
            _countersOnPointStack.Push(counter);
            _activeCounters += 1;

            if (Counters > 15)
            {
                debug_point.DebugMessage($"ERROR: NUMBER OF COUNTERS {Counters} ON POINT {_pointID} IS INVALID!!!");
            }
        }

        internal override Counter PopCounter()
        {
            Counter counter = null;

            if (_countersOnPointStack is null)
            {
                debug_point.DebugMessage($"ERROR: WOOPS!! - NO COUNTERS ON POINT {_pointID}...");
            }
            else
            {
                counter = _countersOnPointStack.Pop();
                _activeCounters -= 1;

                if (Counters <= 0) { _colour = Game.PlayerColour.NONE; }
                {
                    if (Counters < 0)
                    {
                        debug_point.DebugMessage($"ERROR: NUMBER OF COUNTERS {Counters} ON POINT {_pointID} IS INVALID!!!");
                    }
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
                                              gameObject.transform.localPosition.y,
                                              gameObject.transform.localPosition.z + 0.5f * _counterDepth * up);

            // NOTE: MUST BE ABLE TO STACK 15 COUNTERS ON SINGLE POINT
            switch(_activeCounters)
            {
                case < 5: // FIRST ROW - 5
                    {
                        // OFFSET STARTS AT COUNTER '1' - IF == 1 RETURNS DEFAULT
                        offsetTransform += new Vector3(0f, 0f, (_activeCounters) * _counterDepth * up);
                    }
                    break;
                case < 9: // SECOND ROW - 4
                    {
                        // PLACED HALF IN SECOND ROW + '0' OFFSET - ALREADY 5 COUNTERS PLACED
                        offsetTransform += new Vector3(0f, _counterHeight, ((_activeCounters - 4) - 0.5f) * _counterDepth * up);
                    }
                    break;
                case < 12: // THIRD ROW - 3
                    {
                        // PLACED FULL IN THIRD ROW - ALREADY 9 COUNTERS PLACED
                        offsetTransform += new Vector3(0f, 2 * _counterHeight, (_activeCounters - 8) * _counterDepth * up);
                    }
                    break;
                case < 14: // FOURTH ROW - 2
                    {
                        // PLACED HALF IN FOURTH ROW + '1' OFFSET - ALREADY 12 COUNTERS PLACED
                        offsetTransform += new Vector3(0f, 3 * _counterHeight, ((_activeCounters - 11) - 0.5f + 1) * _counterDepth * up);
                    }
                    break; // TOP ROW - 1 - 15 TOTAL
                case 14:
                    {
                        // PLACED FULL IN TOP ROW + 1 OFFSET - ALREADY 14 COUNTERS PLACED
                        offsetTransform += new Vector3(0f, 4 * _counterHeight, ((_activeCounters - 13) + 1) * _counterDepth * up);
                    }
                    break;
            }

            return offsetTransform;
        }

        internal Vector3 GetActiveCounterPosition(int counterNumber = 1)
        {
            var position = Vector3.zero;

            if (Counters == 0) _activeCounters = counterNumber - 1;
            else _activeCounters -= counterNumber;

            position = GetCounterOffsetPosition();

            if (Counters == 0) _activeCounters = 0;
            else _activeCounters += counterNumber;

            return position;
        }

        internal void ResetCounters()
        {
            _countersOnPointStack = new Stack<Counter>();
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

            debug_Sphere.gameObject.SetActive(_isInteractive);

            if (selected) debug_Sphere.GetComponentInChildren<Renderer>().material = _pointSelectedMaterial;
            else if (isHighlighted) debug_Sphere.GetComponentInChildren<Renderer>().material = _pointHighlightMaterial;
        }

        internal override int GetID() { return ID; }

        // ----------------------------------------- GETTERS && SETTERS -----------------------------------------

        private int _pointID;
        private bool _isUpper = false;
        private Game.PlayerColour _colour = Game.PlayerColour.NONE;
        private Game.PlayingAs _owner = Game.PlayingAs.NONE;
        private bool _isInteractive = false;
        private bool _isHighlighted = false;
        private bool _isSelected = false;

        private Stack<Counter> _countersOnPointStack;
        private int _activeCounters = 0;
        private static float _counterDepth = 0f;
        private static float _counterHeight = 0f;

        private Material _material;

        public int ID { get => _pointID; }
        public Game.PlayerColour Colour { get => _colour; }
        public Game.PlayingAs Owner { get => _owner; }
        public int Counters { get => _countersOnPointStack.Count; }
        public bool IsInteractive { get => _isInteractive; }
        public bool IsSelected { get => _isSelected; }
        public MeshRenderer Renderer { get => _pointMesh; }
    }
}
