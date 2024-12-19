using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Backgammon
{
    public class DemoUI : MonoBehaviour
    {
        [Header("CENTER PANEL")]
        [SerializeField] Transform _popupCenter;
        [SerializeField] TextMeshProUGUI _textCenter;
        [SerializeField] Button _continueButtonCenter;
        [SerializeField] Button _backButtonCenter;

        [Header("LEFT PANEL")]
        [SerializeField] RectTransform _popupLeft;
        [SerializeField] TextMeshProUGUI _textLeft;
        [SerializeField] Button _continueButtonLeft;
        [SerializeField] Button _backButtonLeft;

        private Vector2 _defaultLeftTransformPositionMin;
        private Vector2 _defaultLeftTransformPositionMax;

        private LanguageScriptableObject languageSO = null;

        protected void Awake()
        {
            languageSO = Main.Instance.WorldRegionObj.LanguageSO;

            _defaultLeftTransformPositionMin = _popupLeft.anchorMin;
            _defaultLeftTransformPositionMax = _popupLeft.anchorMax;
        }

        protected void OnEnable()
        {
            _continueButtonCenter.onClick.AddListener(() => OnClickContinue());
            _continueButtonLeft.onClick.AddListener(() => OnClickContinue());

            _backButtonCenter.onClick.AddListener(() => OnClickBack());
            _backButtonLeft.onClick.AddListener(() => OnClickBack());

            if (Game2D.Context.IfPlayFromLhs)
            {
                _popupLeft.anchorMin = _defaultLeftTransformPositionMin;
                _popupLeft.anchorMax = _defaultLeftTransformPositionMax;}
            else
            {
                _popupLeft.anchorMin = new Vector2((1 - _defaultLeftTransformPositionMax.x), _defaultLeftTransformPositionMin.y);
                _popupLeft.anchorMax = new Vector2((1 - _defaultLeftTransformPositionMin.x), _defaultLeftTransformPositionMax.y);
            }
        }

        protected void OnDisable()
        {
            _continueButtonCenter.onClick.RemoveAllListeners();
            _continueButtonLeft.onClick.RemoveAllListeners();

            _backButtonCenter.onClick.RemoveAllListeners();
            _backButtonLeft.onClick.RemoveAllListeners();
        }

        internal void SetActive(bool enable)
        {
            this.gameObject.SetActive(enable);
        }

        internal void SetCenterTextPanel(string text)
        {
            _popupCenter.gameObject.SetActive(true);
            _textCenter.text = text;
        }

        internal void SetLeftTextPanel(string text)
        {
            _popupLeft.gameObject.SetActive(true);
            _textLeft.text = text;
        }

        internal void DisableTextPanels()
        {
            _popupCenter.gameObject.SetActive(false);
            _popupLeft.gameObject.SetActive(false);
        }

        private void OnClickContinue()
        {
        }

        private void OnClickBack()
        {
        }
    }
}