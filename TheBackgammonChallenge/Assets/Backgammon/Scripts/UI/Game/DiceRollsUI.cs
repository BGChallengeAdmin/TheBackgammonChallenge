using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

namespace Backgammon
{
    public class DiceRollsUI : MonoBehaviour
    {
        [Header("TEXT FIELDS")]
        [SerializeField] TMP_Text _diceRollsText;
        [SerializeField] TMP_Text _playerDiceRollsButtonText;
        [SerializeField] TMP_Text _playerTurnDiceRollsText;
        [SerializeField] TMP_Text _opponentTurnDiceRollsText;

        [Header("DICE IMAGES")]
        [SerializeField] Image _playerDice1Image;
        [SerializeField] Image _playerDice2Image;
        [SerializeField] Image _playerDice3Image;
        [SerializeField] Image _playerDice4Image;
        [SerializeField] Image _opponentDice1Image;
        [SerializeField] Image _opponentDice2Image;
        [SerializeField] Image _opponentDice3Image;
        [SerializeField] Image _opponentDice4Image;

        [Header("DICE SPRITES")]
        [SerializeField] Sprite[] _diceBlackSprites;
        [SerializeField] Sprite[] _diceWhiteSprites;

        [Header("DICE EFFECTS")]
        [SerializeField] DiceEffects _diceEffectBlackPrefab;
        [SerializeField] DiceEffects _diceEffectWhitePrefab;

        [Header("TRANSFORMS")]
        [SerializeField] Transform _playerDiceContainer;
        [SerializeField] Transform _opponentDiceContainer;
        [SerializeField] Transform _leftSidePointsContainer;
        [SerializeField] Transform _rightSidePointsContainer;

        [Header("BUTTONS")]
        [SerializeField] Button _playerTapToRollButton;

        private void Awake()
        {
            var verticalAdjst = new Vector3(0f, _playerDiceContainer.position.y, 0f);

            _playerDiceContainer.position = _rightSidePointsContainer.position + verticalAdjst;
            _opponentDiceContainer.position = _leftSidePointsContainer.position + verticalAdjst;

            _playerDiceRollsButtonText.gameObject.transform.localPosition = _playerDiceContainer.localPosition + new Vector3(0f, -100f, 0f);

            _playerTapToRollButton.onClick.AddListener(() => OnClickTapToRoll());
        }

        private void OnDestroy()
        {
            _playerTapToRollButton.onClick.RemoveAllListeners();
        }

        internal void SetActive(bool active)
        {
            if (active) SetAllOff();
            if (!active) ResetImageSizes();

            this.gameObject.SetActive(active);

            _playerTapToRollButton.gameObject.SetActive(false);

            _playerTapToRoll = false;
            _diceRollAnimation = false;

            doubleDicePlayedCounter = 0;
        }

        internal void SetDiceUIScale(float counterScale)
        {
            var scale = counterScale * .012f;
            _playerDiceContainer.localScale = new Vector3(scale, scale, scale);
            _opponentDiceContainer.localScale = new Vector3(scale, scale, scale);
        }

        internal void SetAsBlackOrWhite(bool blackOrWhite, float counterScale)
        {
            if (_diceEffects1 is not null) Destroy(_diceEffects1.gameObject);
            if (_diceEffects2 is not null) Destroy(_diceEffects2.gameObject);

            _diceEffects1 = Instantiate(blackOrWhite ? _diceEffectBlackPrefab : _diceEffectWhitePrefab,
                                            _playerDice2Image.transform.localPosition,
                                            Quaternion.identity,
                                            _playerDiceContainer);

            _diceEffects2 = Instantiate(blackOrWhite ? _diceEffectBlackPrefab : _diceEffectWhitePrefab,
                                            _playerDice3Image.transform.localPosition,
                                            Quaternion.identity,
                                            _playerDiceContainer);

            _diceEffects1.transform.localPosition =  new Vector3(-80f, 0f, 0f);
            _diceEffects2.transform.localPosition =  new Vector3(80f, 0f, 0f); 

            var scale = counterScale * .012f;
            _diceEffects1.transform.localScale = new Vector3(scale, scale, scale);
            _diceEffects2.transform.localScale = new Vector3(scale, scale, scale);

            _diceEffects1.SetActive(false);
            _diceEffects2.SetActive(false);
        }

