using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Backgammon
{
    public class ObserveBoardUI : MonoBehaviour
    {
        [Header("TEXT FIELDS")]
        [SerializeField] TMP_Text _observeInfoText = null;

        [Header("BUTTONS")]
        [SerializeField] Button _continueButton = null;

        private void Awake()
        {
            _continueButton.onClick.AddListener(() => OnClickContinue());
        }

        private void OnEnable()
        {
            _clickedContinue = false;
        }

        private void OnDestroy()
        {
            _continueButton.onClick.RemoveAllListeners();
        }

        internal void SetActive(bool active)
        {
            this.gameObject.SetActive(active);
        }

        internal void SetObserveText(string text)
        {
            _observeInfoText.text = text;
        }

        private void OnClickContinue()
        {
            _clickedContinue = true;
        }

        // ----------------------------------------- GETTERS && SETTERS -----------------------------------------

        private bool _clickedContinue = false;
        public bool ClickedContinue { get => _clickedContinue; }
    }
}