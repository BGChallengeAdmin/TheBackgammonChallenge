using System.Collections.Generic;
using System;
using UnityEngine;

namespace Backgammon
{
    public abstract class StateManager<EState> : MonoBehaviour where EState : Enum
    {
        protected void Update()
        {
            if (CurrentState is null) return;

            EState nextStateKey = CurrentState.GetActiveState();

            if (!_isTransitioningState && nextStateKey.Equals(CurrentState.StateKey))
            {
                CurrentState.UpdateState();
            }
            else
            {
                TransitionToNextState(nextStateKey);
            }
        }

        public void TransitionToNextState(EState stateKey)
        {
            if (Main.Instance.IfUsingDebugToolkit)
            {
                DebugPrefab.DebugMessageStatic($"");
                DebugPrefab.DebugMessageStatic($"BASE STATE CHANGE");
                DebugPrefab.DebugMessageStatic($"CURRENT STATE: {CurrentState.StateKey}");
                DebugPrefab.DebugMessageStatic($"NEXT STATE: {stateKey}");
                DebugPrefab.DebugMessageStatic($"");
            }

            _isTransitioningState = true;
            CurrentState.ExitState();
            CurrentState = States[stateKey];
            CurrentState.EnterState();
            _isTransitioningState = false;
        }

        internal void ResetStates()
        {
            States = new Dictionary<EState, BaseState<EState>>();
        }

        // --------------------------------------------- MEMBERS ---------------------------------------------

        protected Dictionary<EState, BaseState<EState>> States = new Dictionary<EState, BaseState<EState>>();
        public BaseState<EState> CurrentState { get; protected set; }

        protected bool _isTransitioningState = false;
    }
}