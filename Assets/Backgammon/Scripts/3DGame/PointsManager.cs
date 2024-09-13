using System.Collections.Generic;
using UnityEngine;

namespace Backgammon
{
	public class PointsManager : MonoBehaviour
	{
		[Header("DEBUG")]
		[SerializeField] DebugPrefab debug_pointsManager = null;

		[Header("TRANSFORMS")]
		[SerializeField] Transform boardPointsLeftTransform;
		[SerializeField] Transform boardPointsRightTransform;
		[SerializeField] Transform pointsUpperContainer;
		[SerializeField] Transform pointsLowerContainer;

        // NOTE: POINTS ARE NUMBERED 24 (UPPER LEFT) 12 (LOWER RIGHT) IN TRANSFORM ARRAYS
        [Header("POINTS")]
		[SerializeField] Point pointPrefab = null;
		[SerializeField] Transform[] pointsTransformsUpper24 = null;
		[SerializeField] Transform[] pointsTransformsLower12 = null;
		private Point[] _points = null;

		[Header("MATERIALS")]
		[SerializeField] Material _defaultBlackMaterial = null;
		[SerializeField] Material _defaultWhiteMaterial = null;
		private List<MeshRenderer> _blackPointsRenderers;
		private List<MeshRenderer> _whitePointsRenderers;

		private void Awake()
		{
			_points = new Point[24];

			_blackPointsRenderers = new List<MeshRenderer>();
			_whitePointsRenderers = new List<MeshRenderer>();
		}

		// INIT
		internal void InitPointsFrom24(bool playingFromLeft = true)
		{
            // NOTE: POINTS ARE NUMBERED 24 (UPPER LEFT) 12 (LOWER RIGHT) IN TRANSFORM ARRAYS
			// THESE ARE TAKEN FROM THE BOARD AND ROTATION / TRANSLATION IS HANDLED BELOW

            ClearAllPointsPrefabs();

			var verticalOffset = new Vector3(0f, 0.001f, 0f);
			var rotate = Quaternion.Euler(0f, 180f, 0f);

			var counter = 24;
			var blackOrWhite = true;

			for (int pU = 0; pU < 12; pU++)
			{
				var p = playingFromLeft ? pU : 11 - pU;
				var side = p < 6 ? boardPointsLeftTransform : boardPointsRightTransform;

				// NOTE: IN BOARD SETUP - POINTS ARE CORRECT - BOARD RIGHT IS ROTATED 180 - ACCOUNT FOR TRANSFORM OFFSET
				var position = side.position + Vector3.Scale(pointsTransformsUpper24[p].localPosition + verticalOffset,
												p < 6 ? Vector3.one : new Vector3(-1f, 1f, -1f));

				// TURN OFF UNDERLYING TRANSFORM
				pointsTransformsUpper24[p].gameObject.SetActive(false);

                var point = Instantiate(pointPrefab, position, Quaternion.identity, pointsUpperContainer);
				point.transform.name = (blackOrWhite ? "BLACK" : "WHITE") + counter;

                point.Init(counter--, true, (blackOrWhite ? _defaultBlackMaterial : _defaultWhiteMaterial));
				_points[counter] = point;

				point.OnPointClickedEvent += OnPointClickedAction;

				if (blackOrWhite) _blackPointsRenderers.Add(point.Renderer);
				else _whitePointsRenderers.Add(point.Renderer);

				blackOrWhite = !blackOrWhite;
			}

			debug_pointsManager.DebugMessage($"*** UPPER POINTS CREATED ***");

            for (int pL = 0; pL < 12; pL++)
			{
				var p = playingFromLeft ? pL : 11 - pL;
				var side = p < 6 ? boardPointsRightTransform : boardPointsLeftTransform;

				// NOTE: IN BOARD SETUP - POINTS ARE CORRECT - BOARD RIGHT IS ROTATED 180 - ACCOUNT FOR TRANSFORM OFFSET
				var position = side.position + Vector3.Scale(pointsTransformsLower12[p].localPosition + verticalOffset,
												p < 6 ? new Vector3(-1f, 1f, -1f) : Vector3.one);

                // TURN OFF UNDERLYING TRANSFORM
                pointsTransformsLower12[p].gameObject.SetActive(false);

                var point = Instantiate(pointPrefab, position, rotate, pointsLowerContainer);
                point.transform.name = (blackOrWhite ? "BLACK" : "WHITE") + counter;

                point.Init(counter--, false, (blackOrWhite ? _defaultBlackMaterial : _defaultWhiteMaterial));
                _points[counter] = point;

                point.OnPointClickedEvent += OnPointClickedAction;

                if (blackOrWhite) _blackPointsRenderers.Add(point.Renderer);
				else _whitePointsRenderers.Add(point.Renderer);

				blackOrWhite = !blackOrWhite;
			}

            debug_pointsManager.DebugMessage($"*** LOWER POINTS CREATED ***");

            Game.Context.HomeManager.PlayerHome.OnHomeClickedEvent += OnPointClickedAction;
        }

