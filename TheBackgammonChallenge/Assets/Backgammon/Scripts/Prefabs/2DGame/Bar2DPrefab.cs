using System.Collections.Generic;
using UnityEngine;

namespace Backgammon
{
    public class Bar2DPrefab : PlayablePosition2D
    {
        [Header("DEBUG")]
        [SerializeField] DebugPrefab debug_bar = null;

        [Header("COUNTER")]
        [SerializeField] Counter2DPrefab _counterPrefab;

        private void Awake()
        {
            var rect = gameObject.GetComponent<RectTransform>().rect;
            _counterHeight = rect.height;
            _counterWidth = rect.width;
        }

        // ------------------------------------------ COUNTER METHODS -------------------------------------------

        internal override void PushCounter(Counter2DPrefab counter)
        {
            counter.ParentID = 25;

            _countersOnBarStack.Push(counter);
            _activeCounters += 1;

            if (Counters > 15)
            {
                debug_bar.DebugMessage($"ERROR: NUMBER OF COUNTERS {Counters} AT {transform.name} IS INVALID!!!");
            }
        }

        internal override Counter2DPrefab PopCounter()
        {
            Counter2DPrefab counter = _countersOnBarStack.Pop();
            _activeCounters -= 1;

            if (counter is null)
            {
                debug_bar.DebugMessage($"ERROR: WOOPS!! - NO COUNTERS ON BAR...");
            }

            if (Counters < 0)
            {
                debug_bar.DebugMessage($"ERROR: NUMBER OF COUNTERS {Counters} ON BAR {transform.name} IS INVALID!!!");
            }

            return counter;
        }

        internal override Vector3 GetCounterOffsetPosition()
        {
            // NOTE: IF '_isUpper' NEED TO USE POSITIVE OFFSET

            var up = (_isUpper ? -1f : 1f);

            // DEFAULT TO '1' POSITION
            var offsetTransform = new Vector3(gameObject.transform.localPosition.x,
                                              gameObject.transform.localPosition.y + 0.5f * _counterWidth * up,
                                              gameObject.transform.localPosition.z);

            // NOTE: MUST BE ABLE TO STACK 15 COUNTERS ON BAR
            switch (_activeCounters)
            {
                case < 5: // FIRST ROW - 5
                    {
                        // OFFSET STARTS AT COUNTER '1' - IF == 1 RETURNS DEFAULT
                        offsetTransform += new Vector3(0f, (_activeCounters) * _counterWidth * up, 0f);
                    }
                    break;
                case < 9: // SECOND ROW - 4
                    {
                        // PLACED HALF IN SECOND ROW + '0' OFFSET - ALREADY 5 COUNTERS PLACED
                        offsetTransform += new Vector3(0f, ((_activeCounters - 4) - 0.5f) * _counterWidth * up, _counterHeight);
                    }
                    break;
                case < 12: // THIRD ROW - 3
                    {
                        // PLACED FULL IN THIRD ROW - ALREADY 9 COUNTERS PLACED
                        offsetTransform += new Vector3(0f, (_activeCounters - 8) * _counterWidth * up, 2 * _counterHeight);
                    }
                    break;
                case < 14: // FOURTH ROW - 2
                    {
                        // PLACED HALF IN FOURTH ROW + '1' OFFSET - ALREADY 12 COUNTERS PLACED
                        offsetTransform += new Vector3(0f, ((_activeCounters - 11) - 0.5f + 1) * _counterWidth * up, 3 * _counterHeight);
                    }
                    break; // TOP ROW - 1 - 15 TOTAL
                case 14:
                    {
                        // PLACED FULL IN TOP ROW + 1 OFFSET - ALREADY 14 COUNTERS PLACED
                        offsetTransform += new Vector3(0f, ((_activeCounters - 13) + 1) * _counterWidth * up, 4 * _counterHeight);
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

        internal void SetCounterAsSelected(bool selected)
        {
            _countersOnBarStack.Peek().SetAsSelected(selected);
        }

        internal void ResetCounters()
        {
            _countersOnBarStack = new Stack<Counter2DPrefab>();
        }

        internal override int GetID() { return (IsUpper ? -1 : 1); }

        // --------------------------------------- GETTERS && SETTERS ----------------------------------------

        private Stack<Counter2DPrefab> _countersOnBarStack;
        private int _activeCounters = 0;

        private bool _isUpper = false;
        private static float _counterHeight = 0f;
        private static float _counterWidth = 0f;

        public int Counters { get => _countersOnBarStack.Count; }
        public bool IsUpper { get => _isUpper; set => _isUpper = value; }
    }
}