        // TAP TO ROLL
        internal void SetPlayerTapToRoll()
        {
            _playerTapToRollButton.gameObject.SetActive(true);

            var randomSeed = 25;
            _randomVec1 = new Vector3(1f + (Random.Range(-randomSeed, randomSeed) * .1f),
                                    1f + (Random.Range(-randomSeed, randomSeed) * .1f),
                                    1f + (Random.Range(-randomSeed, randomSeed) * .1f));

            _randomVec2 = new Vector3(1f + (Random.Range(-randomSeed, randomSeed) * .1f),
                                    1f + (Random.Range(-randomSeed, randomSeed) * .1f),
                                    1f + (Random.Range(-randomSeed, randomSeed) * .1f));
            
            _diceEffects1.SetActive(true);
            _diceEffects2.SetActive(true);

            _diceEffects1.ResetDice();
            _diceEffects2.ResetDice();
        }

        private void OnClickTapToRoll()
        {
            _playerTapToRollButton.gameObject.SetActive(false);
            _playerTapToRoll = true;

            StartCoroutine(DiceRollEffectCoroutine());
        }

        private IEnumerator DiceRollEffectCoroutine()
        {
            _diceEffects1.SetDiceFaceBlank(false);
            _diceEffects2.SetDiceFaceBlank(false);

            while (_diceEffectTimer >= 0)
            {
                _diceEffectTimer -= Time.deltaTime;

                _diceEffects1.AddTorque(ApplyTorque(1));
                _diceEffects2.AddTorque(ApplyTorque(2));

                _diceEffects1.AnimateScale(_speedVec.x * 0.0005f);
                _diceEffects2.AnimateScale(_speedVec.x * 0.0005f);

                yield return null;
            }

            _diceEffects1.SetActive(false);
            _diceEffects2.SetActive(false);

            _diceEffectTimer = _diceEffectTime;
            _diceRollAnimation = true;
        }

        private Vector3 ApplyTorque(int dice = 1)
        {
            var torque = _torqueVec.x * _speedVec.x * .1f;

            if (dice == 2)
                return new Vector3(torque * _randomVec2.x,
                                torque * _randomVec2.y,
                                torque * _randomVec2.z);
            else
                return new Vector3(torque * _randomVec1.x,
                                torque * _randomVec1.y,
                                torque * _randomVec1.z);
        }

        // SET DICE FACES
        internal void SetPlayerDiceFaceValues(bool playerIsBlack, int dice1, int dice2)
        {
            if (dice1 < 0 || dice1 > 6 || dice2 < 0 || dice2 > 6) return;

            if (dice1 == dice2 && dice1 != 0)
            {
                EnableDice(_playerDice1Image, dice1, playerIsBlack);
                EnableDice(_playerDice2Image, dice1, playerIsBlack);
                EnableDice(_playerDice3Image, dice1, playerIsBlack);
                EnableDice(_playerDice4Image, dice1, playerIsBlack);
            }
            else
            {
                EnableDice(_playerDice2Image, dice1, playerIsBlack);
                EnableDice(_playerDice3Image, dice2, playerIsBlack);
            }
        }

        internal void SetPlayerDiceRollsText(string playerName, int dice1, int dice2, bool toShow)
        {
            if (toShow)
            {
                if (playerName != string.Empty)
                {
                    _playerTurnDiceRollsText.text = $"{playerName} rolled {dice1}/{dice2}. " +
                                                    $"How would you play it? \n" +
                                                    $"Make your move now.";
                }
                else _playerTurnDiceRollsText.text = "Make Your Move";
            }
            else _playerTurnDiceRollsText.text = string.Empty;
        }

        internal void SetOpponentDiceFaceValues(bool playerIsBlack, int dice1, int dice2)
        {
            if (dice1 < 0 || dice1 > 6 || dice2 < 0 || dice2 > 6) return;

            if (dice1 == dice2)
            {
                EnableDice(_opponentDice1Image, dice1, playerIsBlack);
                EnableDice(_opponentDice2Image, dice1, playerIsBlack);
                EnableDice(_opponentDice3Image, dice1, playerIsBlack);
                EnableDice(_opponentDice4Image, dice1, playerIsBlack);
            }
            else
            {
                EnableDice(_opponentDice2Image, dice1, playerIsBlack);
                EnableDice(_opponentDice3Image, dice2, playerIsBlack);
            }
        }

