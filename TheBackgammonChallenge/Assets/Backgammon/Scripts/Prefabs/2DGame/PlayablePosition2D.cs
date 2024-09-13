using UnityEngine;

namespace Backgammon
{
    public abstract class PlayablePosition2D : MonoBehaviour
    {
        internal abstract void PushCounter(Counter2DPrefab counter);
        internal abstract Counter2DPrefab PopCounter();
        internal abstract Vector3 GetCounterOffsetPosition();
        internal abstract int GetID();
    }
}