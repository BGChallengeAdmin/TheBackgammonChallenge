using UnityEngine;
using TMPro;
using System;

namespace Backgammon
{
    public class AnalysisAnimationPrefab : MonoBehaviour
    {
        [Header("TRANSFORM")]
        [SerializeField] Transform _transform;

        [Header("TEXT FIELDS")]
        [SerializeField] TMP_Text _rankText;

        internal event Action<AnalysisAnimationPrefab> OnAnimationCompleteAction;

        internal void Init(Vector3 defaultPosition, Vector3 targetPosition, float animationTime, float delayTime = 0f)
        {
            _defaultPosition = defaultPosition;
            _targetPosition = targetPosition;
            _animationTime = animationTime;
            _delayTime = delayTime;

            Reset();
        }

        private void Update()
        {
            if (_animate)
            {
                _delayTimer -= Time.deltaTime;

                if (_delayTimer <= 0)
                {
                    // HANDLE MOVEMENT
                    _animationTimer += Time.deltaTime;
                    var perc = (_animationTimer / _animationTime);
                    transform.position = Vector3.Lerp(_defaultPosition, _targetPosition, perc);

                    // CHECK FINAL POSITION
                    if (Vector3.Distance(transform.position, _targetPosition) < 0.001f)
                    {
                        _animate = false;
                        Reset();

                        if (OnAnimationCompleteAction is not null) OnAnimationCompleteAction(this);
                    }
                }
            }
        }

        internal void SetAndAnimateRankText(string rank)
        {
            _rankText.text = rank;
            gameObject.SetActive(true);

            _animate = true;
        }

        internal void SetToAnimate()
        {
            _animate = true;
        }

        internal void Reset()
        {
            gameObject.SetActive(false);
            gameObject.transform.position = _defaultPosition;
            gameObject.transform.localScale = Vector3.one;
            
            _animationTimer = 0;
            _delayTimer = _delayTime;
        }

        private bool _animate = false;
        private Vector3 _defaultPosition;
        private Vector3 _targetPosition;

        private float _animationTime = 2f;
        private float _animationTimer = 2f;
        private float _delayTime = 0f;
        private float _delayTimer = 0f;
    }
}