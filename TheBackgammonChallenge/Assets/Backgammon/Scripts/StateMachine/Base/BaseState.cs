using System;

namespace Backgammon
{
    public abstract class BaseState<EState> where EState : Enum
    {
        public BaseState(EState key) 
        { 
            StateKey = key;
            ActiveState = StateKey;
        }

        public abstract void EnterState();
        public abstract void UpdateState();
        public abstract void ExitState();
        public abstract EState GetStateKey();
        public abstract EState GetActiveState();

        public EState StateKey { get; private set; }

        public EState ActiveState { get; set; }
    }
}