		internal void SetCountersToStartingPoints(bool playersCounters, Counter[] counters)
		{
			var countersPlaced = 0;
			Point point = null;

			// ADD 2 @ POINT 24
			while (countersPlaced < 2)
			{
				if (playersCounters) point = GetPlayerPointByID(24);
				else point = GetOpponentPointByID(24);
				SetCounterMoveToPoint(counters[countersPlaced++], point);
			}

			// ADD 5 @ POINT 13
			while (countersPlaced < 7)
			{
				if (playersCounters) point = GetPlayerPointByID(13);
				else point = GetOpponentPointByID(13);
				SetCounterMoveToPoint(counters[countersPlaced++], point);
			}

			// ADD 3 @ POINT 8
			while (countersPlaced < 10)
			{
				if (playersCounters) point = GetPlayerPointByID(8);
				else point = GetOpponentPointByID(8);
				SetCounterMoveToPoint(counters[countersPlaced++], point);
			}

			// ADD 5 @ POINT 6
			while (countersPlaced < 15)
			{
				if (playersCounters) point = GetPlayerPointByID(6);
				else point = GetOpponentPointByID(6);
				SetCounterMoveToPoint(counters[countersPlaced++], point);
			}
		}

		private void SetCounterMoveToPoint(Counter counter, Point point)
		{
			// TODO: CONSIDER MOVE TYPE IN COUNTER - SETUP SPEED?

			counter.SetCounterToMoveToPosition(point.GetCounterOffsetPosition());
			point.PushCounter(counter);
		}

		private void OnDestroy()
		{
			if (_points is not null) 
				foreach (Point point in _points) 
				{ if (point is not null) point.OnPointClickedEvent -= OnPointClickedAction; }

            Game.Context.HomeManager.PlayerHome.OnHomeClickedEvent -= OnPointClickedAction;
        }

        // POINT INTERACTIONS
        internal void OnPointClickedAction(PlayablePosition position)
        {
			if (position.GetID() == _selectedPointFrom)
			{
				// SELECTED SAME POINT -> FROM
				SetPointFromCounterAsSelected(false);
				_selectedPointFrom = -1;
				_ifPointFromSelected = false;
			}
			else if (_selectedPointFrom > 0)
			{
				// TEST IF PLAYER IS SELECTING A VALID POINT OR CHANGING POINT FROM
				_ifChangingSelectedPointFrom = true;
				var pointID = position.GetID();

				foreach (int valid in _validPointsToArray)
				{
					if (pointID == valid)
					{
                        // SELECTED NEW POINT -> TO
                        SetPointFromCounterAsSelected(false);
                        _ifChangingSelectedPointFrom = false;
						_selectedPointTo = pointID;
						_ifPointToSelected = true;

						break;
					}
				}

				if (_ifChangingSelectedPointFrom)
				{
					_selectedPointFrom = pointID;
					SetPointFromCounterAsSelected(true);
				}
			}
			else
			{
				// SELECTED NEW POINT -> FROM
				_ifChangingSelectedPointFrom = false;
				_selectedPointFrom = position.GetID();
                SetPointFromCounterAsSelected(true);
                _ifPointFromSelected = true;
            }
        }

