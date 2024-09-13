using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Backgammon
{
	public class BeforeCommenceUI : MonoBehaviour
	{
		[Header("TEXT FIELDS")]
		[SerializeField] TMP_Text _beforeCommenceText;
        [SerializeField] TMP_Text _beforeCommenceInputField;

		[Header("BUTTONS")]
		[SerializeField] Button _commenceButton;

        private void Awake()
        {
            _commenceButton.onClick.AddListener(() => OnCommenceButtonClick());
        }

        private void OnEnable()
        {
            _commenceClicked = false;
            _fastForward = 0;
        }

        private void OnDestroy()
        {
            _commenceButton.onClick.RemoveAllListeners();
        }

        // --------------------------------------- HELPER METHODS --------------------------------------

        internal void SetActive(bool active)
		{
			this.gameObject.SetActive(active);
		}

		internal void SetCommenceText(string text)
		{
			_beforeCommenceText.text = text;
		}

        internal void OnCommenceButtonClick()
        {
            //var index = _beforeCommenceInputField.text;
            //if (index != string.Empty || index != "" || index is not null) _fastForward = int.Parse(index);

            _commenceClicked = true;
        }

        // -------------------------------------- GETTERS && SETTERS --------------------------------------

        private bool _commenceClicked = false;
        private int _fastForward = 0;

        internal bool CommenceClicked { get => _commenceClicked; }
        internal int FastForwardUserInput { get => _fastForward; }
    }
}