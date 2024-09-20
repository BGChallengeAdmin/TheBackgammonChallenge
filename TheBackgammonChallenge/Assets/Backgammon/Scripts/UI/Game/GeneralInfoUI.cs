using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Backgammon
{
	public class GeneralInfoUI : MonoBehaviour
	{
        [Header("BUTTONS")]
        [SerializeField] Button _replayMoveButton;
        [SerializeField] Button _continueButton;
        [SerializeField] Button _debugContinueButton;

        [Header("CONTAINERS")]
        [SerializeField] Transform _buttonsContainerTransform;

		[Header("TEXT FIELDS")]
		[SerializeField] TMP_Text _generalInfoText;

        private void Awake()
        {
            _replayMoveButton.onClick.AddListener(() => OnReplayButtonClick());
            _continueButton.onClick.AddListener(() => OnContinueButtonClick());
            _debugContinueButton.onClick.AddListener(() => OnDebugContinueButtonClick());
        }

        private void OnDestroy()
        {
            _replayMoveButton.onClick.RemoveAllListeners();
            _continueButton.onClick.RemoveAllListeners();
            _debugContinueButton.onClick.RemoveAllListeners();
        }

        internal void SetActive(bool active)
        {
            this.gameObject.SetActive(active);

            _buttonsContainerTransform.gameObject.SetActive(false);
        }

        internal void SetGeneralText(string text)
        {
            _generalInfoText.text = string.Empty;
            _generalInfoText.text = text;
        }

        // ------------------------------------ BUTTONS ----------------------------------

        internal void SetPlayerReplayMoveOptionActive(bool active)
        {
            _buttonsContainerTransform.gameObject.SetActive(active);
            _ifAwaitingPlayerInteraction = active;
            _ifReplayMove = false;
            _ifContinue = false;
        }

        internal void SetDebugContinueButtonActive(bool active)
        {
            _debugContinueButton.transform.parent.gameObject.SetActive(active);
            _ifDebugContinue = false;
        }

        private void OnReplayButtonClick() { _ifReplayMove = true; }
        private void OnContinueButtonClick() { _ifContinue = true; }
        private void OnDebugContinueButtonClick() { _ifDebugContinue = true; }

        private bool _ifAwaitingPlayerInteraction = false;
        private bool _ifReplayMove = false;
        private bool _ifContinue = false;
        private bool _ifDebugContinue = false;

        internal bool IfAwaitingPlayerInteraction { get => _ifAwaitingPlayerInteraction; }
        internal bool IfReplayMove { get => _ifReplayMove; }
        internal bool IfContinue { get => _ifContinue; }
        internal bool IfDebugContinue { get => _ifDebugContinue; }
    }
}