using Backgammon;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RollForGoesFirstUI : MonoBehaviour
{
    [Header("DICE EFFECTS")]
    [SerializeField] DiceEffects _diceEffectBlackPrefab;
    [SerializeField] DiceEffects _diceEffectWhitePrefab;

    [Header("TRANSFORMS")]
    [SerializeField] Transform _playerDiceContainer;
    [SerializeField] Transform _opponentDiceContainer;
    [SerializeField] Transform _leftSidePointsContainer;
    [SerializeField] Transform _rightSidePointsContainer;

    [Header("DICE IMAGES")]
    [SerializeField] Image _playerDiceImage;
    [SerializeField] Image _opponentDiceImage;
    
    [Header("DICE SPRITES")]
    [SerializeField] Sprite[] _diceBlackSprites;
    [SerializeField] Sprite[] _diceWhiteSprites;

    private DiceEffects _playerDiceEffects;
    private DiceEffects _opponentDiceEffects;

    private float _diceEffectTime = 2.5f;
    private float _diceEffectTimer = 2.5f;

    private Vector3 _torqueVec = new Vector3(3f, 3f, 3f);
    private Vector3 _speedVec = new Vector3(6f, 6f, 6f);
    private Vector3 _randomVec1;
    private Vector3 _randomVec2;

    private bool _playerIsBlack;
    private bool _playerGoesFirst;
    private float _showDiceRollsDelay = .5f;
    internal bool Animating { get; private set; }

    // CONFIG.
    internal void Init(float counterScale)
    {
        _playerDiceContainer.position = _rightSidePointsContainer.position;
        _opponentDiceContainer.position = _leftSidePointsContainer.position;

        _playerIsBlack = Game2D.Context.IfPlayerIsBlack;
        SetAsBlackOrWhite(_playerIsBlack, counterScale);
    }

    internal void SetActive(bool active)
    {
        this.gameObject.SetActive(active);
    }

    private void SetAsBlackOrWhite(bool blackOrWhite, float counterScale)
    {
        if (_playerDiceEffects is not null) Destroy(_playerDiceEffects.gameObject);
        if (_opponentDiceEffects is not null) Destroy(_opponentDiceEffects.gameObject);

        _playerDiceEffects = Instantiate(blackOrWhite ? _diceEffectBlackPrefab : _diceEffectWhitePrefab,
                                        _playerDiceContainer.transform.localPosition,
                                        Quaternion.identity,
                                        _playerDiceContainer);

        _opponentDiceEffects = Instantiate(!blackOrWhite ? _diceEffectBlackPrefab : _diceEffectWhitePrefab,
                                        _opponentDiceContainer.transform.localPosition,
                                        Quaternion.identity,
                                        _opponentDiceContainer);

        _playerDiceEffects.transform.localPosition = Vector3.zero;
        _opponentDiceEffects.transform.localPosition = Vector3.zero;

        var scale = counterScale * .012f;
        _playerDiceEffects.SetScale(scale);
        _opponentDiceEffects.SetScale(scale);

        _playerDiceEffects.SetActive(false);
        _opponentDiceEffects.SetActive(false);
    }

    // ANIMATIONS
    internal void RollOffForGoesFirst(bool playerGoesFirst, int playerDice, int opponentDice)
    {
        Animating = true;
        _playerGoesFirst = playerGoesFirst;

        var randomSeed = 25;
        _randomVec1 = new Vector3(1f + (Random.Range(-randomSeed, randomSeed) * .1f),
                                1f + (Random.Range(-randomSeed, randomSeed) * .1f),
                                1f + (Random.Range(-randomSeed, randomSeed) * .1f));

        _randomVec2 = new Vector3(1f + (Random.Range(-randomSeed, randomSeed) * .1f),
                                1f + (Random.Range(-randomSeed, randomSeed) * .1f),
                                1f + (Random.Range(-randomSeed, randomSeed) * .1f));

        StartCoroutine(DiceRollEffectCoroutine(playerDice, opponentDice));
    }

    private IEnumerator DiceRollEffectCoroutine(int playerDice, int opponentDice)
    {
        SetDiceImagesBlank();

        _playerDiceEffects.SetActive(true);
        _opponentDiceEffects.SetActive(true);

        _playerDiceEffects.SetDiceFaceBlank(false);
        _opponentDiceEffects.SetDiceFaceBlank(false);

        while (_diceEffectTimer >= 0)
        {
            _diceEffectTimer -= Time.deltaTime;

            _playerDiceEffects.AddTorque(ApplyTorque(1));
            _opponentDiceEffects.AddTorque(ApplyTorque(2));

            _playerDiceEffects.AnimateScale(_speedVec.x * 0.0005f);
            _opponentDiceEffects.AnimateScale(_speedVec.x * 0.0005f);

            yield return null;
        }

        _playerDiceEffects.SetActive(false);
        _opponentDiceEffects.SetActive(false);

        _diceEffectTimer = _diceEffectTime;

        SetPlayerDiceFaceValues(_playerIsBlack, playerDice);
        SetOpponentDiceFaceValues(!_playerIsBlack, opponentDice);

        yield return new WaitForSeconds(_showDiceRollsDelay);

        Animating = false;
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
    private void SetPlayerDiceFaceValues(bool playerIsBlack, int dice)
    {
        if (dice < 0 || dice > 6) return;
        EnableDice(_playerDiceImage, dice, playerIsBlack);
    }

    private void SetOpponentDiceFaceValues(bool playerIsBlack, int dice)
    {
        if (dice < 0 || dice > 6) return;
        EnableDice(_opponentDiceImage, dice, playerIsBlack);    
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

    private void SetDiceImagesBlank()
    {
        _playerDiceImage.gameObject.SetActive(false);
        _opponentDiceImage.gameObject.SetActive(false);
    }
}