        internal void SetOpponentDiceRollsText(string opponentName, int dice1, int dice2, bool toShow)
        {
            if (toShow)
                _opponentTurnDiceRollsText.text = $"{opponentName} moves.";
            else
                _opponentTurnDiceRollsText.text = string.Empty;
        }

        private void EnableDice(Image dice, int value, bool black)
        {
            dice.gameObject.SetActive(true);
            dice.sprite = DiceImage(value, black);
        }

        private Sprite DiceImage(int dice, bool black)
        {
            return black ? _diceBlackSprites[dice] : _diceWhiteSprites[dice];
        }

        // SET DICE PLAYED
        internal void SetDicePlayed(bool playerTurn, bool dice1or2)
        {
            if (playerTurn)
            {
                if (dice1or2) _playerDice2Image.transform.localScale = _dicePlayedScale;
                else _playerDice3Image.transform.localScale = _dicePlayedScale;
            }
            else
            {
                if (dice1or2) _opponentDice2Image.transform.localScale = _dicePlayedScale;
                else _opponentDice3Image.transform.localScale = _dicePlayedScale;
            }
        }

        int doubleDicePlayedCounter = 0;
        internal void SetDoubleDicePlayed(bool playerTurn, int number)
        {
            for (int dice = doubleDicePlayedCounter; dice <= (doubleDicePlayedCounter + number); ++dice)
            {
                if (dice == 1)
                {
                    if (playerTurn) _playerDice1Image.transform.localScale = _dicePlayedScale;
                    else _opponentDice1Image.transform.localScale = _dicePlayedScale;
                }
                else if (dice ==2)
                {
                    if (playerTurn) _playerDice2Image.transform.localScale = _dicePlayedScale;
                    else _opponentDice2Image.transform.localScale = _dicePlayedScale;
                }
                else if (dice == 3)
                {
                    if (playerTurn) _playerDice3Image.transform.localScale = _dicePlayedScale;
                    else _opponentDice3Image.transform.localScale = _dicePlayedScale;
                }
                else if (dice == 4)
                {
                    if (playerTurn) _playerDice4Image.transform.localScale = _dicePlayedScale;
                    else _opponentDice4Image.transform.localScale = _dicePlayedScale;
                }
            }

            doubleDicePlayedCounter += number;
        }

        // RESET
        private void SetAllOff()
        {
            _playerDice1Image.gameObject.SetActive(false);
            _playerDice2Image.gameObject.SetActive(false);
            _playerDice3Image.gameObject.SetActive(false);
            _playerDice4Image.gameObject.SetActive(false);

            _opponentDice1Image.gameObject.SetActive(false);
            _opponentDice2Image.gameObject.SetActive(false);
            _opponentDice3Image.gameObject.SetActive(false);
            _opponentDice4Image.gameObject.SetActive(false);

            _diceEffects1.SetActive(false);
            _diceEffects2.SetActive(false);
        }

        internal void ResetImageSizes()
        {
            doubleDicePlayedCounter = 0;

            _playerDice1Image.transform.localScale = Vector3.one;
            _playerDice2Image.transform.localScale = Vector3.one;
            _playerDice3Image.transform.localScale = Vector3.one;
            _playerDice4Image.transform.localScale = Vector3.one;

            _opponentDice1Image.transform.localScale = Vector3.one;
            _opponentDice2Image.transform.localScale = Vector3.one;
            _opponentDice3Image.transform.localScale = Vector3.one;
            _opponentDice4Image.transform.localScale = Vector3.one;
        }

        internal void ResetPlayerDiceRollsText()
        {
            SetPlayerDiceRollsText(string.Empty, 0, 0, false);
            SetOpponentDiceRollsText(string.Empty, 0, 0, false);
        }

        private Vector3 _dicePlayedScale = new Vector3(.8f, .8f, .8f);

        // GETTERS && SETTERS

        private bool _playerTapToRoll = false;
        private bool _diceRollAnimation = false;

        private DiceEffects _diceEffects1;
        private DiceEffects _diceEffects2;

        private float _diceEffectTime = 2.5f;
        private float _diceEffectTimer = 2.5f;

        private Vector3 _torqueVec = new Vector3(3f, 3f, 3f);
        private Vector3 _speedVec = new Vector3(6f, 6f, 6f);
        private Vector3 _randomVec1;
        private Vector3 _randomVec2;

        internal bool PlayerTapToRoll { get => _playerTapToRoll; }
        internal bool DiceRollAnimation { get => _diceRollAnimation; }
    }
}