		internal bool SetPointsFromInteractive(Game.PlayerColour playerColour, Game.PlayerColour opponentColour, int dice1, int dice2, int diceAvailable)
		{
            // NOTE: THIS IS NOT TESTING AGAINST BEARING OFF - USE ALTERNATIVE METHOD BELOW

            Debug.Log($"DICE {dice1} {dice2}");

			if (dice1 == 0 && dice2 == 0) return false;

            // FROM POINT 1
            foreach (Point pointFrom in _points)
			{
				// TEST IF PLAYER POINT
				if (pointFrom.Colour != playerColour) continue;

				// NOTE: DOUBLES MOVES IS NOT REQUIRED - HANDLED BY 'MOVE TO'
                // TEST COMBINATIONS OF DICE / DOUBLE - IF VALID SET ACTIVE
     //           if (dice1 == dice2)
     //           {
     //               // DOUBLE ROLLED - UP TO 4 DICE CAN BE USED TO MOVE
     //               for (int d = 1; d <= diceAvailable; d++)
					//{
					//	var step = d * dice1;
					//	var pointToIndex = pointFrom.ID - step;

					//	if (pointToIndex <= 0) break;

					//	var pointTo = GetPlayerPointByID(pointToIndex);
					//	if (pointTo.Colour == opponentColour && pointTo.Counters > 1) break;

					//	pointFrom.SetInteractive(true, true);
					//}
     //           }
                //else
                {
					// DICE DIFFER
					var step1 = (pointFrom.ID - dice1);
					var step2 = (pointFrom.ID - dice2);
					var step3 = (pointFrom.ID - (dice1 + dice2));

                    var valid1 = TestIfCanMoveToPoint(step1, opponentColour);
					var valid2 = TestIfCanMoveToPoint(step2, opponentColour);
					var valid3 = TestIfCanMoveToPoint(step3, opponentColour);

                    if (dice1 != 0) pointFrom.SetInteractive(valid1, true);
                    if (!valid1 && dice2 != 0) pointFrom.SetInteractive(valid2, true);
                    if ((valid1 || valid2) && valid3) pointFrom.SetInteractive(valid3, true);
                }
			}

			return true;
		}

		internal void SetPointsToInteractive(Game.PlayerColour playerColour, Game.PlayerColour opponentColour, int dice1, int dice2, int diceAvailable)
        {
            // NOTE: THIS IS NOT TESTING AGAINST BEARING OFF - USE ALTERNATIVE METHOD BELOW
            _validPointsToArray = new int[4];

			// TEST DICE / DOUBLE COMBINATIONS FROM 'SELECTED POINT'
			if (dice1 == dice2)
			{
				// DOUBLE ROLLED - UP TO 4 DICE CAN BE USED TO MOVE
				for (int d = 1; d <= diceAvailable; d++)
				{
					var step = d * dice1;
					var pointToIndex = _selectedPointFrom - step;

					if (pointToIndex <= 0) break;

					var pointTo = GetPlayerPointByID(pointToIndex);
					if (pointTo.Colour == opponentColour && pointTo.Counters > 1) break;

					// CANNOT INVALIDATE OTHER DICE AS SAME VALUE
					pointTo.SetInteractive(true, true, true);
					_validPointsToArray[d - 1] = pointToIndex;
				}
			}
			else
			{
				// DICE DIFFER
				var step1 = (_selectedPointFrom - dice1);
				var step2 = (_selectedPointFrom - dice2);
				var step3 = (_selectedPointFrom - (dice1 + dice2));

				var valid1 = TestCanMoveDoesNotInvalidateOtherDice(step1, dice2, 24, 1, playerColour, opponentColour);
				var valid2 = TestCanMoveDoesNotInvalidateOtherDice(step2, dice1, 24, 1, playerColour, opponentColour);
				var valid3 = TestIfCanMoveToPoint(step3, opponentColour) &&
								TestDoubleMoveIsNotForcedByBlot(dice1, dice2, opponentColour);

				if (dice1 != 0)
				{
					GetPlayerPointByID(step1).SetInteractive(valid1, true, true);
					_validPointsToArray[0] = step1;
				}
				if (dice2 != 0)
				{
					GetPlayerPointByID(step2).SetInteractive(valid2, true, true);
					_validPointsToArray[1] = step2;
				}
				if ((valid1 || valid2) && valid3)
				{
					GetPlayerPointByID(step3).SetInteractive(valid3, true, true);
                    _validPointsToArray[2] = step3;
                }
            }

			// ENSURE PLAYER CAN DESELECT - CANNOT DESELECT BAR
			if (_selectedPointFrom < 25) GetPlayerPointByID(_selectedPointFrom).SetInteractive(true, true);
		}

