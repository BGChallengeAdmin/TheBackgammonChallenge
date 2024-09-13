using UnityEngine;
using UnityEngine.UI;

namespace Backgammon
{
    public class DiceRollsManager : MonoBehaviour
    {
        [SerializeField]
        private DiceEffects diceEffectPrefabBlack;
        [SerializeField]
        private DiceEffects diceEffectPrefabWhite;
        [SerializeField]
        private GameObject dice1Transform;
        [SerializeField]
        private GameObject dice2Transform;
        [SerializeField]
        private Text tapToRollText;
        [SerializeField]
        private Text tapToRollDemoHeaderText;
        [SerializeField]
        private Button tapToRollDiceButton;
        [SerializeField]
        private GameObject _doublingCubeContainer;

        private DiceEffects dice1;
        private DiceEffects dice2;
        private bool animate;

        private Vector3 torqueVec;
        private Vector3 scaleVec;
        private Vector3 speedVec;
        private Vector3 randomVec1;
        private Vector3 randomVec2;
        private float invert = 1f;

        private float timer = 0f;
        private float updateRate = 0f;
        private float updateRateTimer = 0f;
        private float totalAnimationTime = 2.2f;

        private bool playingAs;
        private bool playerInteraction;

        LanguageScriptableObject languageSO;

        private void Awake()
        {
            _doublingButtonAnimationTimer = _doublingButtonAnimationTime;

            updateRate = totalAnimationTime / 25f;

            var torqueValue = 60f / 20f;
            torqueVec = new Vector3(torqueValue, torqueValue, torqueValue);

            var speedValue = 60f / 10f;
            speedVec = new Vector3(speedValue, speedValue, speedValue);

            // CONFIGURE LANGUAGE

            languageSO = Main.Instance.WorldRegionObj.LanguageSO;

            if (languageSO != null)
            {
                tapToRollText.text = languageSO.diceRollsTapToRoll;
            }
        }

        private void OnEnable()
        {
            PlayerInteraction = false;
            Animating = false;
            ClickedDoubling = false;

            var randomValue = 25;
            randomVec1 = new Vector3(1f + (Random.Range(-randomValue, randomValue) / 10f),
                                    1f + (Random.Range(-randomValue, randomValue) / 10f),
                                    1f + (Random.Range(-randomValue, randomValue) / 10f));

            randomVec2 = new Vector3(1f + (Random.Range(-randomValue, randomValue) / 10f),
                                    1f + (Random.Range(-randomValue, randomValue) / 10f),
                                    1f + (Random.Range(-randomValue, randomValue) / 10f));

            timer = 0f;
            updateRateTimer = 0f;

            tapToRollText.gameObject.SetActive(true);
            _doublingCubeContainer.SetActive(false);

            ResetDice();
            ShowDice(true);
        }

        public void SetPlayingAsBlackORWhite(bool _playingAsBlack)
        {
            foreach (Transform child in dice1Transform.transform)
            {
                Destroy(child.gameObject);
            }

            foreach (Transform child in dice2Transform.transform)
            {
                Destroy(child.gameObject);
            }

            if (_playingAsBlack)
            {
                dice1 = Instantiate<DiceEffects>(diceEffectPrefabBlack) as DiceEffects;
                dice2 = Instantiate<DiceEffects>(diceEffectPrefabBlack) as DiceEffects;
            }
            else
            {
                dice1 = Instantiate<DiceEffects>(diceEffectPrefabWhite) as DiceEffects;
                dice2 = Instantiate<DiceEffects>(diceEffectPrefabWhite) as DiceEffects;
            }

            dice1.gameObject.transform.SetParent(dice1Transform.transform);
            dice2.gameObject.transform.SetParent(dice2Transform.transform);

            dice1.transform.localPosition = Vector3.zero;
            dice2.transform.localPosition = Vector3.zero;

            ShowDice(false);
            this.gameObject.SetActive(false);
        }

        public void SetPlayingLHS(bool playingLHS)
        {
            _playingLHS = playingLHS;
            _doublingCubeContainer.transform.position = new Vector3((_playingLHS ? 600 : -600), 0, 0);
        }

        public void DoublingPossible(bool doublingPossible)
        {
            if (doublingPossible)
            {
                _doublingCubeContainer.SetActive(true);
                _animateDoublingButton = true;
            }
        }

        public void SetDoublingCubePosition(Vector3 cubePosition)
        {
            _doublingCubeContainer.transform.position = cubePosition;
        }

