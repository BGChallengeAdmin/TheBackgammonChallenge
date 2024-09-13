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

        [Header("CONTAINERS")]
        [SerializeField] Transform _buttonsContainerTransform;

		[Header("TEXT FIELDS")]
		[SerializeField] TMP_Text _generalInfoText;

        private void Awake()
        {
            _replayMoveButton.onClick.AddListener(() => OnReplayButtonClick());
            _continueButton.onClick.AddListener(() => OnContinueButtonClick());
        }

        private void OnDestroy()
        {
            _replayMoveButton.onClick.RemoveAllListeners();
            _continueButton.onClick.RemoveAllListeners();
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

        private void OnReplayButtonClick() { _ifReplayMove = true; }
        private void OnContinueButtonClick() { _ifContinue = true; }

        private bool _ifAwaitingPlayerInteraction = false;
        private bool _ifReplayMove = false;
        private bool _ifContinue = false;

        internal bool IfAwaitingPlayerInteraction { get => _ifAwaitingPlayerInteraction; }
        internal bool IfReplayMove { get => _ifReplayMove; }
        internal bool IfContinue { get => _ifContinue; }
    }
}