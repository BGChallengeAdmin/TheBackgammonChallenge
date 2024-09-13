using UnityEngine;

namespace Backgammon
{
    public class Doubling : MonoBehaviour
    {
        [Header("TRANSFORMS")]
        [SerializeField] Transform _upperPosition = null;
        [SerializeField] Transform _midPosition = null;
        [SerializeField] Transform _lowerPosition = null;

        [Header("CUBE")]
        [SerializeField] Transform _doublingCube = null;

        internal void Init()
        {
            _targetPosition = _midPosition.position;
            _isMoving = true;
        }

        private void Update()
        {
            // HANDLE MOVEMENT
            if (_isMoving)
            {
                var step = _movementSpeed * Time.deltaTime;
                _doublingCube.position = Vector3.MoveTowards(_doublingCube.position, _targetPosition, step);

                // CHECK FINAL POSITION
                if (Vector3.Distance(_doublingCube.position, _targetPosition) < 0.001f)
                {
                    _isMoving = false;
                }
            }
        }

        private void OnMouseDown()
        {
            if (!_isInteractive) return;

            _doublingWasClicked = true;
        }

        internal void SetActive(bool active)
        {
            _isInteractive = active;
            ResetClicked();
        }

        internal void SetCubeToMove(bool upper)
        {
            if (upper) _targetPosition = _upperPosition.position;
            else _targetPosition = _lowerPosition.position;

            _isMoving = true;
        }

        internal float GetCubeHeight()
        {
            var height = _doublingCube.GetComponentsInChildren<MeshFilter>()[0].sharedMesh.bounds.size.y;
            var scale = _doublingCube.transform.localScale.y;

            return height * scale;
        }

        internal float GetCubeHeight2D()
        {
            return gameObject.GetComponent<RectTransform>().rect.height;
        }

        internal void ResetClicked()
        {
            _doublingWasClicked = false;
        }

        private Vector3 _targetPosition;
        private float _movementSpeed = 0.05f;
        private bool _isMoving = false;
        private bool _isInteractive = false;
        private bool _doublingWasClicked = false;

        internal bool DoublingWasClicked { get => _doublingWasClicked; }
    }
}