        private void Update()
        {
            //if (_animateDoublingButton)
            //{
            //    _doublingButtonAnimationTimer -= Time.deltaTime;

            //    AnimateDoublingButtonScale((_doublingButtonAnimationTime - _doublingButtonAnimationTimer) * _doublingButtonAnimationSpeed,
            //                               (_doublingButtonAnimationTimer > (0.1f * _doublingButtonAnimationSpeed) ? 1 : -1));

            //    if (_doublingButtonAnimationTimer <= 0)
            //    {
            //        _animateDoublingButton = false;
            //        _doublingButtonAnimationTimer = _doublingButtonAnimationTime;
            //    }
            //}

            if (Animating)
            {
                // EVERY INTERVAL ALTER THE DICE ANIMATIONS BASED ON COUNTDOWN
                timer += Time.deltaTime;

                // SET MAGNITUDES / LIMITS FOR DICE ROTATIONS
                if (timer > updateRateTimer)
                {
                    updateRateTimer += updateRate;

                    dice1.AddTorque(ApplyTorque(1));
                    dice2.AddTorque(ApplyTorque(2));

                    dice1.AnimateScale(speedVec.x * 0.01f);
                    dice2.AnimateScale(speedVec.x * 0.01f);
                }

                if (timer >= totalAnimationTime)
                {
                    // SET DICE TO FINAL STATE
                    PlayerInteraction = true;
                    Animating = false;

                    this.gameObject.SetActive(false);
                }
            }
        }
        
        private void AnimateDoublingButtonScale(float speedVec, float dir)
        {
            _doublingCubeContainer.transform.localScale = Vector3.Lerp(
                                                            Vector3.one,
                                                            _doublingButtonAnimationScale,
                                                            speedVec * dir);
        }

        private Vector3 ApplyTorque(int dice = 1)
        {
            var torque = torqueVec.x * speedVec.x * invert;

            if (dice == 2)
                return new Vector3(torque * randomVec2.x,
                                torque * randomVec2.y,
                                torque * randomVec2.z);
            else
                return new Vector3(torque * randomVec1.x,
                                torque * randomVec1.y,
                                torque * randomVec1.z);
        }

        private void ResetDice()
        {
            dice1.ResetDice();
            dice2.ResetDice();
        }

        private void ShowDice(bool show)
        {
            dice1.gameObject.SetActive(show);
            dice2.gameObject.SetActive(show);
        }

        public void IfDisplayDemoText(bool displayDemoText, string playerName)
        {
            if (displayDemoText)
            {
                tapToRollDemoHeaderText.gameObject.SetActive(true);
                tapToRollDemoHeaderText.text = languageSO.Its + " " + playerName + languageSO.diceRollsDemoHeader;
                tapToRollText.text = languageSO.diceRollsDemoTapToRoll;
            }
            else
            {
                tapToRollDemoHeaderText.gameObject.SetActive(false);
                tapToRollText.text = languageSO.diceRollsTapToRoll;
            }
        }

        public void DisableClickToRoll(bool disable)
        {
            tapToRollDiceButton.interactable = !disable;
        }

        public void OnClickedDoubling()
        {
            ClickedDoubling = true;
        }

        public void OnDoublingComplete()
        {
            ClickedDoubling = false;
            _doublingCubeContainer.SetActive(false);
        }

        public void OnTapToRollDice()
        {
            IfDisplayDemoText(false, "");
            tapToRollText.gameObject.SetActive(false);
            _doublingCubeContainer.SetActive(false);
            dice1.SetDiceFaceBlank(false);
            dice2.SetDiceFaceBlank(false);
            Animating = true;
        }

        public void PlayingAs(bool _playingAsBlack)
        {
            playingAs = _playingAsBlack;
        }

        public bool Animating
        {
            get => animate;
            private set => animate = value;
        }

        public bool PlayerInteraction
        {
            get => playerInteraction;
            private set => playerInteraction = value;
        }

        private bool _playingLHS = false;
        private bool _clickedDoubling = false;

        private bool _animateDoublingButton = false;
        private float _doublingButtonAnimationTime = 1.5f;
        private float _doublingButtonAnimationTimer = 0f;
        private float _doublingButtonAnimationSpeed = 12f;
        private Vector3 _doublingButtonAnimationScale = new Vector3(1.4f, 1.4f, 1f);

        public bool ClickedDoubling 
        { 
            get => _clickedDoubling;
            private set => _clickedDoubling = value;
        }
    }
}