		private bool TestCanMoveDoesNotInvalidateOtherDice(int playedMove, int diceToTest, int validateFrom, int validateTo, Game.PlayerColour playerColour, Game.PlayerColour opponentColour)
		{
			if (!TestIfCanMoveToPoint(playedMove, opponentColour)) return false;

			if (validateTo <= 0) validateTo = 1;

			for (int pointFrom = validateFrom; pointFrom >= validateTo; pointFrom--)
            {
                if (GetPlayerPointByID(pointFrom).Colour != playerColour) continue;

				// EITHER FINDS A VALID MOVE - OR BELOW VALUE OF DICE - CAN'T INVALIDATE
				if (TestIfCanMoveToPoint((pointFrom - diceToTest), opponentColour)) return true;
				else if (pointFrom <= diceToTest) return true;
			}

			return false;
		}

        private bool TestIfCanMoveToPoint(int pointToIndex, Game.PlayerColour opponentColour)
        {
            if (!Game.Context.IfBearingOff && pointToIndex <= 0) return false;

			if (pointToIndex > 0)
			{
				var pointTo = GetPlayerPointByID(pointToIndex);
				if (pointTo.Colour == opponentColour && pointTo.Counters > 1) return false;
			}

            return true;
        }

		private bool TestDoubleMoveIsNotForcedByBlot(int dice1, int dice2, Game.PlayerColour opponentColour)
		{
			// NOTE: MUST BE ABLE TO MOVE EITHER DICE TO COMPLETE THE DOUBLE MOVE
			if (TestIfCanMoveToPoint((_selectedPointFrom - dice1), opponentColour) &&
				TestIfCanMoveToPoint((_selectedPointFrom - dice2), opponentColour))
					return true;
			else if (TestIfCanMoveToPoint((_selectedPointFrom - dice1), opponentColour))
			{
				var pointTo = GetPlayerPointByID((_selectedPointFrom - dice1));
				if (pointTo.Colour == opponentColour && pointTo.Counters > 0) return false;
				else return true;
			}
            else if (TestIfCanMoveToPoint((_selectedPointFrom - dice2), opponentColour))
            {
                var pointTo = GetPlayerPointByID((_selectedPointFrom - dice2));
                if (pointTo.Colour == opponentColour && pointTo.Counters > 0) return false;
                else return true;
            }
            else return false;
		}

        internal void ResetPointInteractions()
		{
			_ifPointFromSelected = false;
			_ifPointToSelected = false;

			_selectedPointFrom = -1;
			_selectedPointTo = -1;
		}

        internal void DeselectAllPointsInteractive()
        {
            foreach (Point point in _points)
            {
                point.SetInteractive(false);
            }

			_ifChangingSelectedPointFrom = false;
        }

        // COUNTER ON BAR
        internal void PlayerCounterOnBar()
        {
            _selectedPointFrom = 25;
            _ifPointFromSelected = true;
        }

