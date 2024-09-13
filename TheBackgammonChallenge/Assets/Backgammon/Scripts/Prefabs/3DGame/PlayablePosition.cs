using UnityEngine;

namespace Backgammon
{
    public abstract class PlayablePosition : MonoBehaviour
    {
        internal abstract void PushCounter(Counter counter);
        internal abstract Counter PopCounter();
        internal abstract Vector3 GetCounterOffsetPosition();
        internal abstract int GetID();
    }
}