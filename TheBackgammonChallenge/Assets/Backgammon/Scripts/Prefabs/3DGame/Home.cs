using System;
using System.Collections.Generic;
using UnityEngine;

namespace Backgammon
{
    public class Home : PlayablePosition
    {
        [Header("DEBUG")]
        [SerializeField] DebugPrefab debug_home;
        [SerializeField] Transform debug_Sphere;

        [Header("COUNTER")]
        [SerializeField] Counter _counterPrefab;

        [Header("MATERIALS")]
        [SerializeField] Material _homeHighlightMaterial;
        [SerializeField] Material _homeSelectedMaterial;

        internal event Action<Home> OnHomeClickedEvent;

        private void Awake()
        {
            Mesh mesh = _counterPrefab.GetComponentsInChildren<MeshFilter>()[0].sharedMesh;
            _counterHeight = mesh.bounds.size.y;

            // DEBUGGING - HOME ALWAYS SHOWS ERRORS
            debug_home.ShowMesssage = true;
        }

        private void OnDestroy()
        {
            debug_home.ShowMesssage = false;
        }

        public void OnMouseDown()
        {
            if (!_isInteractive) return;

            Debug.Log($"{transform.name} WAS CLICKED");

            if (OnHomeClickedEvent != null) OnHomeClickedEvent(this);
        }

        // ------------------------------------------ COUNTER METHODS -------------------------------------------

        internal override void PushCounter(Counter counter)
        {
            counter.ParentID = 0;
            _countersOnHomeStack.Push(counter);
            _activeCounters += 1;

            if (Counters > 15)
            {
                debug_home.DebugMessage($"ERROR: NUMBER OF COUNTERS {Counters} AT {transform.name} IS INVALID!!!");
            }
        }

        internal override Counter PopCounter()
        {
            Counter counter = new Counter();

            if (_countersOnHomeStack is null)
            {
                debug_home.DebugMessage($"ERROR: WOOPS!! - NO COUNTERS ON HOME {transform.name}...");
            }
            else
            {
                counter = _countersOnHomeStack.Pop();
                _activeCounters -= 1;

                if (Counters < 0)
                {
                    debug_home.DebugMessage($"ERROR: NUMBER OF COUNTERS {Counters} ON HOME {transform.name} IS INVALID!!!");
                }
            }

            return counter;
        }

        internal override Vector3 GetCounterOffsetPosition()
        {
            // DEFAULT TO '1' POSITION
            var offsetTransform = new Vector3(gameObject.transform.localPosition.x,
                                              gameObject.transform.localPosition.y,
                                              gameObject.transform.localPosition.z + .85f * _counterHeight * (_isUpper ? -1 : 1));

            offsetTransform += new Vector3(0f, 0f, (_activeCounters) * 1.5f * _counterHeight * (_isUpper ? -1 : 1));

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

        internal void SetInteractive(bool interactive, bool highlight = false, bool selected= false)
        {
            _isInteractive = interactive;

            if (_isInteractive) Debug.Log($"{transform.name} IS INTERACTIVE");

            SetIsHighlighted(highlight, selected);

            if (!_isInteractive)
            {
                _isHighlighted = false;
                SetIsHighlighted();
            }
        }
        internal void SetIsHighlighted(bool isHighlighted = false, bool selected = false)
        {
            _isHighlighted = isHighlighted;
            
            debug_Sphere.gameObject.SetActive(_isInteractive);

            if (selected) debug_Sphere.GetComponentInChildren<Renderer>().material = _homeSelectedMaterial;
            else if (isHighlighted) debug_Sphere.GetComponentInChildren<Renderer>().material = _homeHighlightMaterial;
        }

        internal void ResetCounters()
        {
            _countersOnHomeStack = new Stack<Counter>();
        }

        internal override int GetID() { return 0; }

        // --------------------------------------- GETTERS && SETTERS ----------------------------------------

        private Stack<Counter> _countersOnHomeStack;
        private int _activeCounters = 0;

        private bool _isUpper = false;
        private static float _counterHeight = 0f;
        private bool _isInteractive = false;
        private bool _isHighlighted = false;

        public int Counters { get => _countersOnHomeStack.Count; }
        public bool IsUpper { get => _isUpper; set => _isUpper = value; }
        public bool IsInteractive { get => _isInteractive; }
    }
}