		// BEARING OFF
		internal bool TestIfBearingOff(Game.PlayerColour playerColour)
		{
			var ifBearingOff = true;

			for (int bearingOff = 24; bearingOff > 6; bearingOff--)
			{
				var point = GetPlayerPointByID(bearingOff);
				if (point.Colour == playerColour && point.Counters > 0)
				{
					ifBearingOff = false;
					break;
				}
			}

			_ifBearingOff = ifBearingOff;
			return ifBearingOff;
		}

		internal void SetBearingOffPointsFromInteractive(Game.PlayerColour playerColour, Game.PlayerColour opponentColour, int c_dice1, int c_dice2, int o_dice1, int o_dice2)
		{
			var forced1 = false;
			var forced2 = false;

			// NOTE: c_dice == CURRENT VALUE OF DICE - '0' - DICE HAS BEEN USED
			//		 o_dice == ORIGINAL / ACTUAL VALUE OF DICE ROLLS

            Debug.Log($"BEARING OFF");

			// NOTE: DOUBLE MOVES NOT REQUIRED - HANDLED IN 'POINT TO'

			for (int pointIndex = 6; pointIndex > 0; pointIndex--)
			{
				var pointFrom = GetPlayerPointByID(pointIndex);

				Debug.Log($"POINT {pointIndex}");

				if (pointFrom.Colour != playerColour) continue;

				// POINT INDEX FOR SMALLER DICE VALUE - FORCED MOVE AT HIGHER POINT
				if ((pointIndex < o_dice1) && (o_dice1 <= o_dice2) && forced2) forced1 = true;
				if ((pointIndex < o_dice2) && (o_dice2 < o_dice1) && forced1) forced2 = true;
				if (forced1 && forced2) break;

				var step1 = pointIndex - c_dice1;
				var step2 = pointIndex - c_dice2;

				var canMove1 = (c_dice1 != 0) && TestIfCanMoveToPoint(step1, opponentColour);
				var canMove2 = (c_dice2 != 0) && TestIfCanMoveToPoint(step2, opponentColour);

				var valid1 = canMove1 && TestCanMoveDoesNotInvalidateOtherDice(step1, c_dice2, 6, 1, playerColour, opponentColour);
				var valid2 = canMove2 && TestCanMoveDoesNotInvalidateOtherDice(step2, c_dice1, 6, 1, playerColour, opponentColour);

				if (c_dice1 != 0) pointFrom.SetInteractive(valid1, true);
				if (!valid1 && c_dice2 != 0) pointFrom.SetInteractive(valid2, true);

				Debug.Log($"VALID {valid1} {valid2}");

                #region DEBUG
                //if (Game.Context.IfBearingOff) Debug.Log($"");

                //if (Game.Context.IfBearingOff) Debug.Log($"INDEX: {pointIndex} -> " +
                //										 $"DICE1: {c_dice1} -> " +
                //										 $"STEP1: {step1} -> " +
                //										 $"VALID1: {(c_dice1 != 0 && valid1)}");

                //if (Game.Context.IfBearingOff) Debug.Log($"INDEX: {pointIndex} -> " +
                //										 $"DICE2: {c_dice2} -> " +
                //										 $"STEP2: {step2} -> " +
                //										 $"VALID2: {(!valid1 && c_dice2 != 0 && valid2)}");

                //if (Game.Context.IfBearingOff) Debug.Log($"FORCED1: {(pointIndex < o_dice1)} - {(o_dice1 < o_dice2)} - {forced2}");
                //if (Game.Context.IfBearingOff) Debug.Log($"FORCED2: {(pointIndex < o_dice2)} - {(o_dice2 < o_dice1)} - {forced1}");
                #endregion

                // POINT INDEX FOR LARGER DICE VALUE - FORCED MOVE
                //if (!forced1 && !forced2 || (pointIndex == o_dice1) || (pointIndex == o_dice2))
                if ((c_dice1 != 0 && c_dice2 != 0) || !(forced1 && forced2))
				{
					if ((o_dice1 > o_dice2) && pointIndex <= o_dice1) forced1 = true;
					if ((o_dice2 >= o_dice1) && pointIndex <= o_dice2) forced2 = true;
					Debug.Log($"FORCED LARGER: {forced1} {forced2}");
				}

				if (c_dice1 == 0 && (o_dice1 < o_dice2) && pointIndex <= o_dice2) { forced1 = true; }
				else if (c_dice2 == 0 && (o_dice2 < o_dice1) && pointIndex <= o_dice1) { forced2 = true; }

                Debug.Log($"FORCED END: {forced1} {forced2}");
            }
        }

