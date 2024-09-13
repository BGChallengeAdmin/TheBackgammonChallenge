using System;
using System.Collections.Generic;
using UnityEngine;

namespace Backgammon
{
    public class Home2DPrefab : PlayablePosition2D
    {
        [Header("DEBUG")]
        [SerializeField] DebugPrefab debug_home;
        
        [Header("COUNTER")]
        [SerializeField] Counter2DPrefab _counterPrefab;

        [Header("MATERIALS")]
        [SerializeField] Material _homeHighlightMaterial;
        [SerializeField] Material _homeSelectedMaterial;

        internal event Action<Home2DPrefab> OnHomeClickedEvent;

        private void Awake()
        {
            // DEBUGGING - HOME ALWAYS SHOWS ERRORS
            debug_home.ShowMesssage = true;
        }

        internal void Init(float scaledCounterSize)
        {
            _counterHeight = scaledCounterSize;
        }

        public void OnClickHomeButton()
        {
            if (!_isInteractive) return;

            if (OnHomeClickedEvent != null) OnHomeClickedEvent(this);
            else Debug.Log($"{transform.name} HOME CLICK EVENT IS NULL");
        }

        public void OnMouseDown()
        {
            if (!_isInteractive) return;

            if (OnHomeClickedEvent != null) OnHomeClickedEvent(this);
            else Debug.Log($"{transform.name} HOME CLICK EVENT IS NULL");
        }

        // ------------------------------------------ COUNTER METHODS -------------------------------------------

        internal override void PushCounter(Counter2DPrefab counter)
        {
            counter.ParentID = 0;
            _countersOnHomeStack.Push(counter);
            _activeCounters += 1;

            if (Counters > 15)
            {
                debug_home.DebugMessage($"ERROR: NUMBER OF COUNTERS {Counters} AT {transform.name} IS INVALID!!!");
            }
        }

        internal override Counter2DPrefab PopCounter()
        {
            Counter2DPrefab counter = new Counter2DPrefab();

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
                                              gameObject.transform.localPosition.y + _counterHeight * (_isUpper ? -1 : 1),
                                              0f);

            var offsetTransform1 = new Vector3(0f, (_activeCounters) * 1.51f * _counterHeight * (_isUpper ? -1 : 1), 0f);

            offsetTransform += offsetTransform1;

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

        internal void SetInteractive(bool interactive, bool highlight = false, bool selected = false)
        {
            _isInteractive = interactive;

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
        }

        internal void ResetCounters()
        {
            _countersOnHomeStack = new Stack<Counter2DPrefab>();
            _activeCounters = 0;
        }

        internal override int GetID() { return 0; }

        // --------------------------------------- GETTERS && SETTERS ----------------------------------------

        private Stack<Counter2DPrefab> _countersOnHomeStack;
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