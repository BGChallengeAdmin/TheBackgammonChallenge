using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Backgammon
{
    public class Counter2DPrefab : MonoBehaviour
    {
        [Header("IMAGES")]
        [SerializeField] Image _counterImage;
        [SerializeField] Image _highlightImage;

        internal void InitCounter(int id, Game2D.PlayerColour colour, Game2D.PlayingAs owner,
                                  Transform movingContainerTransform, Transform containerTransform)
        {
            this.transform.name = colour.ToString() + id;

            this._id = id;
            this._owner = owner;
            this._colour = colour;

            this._movingContainerTransform = movingContainerTransform;
            this._containerTransform = containerTransform;

            _counterDepth = 1;

            SetAsSelected(false);
        }

        private void FixedUpdate()
        {
            if (_isMoving || _isRotating)
            {
                //// HANDLE ROTATION
                //if (_isRotating)
                //{
                //    transform.rotation = Quaternion.Lerp(transform.rotation, _targetRotation, _roatationSpeed * 0.05f);
                //}

                //// CHECK FINAL ROTATION
                //if (Quaternion.Dot(transform.rotation, _targetRotation) > 0.999995f)
                //{
                //    if (_isRotating)
                //    {
                //        if (_isRotatingToFlat)
                //        {
                //            transform.rotation = Quaternion.identity;
                //            _isRotatingToFlat = false;
                //        }
                //        else if (_isRotatingToHome)
                //        {
                //            transform.rotation = Quaternion.Euler(_targetRotation.eulerAngles.x, 0f, 0f);
                //            _isRotatingToHome = false;

                //        }
                //        else if (_isRotatingToEdge)
                //        {
                //            transform.rotation = Quaternion.Euler(_targetRotation.eulerAngles.x, 0f, 0f);
                //            _isRotatingToEdge = false;
                //        }

                //        _isRotating = false;
                //    }
                //    else transform.rotation = Quaternion.identity;

                //    _targetRotation = Quaternion.identity;
                //}

                // HANDLE MOVEMENT
                if (_isMoving)
                {
                    var step = _movementSpeed * Time.fixedDeltaTime;
                    transform.localPosition = Vector3.MoveTowards(transform.localPosition, _targetPosition, step);
                }

                // MOVE ABOVE TARGET
                if (_isMovingOver)
                {
                    var quickStep = _movementOverSpeed * Time.fixedDeltaTime;
                    //var abovePosition = new Vector3(transform.localPosition.x, _targetPosition.y, _aboveTargetPosition.z);
                    transform.localPosition = Vector3.MoveTowards(transform.localPosition, _aboveTargetPosition, quickStep);
                }

                // CHECK ABOVE FINAL POSITION
                if (Vector3.Distance(transform.localPosition, _aboveTargetPosition) < 0.1f)
                {
                    _isMovingOver = false;
                }

                // CHECK FINAL POSITION
                if (Vector3.Distance(transform.localPosition, _targetPosition) < 0.1f)
                {
                    // AFTER ANIMATION MOVE TO CONTAINER
                    //this.transform.parent = _containerTransform;
                    this.transform.SetParent(_containerTransform, false);

                    _isMoving = false;
                }
            }
        }

        // ----------------------------------------- HELPER METHODS ------------------------------------------

        internal void SetCounterColour(Material material)
        {
            _counterImage.material = material;
        }

        internal void SetSourceImage(Sprite sprite)
        {
            _counterImage.sprite = sprite;
        }

        internal void SetCounterToMoveInstantaneous(Vector3 targetPosition)
        {
            // ASSUME COUNTER IS NOT FLAT
            _targetRotation = Quaternion.identity;
            _isRotating = true;
            _isRotatingToFlat = true;

            this.transform.localPosition = targetPosition;
            //this.transform.parent = _containerTransform;
            this.transform.SetParent(_containerTransform, false);
        }

        internal void SetCounterToMoveToPosition(Vector3 targetPosition, bool ifAsPlayer, bool ifAsBlot = false)
        {
            // ASSUME COUNTER IS NOT FLAT
            //_targetRotation = Quaternion.identity;
            //_isRotating = true;
            //_isRotatingToFlat = true;

            // MOVE TO PARENT TRANSFORM TO ANIMATE ABOVE OTHER OBJECTS
            //this.transform.parent = _parentTransform;
            this.transform.SetParent(_movingContainerTransform, false);

            _targetPosition = targetPosition;
            _aboveTargetPosition = new Vector3(_targetPosition.x, _targetPosition.y, _counterDepth);

            _movementSpeed = ifAsPlayer ? _playerMovementSpeed : _opponentMovementSpeed;

            if (!ifAsBlot)
            {
                _isMoving = true;
                _isMovingOver = true;
            }
            else StartCoroutine(DelayBlotAnimation());
        }

        internal void SetCounterToMoveToPositionInstant(Vector3 targetPosition, bool ifAsBlot = false)
        {
            // ASSUME COUNTER IS NOT FLAT
            //_targetRotation = Quaternion.identity;
            //_isRotating = true;
            //_isRotatingToFlat = true;

            this.transform.SetParent(_movingContainerTransform, false);

            _targetPosition = targetPosition;
            _aboveTargetPosition = new Vector3(_targetPosition.x, _targetPosition.y, -_counterDepth);

            // INSTANT MOTION - ALLOW UPDATE TO HANDLE FINE TUNING
            transform.localPosition = _targetPosition;

            _isMoving = true;
            _isMovingOver = true;
        }

        private IEnumerator DelayBlotAnimation()
        {
            // DELAY COUNTER MOVE TO BAR UNTIL
            if (_blotCounter is not null)
            {
                while ((Vector3.Distance(transform.localPosition, _blotCounter.transform.localPosition)) >
                                                 _blotCounterOffsetBeforeMove * _counterDepth)
                    yield return null;
            }
            else yield return new WaitForSeconds(0.5f);

            _isMoving = true;
            _isMovingOver = true;
        }

        internal void SetCounterToMoveToBar(Vector3 targetPosition, Counter2DPrefab? counter = null)
        {
            _blotCounter = counter;
            SetCounterToMoveToPosition(targetPosition, true);
        }

        internal void SetCounterToMoveToBarInstant(Vector3 targetPosition, Counter2DPrefab? counter = null)
        {
            _blotCounter = counter;
            SetCounterToMoveToPositionInstant(targetPosition, true);
        }

        internal void SetCounterToMoveToHome(Vector3 targetPosition)
        {
            SetCounterToMoveToPosition(targetPosition, true);

            //// PREVENT COUNTER FROM ROTATING TO FLAT
            //_isRotatingToFlat = false;

            //// NOTE: IF TARGET IS IN UPPER z > 0, MUST BE ROTATED IN REVERSE DIRECTION
            //_targetRotation = Quaternion.Euler(45f * (targetPosition.z > 0 ? -1 : 1), 0f, 0f);
            //_isRotating = true;
            //_isRotatingToHome = true;
        }

        internal void SetCounterToMoveToHomeInstant(Vector3 targetPosition)
        {
            SetCounterToMoveToPositionInstant(targetPosition, true);
        }

        internal void SetCounterToMoveToEdge()
        {
            var pos = transform.localPosition;
            SetCounterToMoveToPosition(new Vector3(pos.x, pos.y + .5f * _counterDepth, pos.z), true);

            // PREVENT COUNTER FROM ROTATING TO FLAT
            _isRotatingToFlat = false;

            _targetRotation = Quaternion.Euler(90f, 0f, 0f);
            _isRotating = true;
            _isRotatingToEdge = true;
        }

        internal void SetAsSelected(bool selected = false)
        {
            if (selected)
            {
                this.transform.localScale = new Vector3(1.2f, 1.2f, 1f);
                _highlightImage.gameObject.SetActive(true);
            }
            else
            {
                this.transform.localScale = Vector3.one;
                _highlightImage.gameObject.SetActive(false);
            }
        }

        // --------------------------------------- GETTERS && SETTERS ----------------------------------------

        private Transform _movingContainerTransform;
        private Transform _containerTransform;

        private int _id = 0;
        private Game2D.PlayingAs _owner = Game2D.PlayingAs.NONE;
        private Game2D.PlayerColour _colour = Game2D.PlayerColour.NONE;
        // NOTE: 25 - BAR / 0 - HOME
        private int _parent = 0;
        private static float _counterDepth;

        private Vector3 _targetPosition;
        private Vector3 _aboveTargetPosition;
        private float _movementSpeed = 1000f;
        private float _playerMovementSpeed = 1000f;
        private float _opponentMovementSpeed = 750f;
        private float _movementOverSpeed = 5.0f;
        private bool _isMoving = false;
        private bool _isMovingOver = false;

        private Counter2DPrefab _blotCounter = null;
        private float _blotCounterOffsetBeforeMove = 2.0f;

        private Quaternion _targetRotation;
        private float _roatationSpeed = 3f;
        private bool _isRotating = false;
        private bool _isRotatingToFlat = false;
        private bool _isRotatingToHome = false;
        private bool _isRotatingToEdge = false;

        public int ID { get => _id; }
        public Game2D.PlayingAs Owner { get => _owner; }
        public Game2D.PlayerColour Colour { get => _colour; }
        public int ParentID { get => _parent; set => _parent = value; }
        public bool IsMoving { get => _isMoving; }

        // --------------------------------------------- MEMBERS ---------------------------------------------
    }
}