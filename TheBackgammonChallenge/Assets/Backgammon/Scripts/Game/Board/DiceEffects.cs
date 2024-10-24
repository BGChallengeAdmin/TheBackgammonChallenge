using UnityEngine;

namespace Backgammon
{
    public class DiceEffects : MonoBehaviour
    {
        [SerializeField] private GameObject blankFace = null;

        private Rigidbody rb;

        private void Awake()
        {
            rb = this.gameObject.GetComponent<Rigidbody>();
        }

        internal void SetActive(bool active)
        {
            this.gameObject.SetActive(active);
        }

        internal void SetScale(float setScale)
        {
            var setScaleX = this.transform.localScale.x;
            animationScale = new Vector3(setScaleX * 0.6f, setScaleX * 0.6f, setScaleX * 0.6f);

            this.transform.localScale = new Vector3(setScale, setScale, setScale);
            blankFace.gameObject.transform.localScale = new Vector3(setScale, setScale, setScale);
            
            _setScale = this.transform.localScale;
        }

        public void SetDiceFaceBlank(bool _setBlank)
        {
            blankFace.gameObject.SetActive(_setBlank);
        }

        public void SetPosition(Vector3 _position)
        {
            rb.position = _position;
        }

        public void AddTorque(Vector3 torqueVec)
        {
            rb.AddTorque(torqueVec);
        }

        private Vector3 _setScale;
        private Vector3 animationScale;
        private float animationRatio;
        private float animationDirection = 1f;

        public void AnimateScale(float speedVec)
        {
            animationRatio += speedVec * animationDirection;

            transform.localScale = Vector3.Lerp(_setScale, animationScale, animationRatio);

            if (animationRatio <= 0)
            {
                animationRatio = 0;
                animationDirection = 1f;
            }

            if (animationRatio >= 1)
            {
                animationRatio = 1;
                animationDirection = -1f;
            }
        }

        public void ResetDice()
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            transform.position = gameObject.GetComponentInParent<RectTransform>().transform.position;
            transform.rotation = Quaternion.identity;
            transform.localScale = _setScale;

            animationRatio = 0;
            animationDirection = 1f;

            SetDiceFaceBlank(true);
        }
    }
}