		internal void SetBearingOffPointsToInteractive(Game.PlayerColour playerColour, Game.PlayerColour opponentColour, int dice1, int dice2, int diceAvailable)
		{
            _validPointsToArray = new int[4];

            // TEST DICE / DOUBLE COMBINATIONS FROM 'SELECTED POINT'
            if (dice1 == dice2)
            {
				// DOUBLE ROLLED - UP TO 4 DICE CAN BE USED TO MOVE
                for (int d = 1; d <= diceAvailable; d++)
                {
                    var step = d * dice1;
                    var pointToIndex = _selectedPointFrom - step;

					// CANNOT DOUBLE MOVE IF THERE IS ANOTHER PLAYABLE MOVE
					if (pointToIndex < 0)
						if (TestCanMoveDoesNotInvalidateOtherDice(pointToIndex, dice1, 6, pointToIndex, 
																		playerColour, opponentColour)) break;

					if (pointToIndex > 0)
					{
						var pointTo = GetPlayerPointByID(pointToIndex);
						if (pointTo.Colour == opponentColour && pointTo.Counters > 1) break;

						// CANNOT INVALIDATE OTHER DICE AS SAME VALUE
						pointTo.SetInteractive(true, true, true);
						_validPointsToArray[d - 1] = pointToIndex;
                    }
					else
					{
						Game.Context.HomeManager.PlayerHome.SetInteractive(true, true, true);
                        break;
					}

                    if (Game.Context.IfBearingOff) Debug.Log($"FROM: {_selectedPointFrom} -> " +
                                                             $"DICE1: {dice1} -> " +
                                                             $"STEP1: {step} -> ");
                }
            }
			else
			{
				// DICE DIFFER
				var step1 = (_selectedPointFrom - dice1);
				var step2 = (_selectedPointFrom - dice2);
				var step3 = (_selectedPointFrom - (dice1 + dice2));

				var canMove1 = TestCanMoveDoesNotInvalidateOtherDice(step1, dice2, 6, 1, playerColour, opponentColour);
				var canMove2 = TestCanMoveDoesNotInvalidateOtherDice(step2, dice1, 6, 1, playerColour, opponentColour);
				var canMove3 = TestIfCanMoveToPoint(step3, opponentColour) &&
								TestDoubleMoveIsNotForcedByBlot(dice1, dice2, opponentColour);
                var valid3Sanity = TestCanMoveDoesNotInvalidateOtherDice(step1, dice2, _selectedPointFrom - 1, 1, playerColour, opponentColour) &&
									TestCanMoveDoesNotInvalidateOtherDice(step2, dice1, _selectedPointFrom - 1, 1, playerColour, opponentColour);

                var valid1 = canMove1 && TestNoForecedMoveFromHigherPoint(dice1, playerColour);
                var valid2 = canMove2 && TestNoForecedMoveFromHigherPoint(dice2, playerColour);

               // if (Game.Context.IfBearingOff) Debug.Log($"");

               // if (Game.Context.IfBearingOff) Debug.Log($"FROM: {_selectedPointFrom} -> " +
														 //$"DICE1: {dice1} -> " +
														 //$"STEP1: {step1} -> " +
														 //$"VALID1: {valid1} " +
														 //$"MOVE1: {(valid1 && (dice1 != 0))}");

               // if (Game.Context.IfBearingOff) Debug.Log($"FROM: {_selectedPointFrom} -> " +
               //                                          $"DICE2: {dice2} -> " +
               //                                          $"STEP2: {step2} -> " +
               //                                          $"VALID2: {valid2} " +
               //                                          $"MOVE2: {(valid2 && (dice2 != 0))}");

                if (dice1 != 0)
				{
					if (step1 > 0)
					{
						GetPlayerPointByID(step1).SetInteractive(valid1, true, true);
						_validPointsToArray[0] = step1;
					}
					else Game.Context.HomeManager.PlayerHome.SetInteractive(valid1, true, true);
				}
				if (dice2 != 0)
				{
					if (step2 > 0)
					{
						GetPlayerPointByID(step2).SetInteractive(valid2, true, true);
						_validPointsToArray[1] = step2;
					}
					else Game.Context.HomeManager.PlayerHome.SetInteractive(valid2, true, true);
				}
				//if ((valid1 || valid2) && valid3)
				//{
					//if (step3 > 0) GetPlayerPointByID(step3).SetInteractive(valid3, true, true);
                    //else Game.Context.HomeManager.PlayerHome.SetInteractive(valid3, true, true);
                //}
			}

            // ENSURE PLAYER CAN DESELECT - CANNOT DESELECT BAR
            if (_selectedPointFrom < 25) GetPlayerPointByID(_selectedPointFrom).SetInteractive(true, true);
        }

