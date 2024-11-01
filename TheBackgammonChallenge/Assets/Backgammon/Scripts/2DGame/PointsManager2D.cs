using System.Collections.Generic;
using UnityEngine;

namespace Backgammon
{
	public class PointsManager2D : MonoBehaviour
	{
        [Header("POINTS")]
        [SerializeField] Point2DPrefab _point2DPrefab;
		[SerializeField] RectTransform _pointsLeftContainer;
		[SerializeField] RectTransform _pointsRightContainer;

        [Header("EDGES")]
        [SerializeField] RectTransform _edgeUpperTransform;
        [SerializeField] RectTransform _edgeLowerTransform;
        [SerializeField] RectTransform _edgeLeftTransform;
        [SerializeField] RectTransform _edgeRightTransform;
        [SerializeField] RectTransform _edgeBarTransform;

        [Header("MATERIALS")]
        [SerializeField] Material _defaultBlackMaterial = null;
        [SerializeField] Material _defaultWhiteMaterial = null;
        private List<MeshRenderer> _blackPointsRenderers;
        private List<MeshRenderer> _whitePointsRenderers;

        [Header("TEXTURES")]
        [SerializeField] Texture _defaultBlackTexture = null;
        [SerializeField] Texture _defaultWhiteTexture = null;

        [Header("COLOURS")]
        [SerializeField] Color _defaultBlackColour = Color.black;
        [SerializeField] Color _defaultWhiteColour = Color.white;

        private void Awake()
        {
            _points = new Point2DPrefab[24];

            _blackPointsRenderers = new List<MeshRenderer>();
            _whitePointsRenderers = new List<MeshRenderer>();

            _pointContainerWidth = _pointsLeftContainer.rect.width;
            _pointContainerHeight = _pointsLeftContainer.rect.height;
        }

        internal void ConfigurePointsDimsToScreenSize()
        {
            var upperLeftX = (.5f * Screen.width) + (.5f * _edgeBarTransform.rect.width);
            _pointsLeftContainer.offsetMax = new Vector2(-upperLeftX, -_edgeUpperTransform.rect.height);
            _pointsLeftContainer.offsetMin = new Vector2(_edgeLeftTransform.rect.width, _edgeLowerTransform.rect.height);

            var lowerRightX = (.5f * Screen.width) + (.5f * _edgeBarTransform.rect.width);
            _pointsRightContainer.offsetMin = new Vector2(lowerRightX, _edgeLowerTransform.rect.height);
            _pointsRightContainer.offsetMax = new Vector2(-_edgeRightTransform.rect.width, -_edgeUpperTransform.rect.height);

            _pointContainerWidth = _pointsLeftContainer.rect.width;
            _pointContainerHeight = _pointsLeftContainer.rect.height;
        }

