using System.Collections;
using UnityEngine;

namespace Backgammon
{
	public class Counter : MonoBehaviour
	{
        internal void InitCounter(int id, Game.PlayerColour colour, Game.PlayingAs owner)
        {
            this._id = id;
            this._owner = owner;
            this._colour = colour;

            var mesh = this.GetComponentsInChildren<MeshFilter>()[0].sharedMesh;
            _counterDepth = mesh.bounds.size.z;
        }

        protected void FixedUpdate()
        {
            if (_isMoving || _isRotating)
            {
                // HANDLE ROTATION
                if (_isRotating)
                {
                    transform.rotation = Quaternion.Lerp(transform.rotation, _targetRotation, _roatationSpeed * 0.05f);
                }

                // CHECK FINAL ROTATION
                if (Quaternion.Dot(transform.rotation, _targetRotation) > 0.999995f)
                {
                    if (_isRotating)
                    {
                        if (_isRotatingToFlat)
                        {
                            transform.rotation = Quaternion.identity;
                            _isRotatingToFlat = false;
                        }
                        else if (_isRotatingToHome)
                        {
                            transform.rotation = Quaternion.Euler(_targetRotation.eulerAngles.x, 0f, 0f);
                            _isRotatingToHome = false;

                        }
                        else if (_isRotatingToEdge)
                        {
                            transform.rotation = Quaternion.Euler(_targetRotation.eulerAngles.x, 0f, 0f);
                            _isRotatingToEdge = false;
                        }

                        _isRotating = false;
                    }
                    else transform.rotation = Quaternion.identity;

                    _targetRotation = Quaternion.identity;
                }

                // HANDLE MOVEMENT
                if (_isMoving)
                {
                    var step = _movementSpeed * Time.fixedDeltaTime;
                    transform.position = Vector3.MoveTowards(transform.position, _targetPosition, step);
                }

                // MOVE ABOVE TARGET
                if (_isMovingOver)
                {
                    var quickStep = _movementOverSpeed * Time.fixedDeltaTime;
                    var abovePosition = new Vector3(transform.position.x, _counterDepth, transform.position.z);
                    transform.position = Vector3.MoveTowards(transform.position, abovePosition, quickStep);
                }

                // CHECK ABOVE FINAL POSITION
                if (Vector3.Distance(transform.position, _aboveTargetPosition) < 0.001f)
                {
                    _isMovingOver = false;
                }

                // CHECK FINAL POSITION
                if (Vector3.Distance(transform.position, _targetPosition) < 0.001f)
                {
                    _isMoving = false;
                }
            }
        }

        // ----------------------------------------- HELPER METHODS ------------------------------------------

        internal void SetCounterColour(Material material)
        {
            gameObject.GetComponentInChildren<Renderer>().material = material;
        }

        internal void SetCounterToMoveInstantaneous(Vector3 targetPosition)
        {
            // ASSUME COUNTER IS NOT FLAT
            _targetRotation = Quaternion.identity;
            _isRotating = true;
            _isRotatingToFlat = true;

            this.transform.position = targetPosition;
        }

        internal void SetCounterToMoveToPosition(Vector3 targetPosition, bool ifAsBlot = false)
        {
            // ASSUME COUNTER IS NOT FLAT
            _targetRotation = Quaternion.identity;
            _isRotating = true;
            _isRotatingToFlat = true;

            _targetPosition = targetPosition;
            _aboveTargetPosition = new Vector3(_targetPosition.x, _counterDepth, _targetPosition.z);

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
            _targetRotation = Quaternion.identity;
            _isRotating = true;
            _isRotatingToFlat = true;

            _targetPosition = targetPosition;
            _aboveTargetPosition = new Vector3(_targetPosition.x, _counterDepth, _targetPosition.z);

            // INSTANT MOTION - ALLOW UPDATE TO HANDLE FINE TUNING
            transform.position = _targetPosition;

            _isMoving = true;
            _isMovingOver = true;
        }

        private IEnumerator DelayBlotAnimation()
        {
            // DELAY COUNTER MOVE TO BAR UNTIL
            if (_blotCounter is not null)
            {
                while ((Vector3.Distance(transform.position, _blotCounter.transform.position)) > 
                                                 _blotCounterOffsetBeforeMove * _counterDepth)
                    yield return null;
            }
            else yield return new WaitForSeconds(0.5f);
            
            _isMoving = true;
            _isMovingOver = true;
        }

        internal void SetCounterToMoveToBar(Vector3 targetPosition, Counter? counter = null)
        {
            _blotCounter = counter;
            SetCounterToMoveToPosition(targetPosition, true);
        }

        internal void SetCounterToMoveToHome(Vector3 targetPosition)
        {
            SetCounterToMoveToPosition(targetPosition);

            // PREVENT COUNTER FROM ROTATING TO FLAT
            _isRotatingToFlat = false;

            // NOTE: IF TARGET IS IN UPPER z > 0, MUST BE ROTATED IN REVERSE DIRECTION
            _targetRotation = Quaternion.Euler(45f * (targetPosition.z > 0 ? -1 : 1), 0f, 0f);
            _isRotating = true;
            _isRotatingToHome = true;
        }

        internal void SetCounterToMoveToEdge()
        {
            var pos = transform.position;
            SetCounterToMoveToPosition(new Vector3(pos.x, pos.y + .5f * _counterDepth, pos.z));

            // PREVENT COUNTER FROM ROTATING TO FLAT
            _isRotatingToFlat = false;

            _targetRotation = Quaternion.Euler(90f, 0f, 0f);
            _isRotating = true;
            _isRotatingToEdge = true;
        }

        internal void SetAsSelected(bool selected = false)
        {
            if (selected) this.transform.localScale = new Vector3(1.2f, 1f, 1.2f);
            else this.transform.localScale = Vector3.one;
        }

        // --------------------------------------- GETTERS && SETTERS ----------------------------------------

        private int _id = 0;
        private Game.PlayingAs _owner = Game.PlayingAs.NONE;
        private Game.PlayerColour _colour = Game.PlayerColour.NONE;
        // NOTE: 25 - BAR / 0 - HOME
        private int _parent = 0;
        private static float _counterDepth;

        private Vector3 _targetPosition;
        private Vector3 _aboveTargetPosition;
        private float _movementSpeed = 0.35f;
        private float _movementOverSpeed = 1f;
        private bool _isMoving = false;
        private bool _isMovingOver = false;

        private Counter _blotCounter = null;
        private float _blotCounterOffsetBeforeMove = 2.0f;

        private Quaternion _targetRotation;
        private float _roatationSpeed = 3f;
        private bool _isRotating = false;
        private bool _isRotatingToFlat = false;
        private bool _isRotatingToHome = false;
        private bool _isRotatingToEdge = false;

        public int ID { get => _id; }
        public Game.PlayingAs Owner { get => _owner; }
        public Game.PlayerColour Colour { get => _colour; }
        public int ParentID { get => _parent; set => _parent = value; }
        public bool IsMoving { get => _isMoving; }

        // --------------------------------------------- MEMBERS ---------------------------------------------
    }
}