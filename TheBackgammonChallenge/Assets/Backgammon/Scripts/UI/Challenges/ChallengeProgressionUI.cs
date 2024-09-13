using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace Backgammon
{
    public class ChallengeProgressionUI : MonoBehaviour
    {
        [Header("TEXT FIELDS")]
        [SerializeField] TMP_Text _proChallengeButtonText;
        [SerializeField] TMP_Text _aiChallengeMediumButtonText;
        [SerializeField] TMP_Text _aiChallengeHard1ButtonText;
        [SerializeField] TMP_Text _aiChallengeHard2ButtonText;

        [Header("BUTTONS")]
        [SerializeField] Button _proChallengeButton;
        [SerializeField] Button _aiChallengeMediumButton;
        [SerializeField] Button _aiChallengeHard1Button;
        [SerializeField] Button _aiChallengeHard2Button;

        [Header("SLIDER")]
        [SerializeField] Slider _progressSlider;
        [SerializeField] Transform _sliderHandler;

        [Header("TRANSFORMS")]
        [SerializeField] Transform _proChallengeMilestone;
        [SerializeField] Transform _mediumChallengeMilestone;
        [SerializeField] Transform _hard1ChallengeMilestone;
        [SerializeField] Transform _hard2ChallengeMilestone;

        private void Awake()
        {
            _proChallengeButton.onClick.AddListener(() => OnProChallengeButtonClick());
            _aiChallengeMediumButton.onClick.AddListener(() => OnMediumChallengeButtonClick());
            _aiChallengeHard1Button.onClick.AddListener(() => OnHardChallenge1ButtonClick());
            _aiChallengeHard2Button.onClick.AddListener(() => OnHardChallenge2ButtonClick());

            _proChallengeButtonText.text = "PRO Challenge";
            _aiChallengeMediumButtonText.text = "A.I. MEDIUM";
            _aiChallengeHard1ButtonText.text = "A.I. DIFFICULT";
            _aiChallengeHard2ButtonText.text = "A.I. HARD";

            SetAllMiestonesInactive();
        }

        private void OnDestroy()
        {
            _proChallengeButton.onClick.RemoveAllListeners();
            _aiChallengeMediumButton.onClick.RemoveAllListeners();
            _aiChallengeHard1Button.onClick.RemoveAllListeners();
            _aiChallengeHard2Button.onClick.RemoveAllListeners();
        }

        internal void Init()
        {
            _proChallengeButton.interactable = false;
            _aiChallengeMediumButton.interactable = false;
            _aiChallengeHard1Button.interactable = false;
            _aiChallengeHard2Button.interactable = false;
        }

        internal void SetActive(bool active)
        {
            this.gameObject.SetActive(active);
        }

        // BUTTONS
        private void OnProChallengeButtonClick() 
        {
            SetAllMiestonesInactive();

            var current = _progressSlider.value;
            if (current > .085f)
            {
                _progressSlider.SetValueWithoutNotify(.085f);
                TestMilestones();
            }
            else
                StartCoroutine(SetSliderValueCoroutine(.085f));        
        }

        private void OnMediumChallengeButtonClick()
        {
            SetAllMiestonesInactive();

            var current = _progressSlider.value;
            if (current > .4f)
            {
                _progressSlider.SetValueWithoutNotify(.4f);
                TestMilestones();
            }
            else
                StartCoroutine(SetSliderValueCoroutine(.4f)); 
        }

        private void OnHardChallenge1ButtonClick()
        {
            SetAllMiestonesInactive();

            var current = _progressSlider.value;
            if (current > .7f)
            {
                _progressSlider.SetValueWithoutNotify(.7f);
                TestMilestones();
            }
            else
                StartCoroutine(SetSliderValueCoroutine(.7f));
        }

        private void OnHardChallenge2ButtonClick()
        {
            StartCoroutine(SetSliderValueCoroutine(1f));
        }

        // SLIDER

        internal void SetSliderValue(float value)
        {
            StartCoroutine(SetSliderValueCoroutine(value));
        }

        private IEnumerator SetSliderValueCoroutine(float value)
        {
            var current = _progressSlider.value;

            while (current <= value)
            {
                current += .01f;
                _progressSlider.SetValueWithoutNotify(current);
                TestMilestones();

                yield return new WaitForSeconds(.01f);
            }
        }

        private void TestMilestones()
        {
            if (!_proChallengeMilestone.gameObject.activeInHierarchy)
                if (_sliderHandler.position.x > _proChallengeMilestone.gameObject.transform.position.x)
                {
                    _proChallengeMilestone.gameObject.SetActive(true);
                    _proChallengeButton.interactable = true;
                }

            if (!_mediumChallengeMilestone.gameObject.activeInHierarchy)
                if (_sliderHandler.position.x > _mediumChallengeMilestone.gameObject.transform.position.x)
                {
                    _mediumChallengeMilestone.gameObject.SetActive(true);
                    _aiChallengeMediumButton.interactable = true;
                }

            if (!_hard1ChallengeMilestone.gameObject.activeInHierarchy)
                if (_sliderHandler.position.x > _hard1ChallengeMilestone.gameObject.transform.position.x)
                {
                    _hard1ChallengeMilestone.gameObject.SetActive(true);
                    _aiChallengeHard1Button.interactable = true;
                }

            if (!_hard2ChallengeMilestone.gameObject.activeInHierarchy)
                if (_sliderHandler.position.x > _hard2ChallengeMilestone.gameObject.transform.position.x)
                {
                    _hard2ChallengeMilestone.gameObject.SetActive(true);
                    _aiChallengeHard2Button.interactable = true;
                }
        }

        private void SetAllMiestonesInactive()
        {
            _proChallengeMilestone.gameObject.SetActive(false);
            _mediumChallengeMilestone.gameObject.SetActive(false);
            _hard1ChallengeMilestone.gameObject.SetActive(false);
            _hard2ChallengeMilestone.gameObject.SetActive(false);
        }
    }
}