        // INIT
        internal void InitPointsFrom24(bool playingFromLeft = true)
        {
            // NOTE: POINTS ARE NUMBERED 24 (UPPER LEFT) 12 (LOWER RIGHT)
            // EACH LOOP ADDS POINTS BY THEIR NUMBER

            var rotate = Quaternion.Euler(0f, 0f, 180f);

            var counter = 24;
            var blackOrWhite = true;

            var _posXStep = Mathf.Floor(_pointContainerWidth / 6);
            var _posYStep = Mathf.Floor(_pointContainerHeight / 2.5f);

            // SET UPPER POINTS
            for (int pU = 0; pU < 12; pU++)
            {
                var p = playingFromLeft ? pU : 11 - pU;
                var side = p < 6 ? _pointsLeftContainer : _pointsRightContainer;

                var sub = pU >= 6 ? 6 : 0;
                var stepX = _posXStep * ((pU < 3 || (pU >= 6 && pU < 9)) ? (-.5f - (2 - pU + sub)) : (.5f + (pU - 3 - sub)));
                var position = new Vector3(stepX * (playingFromLeft ? 1 : -1), (.5f * _pointContainerHeight), 0);

                var point = Instantiate(_point2DPrefab, position, rotate, side.transform);

                point.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(_posXStep, _posYStep);
                //point.gameObject.GetComponent<BoxCollider2D>().size = new Vector2(_posXStep, _posYStep);
                point.transform.localPosition = position;
                point.transform.name = (blackOrWhite ? "BLACK" : "WHITE") + counter;

                point.Init(counter--, true, (blackOrWhite ? _defaultBlackMaterial : _defaultWhiteMaterial),
                                            (blackOrWhite ? _defaultBlackTexture : _defaultWhiteTexture),
                                            (blackOrWhite ? _defaultBlackColour : _defaultWhiteColour));
                _points[counter] = point;

                point.OnPointClickedEvent += OnPointClickedAction;

                if (blackOrWhite) _blackPointsRenderers.Add(point.Renderer);
                else _whitePointsRenderers.Add(point.Renderer);

                blackOrWhite = !blackOrWhite;
            }

            // SET LOWER POINTS
            for (int pL = 0; pL < 12; pL++)
            {
                var p = playingFromLeft ? pL : 11 - pL;
                var side = p < 6 ? _pointsRightContainer : _pointsLeftContainer;

                var sub = pL >= 6 ? 6 : 0;
                var stepX = _posXStep * ((pL < 3 || (pL >= 6 && pL < 9)) ? (.5f + (2 - pL + sub)) : (-.5f - (pL - 3 - sub)));
                var position = new Vector3(stepX * (playingFromLeft ? 1 : -1), (-.5f * _pointContainerHeight), 0);

                var point = Instantiate(_point2DPrefab, position, Quaternion.identity, side.transform);

                point.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(_posXStep, _posYStep);
                //point.gameObject.GetComponent<BoxCollider2D>().size = new Vector2(_posXStep, _posYStep);
                point.transform.localPosition = position;
                point.transform.name = (blackOrWhite ? "BLACK" : "WHITE") + counter;

                point.Init(counter--, false, (blackOrWhite ? _defaultBlackMaterial : _defaultWhiteMaterial),
                                             (blackOrWhite ? _defaultBlackTexture : _defaultWhiteTexture),
                                             (blackOrWhite ? _defaultBlackColour : _defaultWhiteColour));
                _points[counter] = point;

                point.OnPointClickedEvent += OnPointClickedAction;

                if (blackOrWhite) _blackPointsRenderers.Add(point.Renderer);
                else _whitePointsRenderers.Add(point.Renderer);

                blackOrWhite = !blackOrWhite;
            }

            Game2D.Context.HomeManager.PlayerHome.OnHomeClickedEvent += OnPointClickedAction;
        }

        internal float DefinePointCounterScaling()
        {
            var scale = GetPointByID(1).GetComponent<RectTransform>().rect.height * .2f;
            Point2DPrefab.CounterHeight = scale;
            return scale;
        }

        internal void SetCountersToStartingPoints(bool playersCounters, Counter2DPrefab[] counters)
        {
            var countersPlaced = 0;
            Point2DPrefab point = null;

            // ADD 2 @ POINT 24
            while (countersPlaced < 2)
            {
                if (playersCounters) point = GetPlayerPointByID(24);
                else point = GetOpponentPointByID(24);
                SetCounterMoveToInstant(counters[countersPlaced++], point);
            }

            // ADD 5 @ POINT 13
            while (countersPlaced < 7)
            {
                if (playersCounters) point = GetPlayerPointByID(13);
                else point = GetOpponentPointByID(13);
                SetCounterMoveToInstant(counters[countersPlaced++], point);
            }

            // ADD 3 @ POINT 8
            while (countersPlaced < 10)
            {
                if (playersCounters) point = GetPlayerPointByID(8);
                else point = GetOpponentPointByID(8);
                SetCounterMoveToInstant(counters[countersPlaced++], point);
            }

            // ADD 5 @ POINT 6
            while (countersPlaced < 15)
            {
                if (playersCounters) point = GetPlayerPointByID(6);
                else point = GetOpponentPointByID(6);
                SetCounterMoveToInstant(counters[countersPlaced++], point);
            }
        }

        internal void SetCountersFromArray(int[] points, Counter2DPrefab[] playerCounters, Counter2DPrefab[] opponentCounters)
        {
            ClearAllCountersFromPoints();

            var playerCounterIndex = 0;
            var opponentCounterIndex = 0;

            for (int p = 0; p < 24; p++)
            {
                if (points[p] == 0) continue;

                var playerOrPro = points[p] > 0 ? true : false;
                var counters = Mathf.Abs(points[p]);

                for (int c = 0; c < counters; c++)
                {
                    var counter = playerOrPro ? playerCounters[playerCounterIndex++] :
                                                opponentCounters[opponentCounterIndex++];
                    var point = GetPlayerPointByID(p + 1);
                    SetCounterMoveToInstant(counter, point);
                }
            }
        }

        internal void ClearAllCountersFromPoints()
        {
            foreach (var p in Points) { p.ResetCounters(); }
        }