		internal bool TestNoForecedMoveFromHigherPoint(int diceRoll, Game.PlayerColour playerColour)
		{
			for (int p = diceRoll; p > _selectedPointFrom; p--)
			{
				var point = GetPlayerPointByID(p);
				if (point.Colour != playerColour) continue;
				if (point.Counters > 0) return false;
			}

			return true;
		}

		internal void DeselectHomeInteractive()
		{
			Game.Context.HomeManager.PlayerHome.SetInteractive(false);
		}

        // BLOT
        internal bool IfBlot(int pointTo, Game.PlayerColour opponentColour)
        {
            var point = GetPlayerPointByID(pointTo);
            return (point.Colour == opponentColour && point.Counters == 1);
        }

		// COUNTER INTERACTION
		private void SetPointFromCounterAsSelected(bool selected)
		{
			if (_selectedPointFrom == 25) return;

			var point = GetPlayerPointByID(_selectedPointFrom);
			point.SetCounterAsSelected(selected);
		}

        // GET POINT
        internal Point GetPointByID(int id) { return (id > 0 ? _points[id - 1] : Instantiate(pointPrefab)); }

		internal Point GetPlayerPointByID(int id) { return GetPointByID(id); }

		internal Point GetOpponentPointByID(int id) { return GetPointByID(25 - id); }

        // ------------------------------------------- HELPER METHODS -------------------------------------------

        internal void SetPointsColours(bool blackOrWhite, Material material)
		{
			if (blackOrWhite)
			{
				foreach (MeshRenderer mesh in _blackPointsRenderers)
				{
					mesh.material = material;
				}
			}
			else
			{
				foreach (MeshRenderer mesh in _whitePointsRenderers)
				{
					mesh.material = material;
				}
			}
		}

		private void ClearAllPointsPrefabs()
		{
			_blackPointsRenderers.Clear();
			_whitePointsRenderers.Clear();

			foreach (Transform point in pointsUpperContainer)
			{
                Destroy(point.gameObject);
			}

			foreach (Transform point in pointsLowerContainer)
			{
				Destroy(point.gameObject);
			}
		}

		// ----------------------------------------- GETTERS && SETTERS -----------------------------------------

		private bool _ifPointFromSelected = false;
		private bool _ifPointToSelected = false;
		private bool _ifChangingSelectedPointFrom = false;
		private int[] _validPointsToArray = new int[4];

		private int _selectedPointFrom = -1;
		private int _selectedPointTo = -1;
        
		private bool _ifBearingOff = false;

		public Point[] Points { get => _points; }
		public bool IfPointFromSelected { get => _ifPointFromSelected; }
        public bool IfPointToSelected { get => _ifPointToSelected; }
		public bool IfChangingSelectedPointFrom { get => _ifChangingSelectedPointFrom; }
        public int SelectedPointFrom { get => _selectedPointFrom; }
        public int SelectedPointTo { get => _selectedPointTo; }
	}
}