using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Backgammon
{
    public class DoublingUI : MonoBehaviour
    {
        [Header("TEXT FIELDS")]
        [SerializeField] TMP_Text _doublingText;
        [SerializeField] TMP_Text _inGameDoublingText;

        [Header("TRANSFORMS")]
        [SerializeField] Transform _doublingDialogContainer;
        [SerializeField] Transform _inGameDoublingDialogContainer;
        [SerializeField] Transform _inGameDoublingConfirmButton;

        [Header("BUTTONS")]
        [SerializeField] Button _yesButton;
        [SerializeField] Button _noButton;
        [SerializeField] Button _confirmButton;

        [Header("SPRITES")]
        [SerializeField] Sprite[] _cubeSpritesArray;

        [Header("DEBUG")]
        [SerializeField] DebugPrefab debug_Doubling;

        private TMP_Text _confirmButtonText;
        private TMP_Text _declineButtonText;

        private void Awake()
        {
            _confirmButtonText = _yesButton.gameObject.GetComponentInChildren<TMP_Text>();
            _declineButtonText = _noButton.gameObject.GetComponentInChildren<TMP_Text>();

            _yesButton.onClick.AddListener(() => OnClickYesButton());
            _noButton.onClick.AddListener(() => OnClickNoButton());
            _confirmButton.onClick.AddListener(() => OnClickConfirmButton());
        }

        private void OnDestroy()
        {
            _yesButton.onClick.RemoveAllListeners();
            _noButton.onClick.RemoveAllListeners();
            _confirmButton.onClick.RemoveAllListeners();
        }

        internal void SetActive(bool active)
        {
            _doublingDialogContainer.gameObject.SetActive(false);
            _inGameDoublingDialogContainer.gameObject.SetActive(false);
            _inGameDoublingConfirmButton.gameObject.SetActive(false);

            _ifClicked = false;
            _ifClickedYes = false;
            _ifClickedNo = false;
            _ifClickedConfirm = false;

            this.gameObject.SetActive(active);
        }

        // IN GAME OFFERS DOUBLES
        internal void IfOfferedDoubles()
        {
            _inGameDoublingDialogContainer.gameObject.SetActive(true);
            _inGameDoublingText.text = "Would you like to offer Doubles?";
        }

        internal void SetOfferedDoublesText(Probabilities probabilities, bool playerOfferedDoubles)
        {
            _inGameDoublingConfirmButton.gameObject.SetActive(true);

            // TODO: SET CORRECT / INCORRECT - %

            _inGameDoublingText.text = "CORRECT / INCORRECT";

            if (probabilities != null)
            {
                var decision = probabilities.cubeDecision.owner;

                _inGameDoublingText.text = CalculateDoublingValues(probabilities, playerOfferedDoubles);
            }
        }

        private string CalculateDoublingValues(Probabilities probabilities, bool playerOfferedDoubles)
        {
            // IF PLAYER / PRO MATCHED CORRECT DECISIONS - TAKE / DROP / DOUBLE / REDOUBLE

            var aiEvaluation = string.Empty;

            if (playerOfferedDoubles)
            {
                // TEST AGAINST PLAYER OR PRO OFFERING DOUBLES

                if (probabilities.cubeDecision.owner == "togood")
                {
                    if (_ifClickedYes)
                    {
                        //aiEvaluation = languageSO.doublingTOOGOOD;
                        aiEvaluation = "INCORRECT; Equity is 'TOO GOOD', should have held for GAMMON";
                        debug_Doubling.DebugMessage($"DOUBLING: INCORRECT - TOO GOOD - PLAY FOR GAMMON");
                    }
                    else if (_ifClickedNo)
                    {
                        //aiEvaluation = languageSO.doublingCORRECT;
                        aiEvaluation = "CORRECT: Equity is 'TOO GOOD', hold for GAMMON";
                        debug_Doubling.DebugMessage($"DOUBLING: CORRECT - TOO GOOD - PLAY FOR GAMMON");
                    }

                    debug_Doubling.DebugMessage($"DOUBLING: PRO SHOULD HAVE HELD");
                }
                else if (probabilities.cubeDecision.owner == "hold")
                {
                    if (_ifClickedYes)
                    {
                        //aiEvaluation = languageSO.doublingINCORRECT;
                        aiEvaluation = "INCORRECT: Should hold";
                        debug_Doubling.DebugMessage($"DOUBLING: INCORRECT - SOULD HOLD");
                    }
                    else if (_ifClickedNo)
                    {
                        //aiEvaluation = languageSO.doublingCORRECT;
                        aiEvaluation = "CORRECT: Should hold";
                        debug_Doubling.DebugMessage($"DOUBLING: CORRECT - SHOULD HOLD");
                    }

                    debug_Doubling.DebugMessage($"DOUBLING: PRO SHOULD HAVE HELD");
                }
                else if (probabilities.cubeDecision.owner == "double")
                {
                    if (_ifClickedYes)
                    {
                        //aiEvaluation = languageSO.doublingCORRECT;
                        aiEvaluation = "CORRECT: Should double";
                        debug_Doubling.DebugMessage($"DOUBLING: CORRECT DECISION TO DOUBLE");
                    }
                    else if (_ifClickedNo)
                    {
                        //aiEvaluation = languageSO.doublingINCORRECT;
                        aiEvaluation = "INCORRECT: Should have doubled";
                        debug_Doubling.DebugMessage($"DOUBLING: INCORRECT - SHOULD HAVE DOUBLED");
                    }

                    debug_Doubling.DebugMessage($"DOUBLING: PRO WAS CORRECT");
                }
                else if (probabilities.cubeDecision.owner == "redouble")
                {
                    if (_ifClickedYes)
                    {
                        //aiEvaluation = languageSO.doublingCORRECT;
                        aiEvaluation = "CORRECT: Should re-double";
                        debug_Doubling.DebugMessage($"DOUBLING: CORRECT DECISION TO RE-DOUBLE");
                    }
                    else if (_ifClickedNo)
                    {
                        //aiEvaluation = languageSO.doublingINCORRECT;
                        aiEvaluation = "INCORRECT: Should have re-doubled";
                        debug_Doubling.DebugMessage($"DOUBLING: INCORRECT - SHOULD HAVE RE-DOUBLED");
                    }

                    debug_Doubling.DebugMessage($"DOUBLING: PRO WAS CORRECT");
                }
            }
            else if (!playerOfferedDoubles)
            {
                // OPPONENT OFFERED DOUBLES

                if (probabilities.cubeDecision.other == "accept")
                {
                    if (_ifClickedYes)
                    {
                        //aiEvaluation = languageSO.doublingCORRECT;
                        aiEvaluation = "CORRECT: Should accept the double";
                        debug_Doubling.DebugMessage($"DOUBLING: CORRECT - DECISION TO ACCEPT");
                    }
                    else if (_ifClickedNo)
                    {
                        //aiEvaluation = languageSO.doublingINCORRECT;
                        aiEvaluation = "INCORRECT: Should have accepted the double";
                        debug_Doubling.DebugMessage($"DOUBLING: INCORRECT - SHOULD HAVE ACCEPTED");
                    }

                    //if (_proTakesOrDrops)
                    //{
                    //    _proDoublingValue += 1;
                    //    Debug.Log($"DOUBLING: PRO WAS CORRECT");
                    //}
                    //else if (!_proTakesOrDrops)
                    //{
                    //    Debug.Log($"DOUBLING: PRO WAS INCORRECT - SHOULD HAVE ACCEPTED");
                    //}
                }
                else if (probabilities.cubeDecision.other == "resign")
                {
                    if (_ifClickedYes)
                    {
                        //aiEvaluation = languageSO.doublingINCORRECT;
                        aiEvaluation = "INCORRECT: Should have dropped";
                        debug_Doubling.DebugMessage($"DOUBLING: INCORRECT - SHOULD HAVE DROPPED");
                    }
                    else if (_ifClickedNo)
                    {
                        //aiEvaluation = languageSO.doublingCORRECT;
                        aiEvaluation = "CORRECT: Should not have taken the double";
                        debug_Doubling.DebugMessage($"DOUBLING: CORRECT - DROPPED CORRECTLY");
                    }

                    //if (_proTakesOrDrops)
                    //{
                    //    Debug.Log($"DOUBLING: PRO WAS INCORRECT - SHOULD HAVE DROPPED");
                    //}
                    //else if (!_proTakesOrDrops)
                    //{
                    //    Debug.Log($"DOUBLING: PRO WAS CORRECT - DROPPED CORRECTLY");
                    //}
                }
            }

            return aiEvaluation;
        }

        private void OnClickYesButton()
        {
            _ifClicked = true;
            _ifClickedYes = true;
        }

        private void OnClickNoButton()
        {
            _ifClicked = true;
            _ifClickedNo = true;
        }

        private void OnClickConfirmButton()
        {
            _ifClicked = true;
            _ifClickedConfirm = true;
        }

        // HELPER METHODS
        internal void SetOffersDoubleText(string player, string opponent)
        {
            _inGameDoublingDialogContainer.gameObject.SetActive(true);
            _inGameDoublingText.text = player + " has offered Double. Should " + opponent + " accept?";
        }

        internal void SetTakesDoubleText(string player)
        {
            _doublingDialogContainer.gameObject.SetActive(true);
            _doublingText.text = player + " Takes the Double";

            if (Main.Instance.IfPlayerVsAI && Game2D.Context.IsPlayersTurn)
                _doublingText.text = player + " Take the Double";
        }

        internal void SetDropsDoubleText(string player)
        {
            _doublingDialogContainer.gameObject.SetActive(true);
            _doublingText.text = player + " Drops the Double";

            if (Main.Instance.IfPlayerVsAI && Game2D.Context.IsPlayersTurn)
                _doublingText.text = player + " Drop the Double";
        }

        internal void SetCubeImage(Image cubeImage)
        {
            _cubeImage = cubeImage;
        }

        internal void SetCubeFace(int doublingIndex)
        {
            _cubeImage.sprite = _cubeSpritesArray[doublingIndex];
        }

        // GETTERS && SETTERS

        private Image _cubeImage;
        
        private bool _ifClicked = false;
        private bool _ifClickedYes = false;
        private bool _ifClickedNo = false;
        private bool _ifClickedConfirm = false;

        internal bool IfClicked { get => _ifClicked; }
        internal bool IfClickedYes { get => _ifClickedYes; }
        internal bool IfClickedNo { get => _ifClickedNo; }
        internal bool IfClickedConfirm { get => _ifClickedConfirm; }
    }
}