        private void SetCounterMoveToInstant(Counter2DPrefab counter, Point2DPrefab point)
        {
            var id = point.GetID();
            var position = Vector3.zero;

            if (id > 18 || id < 7) position = Game2D.Context.IfPlayFromLhs ? _pointsLeftContainer.localPosition :
                                                                             _pointsRightContainer.localPosition;
            else position = Game2D.Context.IfPlayFromLhs ? _pointsRightContainer.localPosition :
                                                           _pointsLeftContainer.localPosition;

            position += point.GetCounterOffsetPosition();

            counter.SetCounterToMoveInstantaneous(position);
            point.PushCounter(counter);
        }

        private void OnDestroy()
        {
            if (_points is not null)
                foreach (Point2DPrefab point in _points)
                { if (point is not null) point.OnPointClickedEvent -= OnPointClickedAction; }

            Game2D.Context.HomeManager.PlayerHome.OnHomeClickedEvent -= OnPointClickedAction;
        }

        // POINT INTERACTIONS
        internal void OnPointClickedAction(PlayablePosition2D position)
        {
            // USED TO SET UP THE BOARD BY HAND
            if (Game2D.IfManualSetupBoard)
            {
                _selectedPointFrom = position.GetID();
                ManualSetupPointSelected = true;
                return;
            }

            // NORMAL CLICK INTERACTION
            if (position.GetID() == _selectedPointFrom)
            {
                // SELECTED SAME POINT -> FROM
                SetPointFromCounterAsSelected(false);
                _selectedPointFrom = -1;
                _ifPointFromSelected = false;
            }
            else if (_selectedPointFrom > 0)
            {
                // TEST IF PLAYER IS SELECTING A VALID POINT TO OR CHANGING POINT FROM
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

                // SELECTED NEW POINT -> FROM
                if (_ifChangingSelectedPointFrom)
                {
                    SetPointFromCounterAsSelected(false);
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

        internal void SetAllPointsActive(bool active)
        {
            foreach (var point in _points)
                point.SetInteractive(active);
        }

        internal bool SetPointsFromInteractive(Game2D.PlayerColour playerColour, Game2D.PlayerColour opponentColour, int dice1, int dice2, int diceAvailable)
        {
            // NOTE: THIS IS NOT TESTING AGAINST BEARING OFF - USE ALTERNATIVE METHOD BELOW

            if (dice1 == 0 && dice2 == 0) return false;

            // FROM POINT 1
            foreach (Point2DPrefab pointFrom in _points)
            {
                // TEST IF PLAYER POINT
                if (pointFrom.Colour != playerColour) continue;

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

            return true;
        }

        internal void SetPointsToInteractive(Game2D.PlayerColour playerColour, Game2D.PlayerColour opponentColour, int dice1, int dice2, int diceAvailable)
        {
            _ifChangingSelectedPointFrom = false;

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

        private bool TestCanMoveDoesNotInvalidateOtherDice(int playedMove, int diceToTest, int validateFrom, int validateTo, Game2D.PlayerColour playerColour, Game2D.PlayerColour opponentColour)
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

        internal bool TestIfCanMoveToPoint(int pointToIndex, Game2D.PlayerColour opponentColour)
        {
            if (pointToIndex > 24) return false;
            else if (!Game2D.Context.IfBearingOff && pointToIndex <= 0) return false;

            if (pointToIndex > 0)
            {
                var pointTo = GetPlayerPointByID(pointToIndex);
                if (pointTo.Colour == opponentColour && pointTo.Counters > 1) return false;
            }

            return true;
        }

        private bool TestDoubleMoveIsNotForcedByBlot(int dice1, int dice2, Game2D.PlayerColour opponentColour)
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
            foreach (Point2DPrefab point in _points)
            {
                point.SetInteractive(false);
            }

            SetPointFromCounterAsSelected(false);

            _ifChangingSelectedPointFrom = false;
        }

        // COUNTER ON BAR
        internal void PlayerCounterOnBar()
        {
            _selectedPointFrom = 25;
            _ifPointFromSelected = true;
        }

        // BEARING OFF
        internal bool TestIfBearingOff(Game2D.PlayerColour playerColour)
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

        internal void SetBearingOffPointsFromInteractive(Game2D.PlayerColour playerColour, Game2D.PlayerColour opponentColour, 
                                                            int c_dice1, int c_dice2, int o_dice1, int o_dice2)
        {
            var forced1 = false;
            var forced2 = false;

            // NOTE: c_dice == CURRENT VALUE OF DICE - '0' - DICE HAS BEEN USED
            //		 o_dice == ORIGINAL / ACTUAL VALUE OF DICE ROLLS

            // NOTE: DOUBLE MOVES NOT REQUIRED - HANDLED IN 'POINT TO'

            for (int pointIndex = 6; pointIndex > 0; pointIndex--)
            {
                var pointFrom = GetPlayerPointByID(pointIndex);

                if (pointFrom.Colour != playerColour) continue;

                // POINT INDEX FOR SMALLER DICE VALUE - FORCED MOVE AT HIGHER POINT
                if ((pointIndex < o_dice1) && (o_dice1 <= o_dice2) && forced2) forced1 = true;
                if ((pointIndex < o_dice2) && (o_dice2 < o_dice1) && forced1) forced2 = true;
                if (forced1 && forced2) break;

                var step1 = pointIndex - c_dice1;
                var step2 = pointIndex - c_dice2;

                var canMove1 = (c_dice1 != 0) && TestIfCanMoveToPoint(step1, opponentColour);
                var canMove2 = (c_dice2 != 0) && TestIfCanMoveToPoint(step2, opponentColour);

                var valid1 = canMove1 && !forced1 && TestCanMoveDoesNotInvalidateOtherDice(step1, c_dice2, 6, 1, playerColour, opponentColour);
                var valid2 = canMove2 && !forced2 && TestCanMoveDoesNotInvalidateOtherDice(step2, c_dice1, 6, 1, playerColour, opponentColour);

                if (c_dice1 != 0) pointFrom.SetInteractive(valid1, true);
                if (!valid1 && c_dice2 != 0) pointFrom.SetInteractive(valid2, true);

                #region DEBUG
                //if (Game2D.Context.IfBearingOff) Debug.Log($"");

                //if (Game2D.Context.IfBearingOff) Debug.Log($"INDEX: {pointIndex} -> " +
                //                                         $"DICE1: {c_dice1} -> " +
                //                                         $"STEP1: {step1} -> " +
                //                                         $"VALID1: {(c_dice1 != 0 && valid1)}");

                //if (Game2D.Context.IfBearingOff) Debug.Log($"INDEX: {pointIndex} -> " +
                //                                         $"DICE2: {c_dice2} -> " +
                //                                         $"STEP2: {step2} -> " +
                //                                         $"VALID2: {(!valid1 && c_dice2 != 0 && valid2)}");

                //if (Game2D.Context.IfBearingOff) Debug.Log($"FORCED1: {(pointIndex < o_dice1)} - {(o_dice1 < o_dice2)} - {forced2}");
                //if (Game2D.Context.IfBearingOff) Debug.Log($"FORCED2: {(pointIndex < o_dice2)} - {(o_dice2 < o_dice1)} - {forced1}");
                #endregion

                // POINT INDEX FOR LARGER DICE VALUE - FORCED MOVE
                //if (!forced1 && !forced2 || (pointIndex == o_dice1) || (pointIndex == o_dice2))
                if ((c_dice1 != 0 && c_dice2 != 0) || !(forced1 && forced2))
                {
                    if ((o_dice1 > o_dice2) && pointIndex <= o_dice1) forced1 = true;
                    if ((o_dice2 >= o_dice1) && pointIndex <= o_dice2) forced2 = true;
                }

                if (c_dice1 == 0 && (o_dice1 < o_dice2) && pointIndex <= o_dice2) { forced1 = true; }
                else if (c_dice2 == 0 && (o_dice2 < o_dice1) && pointIndex <= o_dice1) { forced2 = true; }
            }
        }

        internal void SetBearingOffPointsToInteractive(Game2D.PlayerColour playerColour, Game2D.PlayerColour opponentColour, 
                                                          int dice1, int dice2, int diceAvailable)
        {
            _ifChangingSelectedPointFrom = false;

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
                        if (!TestCanMoveDoesNotInvalidateOtherDice(pointToIndex, dice1, 6, pointToIndex,
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
                        Game2D.Context.HomeManager.PlayerHome.SetInteractive(true, true, true);
                        break;
                    }

                    //if (Game2D.Context.IfBearingOff) Debug.Log($"FROM: {_selectedPointFrom} -> " +
                    //                                         $"DICE1: {dice1} -> " +
                    //                                         $"STEP1: {step} -> ");
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

                #region DEBUG
                //if (Game2D.Context.IfBearingOff) Debug.Log($"BEARING OFF TO");

                //if (Game2D.Context.IfBearingOff) Debug.Log($"FROM: {_selectedPointFrom} -> " +
                //                                           $"DICE1: {dice1} -> " +
                //                                           $"STEP1: {step1} -> " +
                //                                           $"VALID1: {valid1} " +
                //                                           $"MOVE1: {(valid1 && (dice1 != 0))}");

                //if (Game2D.Context.IfBearingOff) Debug.Log($"FROM: {_selectedPointFrom} -> " +
                //                                         $"DICE2: {dice2} -> " +
                //                                         $"STEP2: {step2} -> " +
                //                                         $"VALID2: {valid2} " +
                //                                         $"MOVE2: {(valid2 && (dice2 != 0))}");
                #endregion

                if (dice1 != 0)
                {
                    if (step1 > 0)
                    {
                        GetPlayerPointByID(step1).SetInteractive(valid1, true, true);
                        _validPointsToArray[0] = step1;
                    }
                    else Game2D.Context.HomeManager.PlayerHome.SetInteractive(valid1, true, true);
                }
                if (dice2 != 0)
                {
                    if (step2 > 0)
                    {
                        GetPlayerPointByID(step2).SetInteractive(valid2, true, true);
                        _validPointsToArray[1] = step2;
                    }
                    else Game2D.Context.HomeManager.PlayerHome.SetInteractive(valid2, true, true);
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

        internal bool TestNoForecedMoveFromHigherPoint(int diceRoll, Game2D.PlayerColour playerColour)
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
            Game2D.Context.HomeManager.PlayerHome.SetInteractive(false);
        }

        // BLOT
        internal bool IfBlot(int pointTo, Game2D.PlayerColour opponentColour)
        {
            var point = GetPlayerPointByID(pointTo);
            return (point.Colour == opponentColour && point.Counters == 1);
        }

        // COUNTER INTERACTION
        private void SetPointFromCounterAsSelected(bool selected)
        {
            if (_selectedPointFrom <= 0 || _selectedPointFrom == 25) return;

            var point = GetPlayerPointByID(_selectedPointFrom);
            point.SetCounterAsSelected(selected);
        }

        // GET POINT
        internal Vector3 GetAdjustedPointPosition(PlayablePosition2D point)
        {
            var id = point.GetID();
            var position = Vector3.zero;

            if (id > 18 || id < 7) position = Game2D.Context.IfPlayFromLhs ? _pointsLeftContainer.localPosition :
                                                                             _pointsRightContainer.localPosition;
            else position = Game2D.Context.IfPlayFromLhs ? _pointsRightContainer.localPosition :
                                                           _pointsLeftContainer.localPosition;

            position += point.GetCounterOffsetPosition();

            return position;
        }

        internal Vector3 GetAdjustedPointPositionMultiple(Point2DPrefab point, int multiple)
        {
            var id = point.GetID();
            var position = Vector3.zero;

            if (id > 18 || id < 7) position = Game2D.Context.IfPlayFromLhs ? _pointsLeftContainer.localPosition :
                                                                             _pointsRightContainer.localPosition;
            else position = Game2D.Context.IfPlayFromLhs ? _pointsRightContainer.localPosition :
                                                           _pointsLeftContainer.localPosition;

            position += point.GetActiveCounterPosition(multiple);

            return position;
        }

        internal Point2DPrefab GetPointByID(int id) { return (id > 0 ? _points[id - 1] : Instantiate(_point2DPrefab)); }

        internal Point2DPrefab GetPlayerPointByID(int id) { return GetPointByID(id); }

        internal Point2DPrefab GetOpponentPointByID(int id) { return GetPointByID(25 - id); }

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

        internal void ResetFromManualPlacement()
        {
            ManualSetupPointSelected = false;
            _selectedPointFrom = -1;
        }

        // ----------------------------------------- GETTERS && SETTERS -----------------------------------------

        private Point2DPrefab[] _points = null;

        private bool _ifPointFromSelected = false;
        private bool _ifPointToSelected = false;
        private bool _ifChangingSelectedPointFrom = false;
        private int[] _validPointsToArray = new int[4];

        private int _selectedPointFrom = -1;
        private int _selectedPointTo = -1;

        private bool _ifBearingOff = false;

        private float _pointContainerWidth = 0f;
        private float _pointContainerHeight = 0f;

        public Point2DPrefab[] Points { get => _points; }
        public bool IfPointFromSelected { get => _ifPointFromSelected; }
        public bool IfPointToSelected { get => _ifPointToSelected; }
        public bool IfChangingSelectedPointFrom { get => _ifChangingSelectedPointFrom; }
        public int SelectedPointFrom { get => _selectedPointFrom; }
        public int SelectedPointTo { get => _selectedPointTo; }

        // FOR MANUAL CONFIGURATION OF BOARD
        public bool ManualSetupPointSelected { get; set; }
    }
}