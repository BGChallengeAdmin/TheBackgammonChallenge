using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Backgammon
{
    public class BoardDesignerUI : MonoBehaviour
    {
        [Header("BUTTONS")]
        [SerializeField] Button _changeBoardDownButton = null;
        [SerializeField] Button _changeBoardUpButton = null;
        [SerializeField] Button _exitButton = null;

        [Header("LIGHTING DEBUG")]
        [SerializeField] Button _lightsDownLargeButton = null;
        [SerializeField] Button _lightsDownButton = null;
        [SerializeField] Button _lightsUpButton = null;
        [SerializeField] Button _lightsUpLargeButton = null;
        [SerializeField] TMP_Text _lightingLabelText = null;
        [SerializeField] Light[] _lights = null;

        private void Awake()
        {
            _boardIndex = Main.BoardDesignSOIndex;
            _maxNumberOfBoardSO = Main.Instance.MaxNumberOfBoardSO;

            _changeBoardDownButton.onClick.AddListener(() => ChangeBoard(-1));
            _changeBoardUpButton.onClick.AddListener(() => ChangeBoard(1));
            _exitButton.onClick.AddListener(() => OnClickExit());

            _lightsDownLargeButton.onClick.AddListener(() => AdjustLighting(-.5f));
            _lightsDownButton.onClick.AddListener(() => AdjustLighting(-.1f));
            _lightsUpButton.onClick.AddListener(() => AdjustLighting(+.1f));
            _lightsUpLargeButton.onClick.AddListener(() => AdjustLighting(+.5f));
        }

        private void OnEnable()
        {
            _ifClickedExit = false;

            _lightingIntensity = _lights[0].intensity;
            _lightingLabelText.text = _lightingIntensity.ToString();
        }

        private void OnDestroy()
        {
            _changeBoardDownButton.onClick.RemoveAllListeners();
            _changeBoardUpButton.onClick.RemoveAllListeners();
            _exitButton.onClick.RemoveAllListeners();

            _lightsDownButton.onClick.RemoveAllListeners();
            _lightsUpButton.onClick.RemoveAllListeners();
        }

        private void ChangeBoard(int change)
        {
            _boardIndex += change;

            if (_boardIndex < 0) _boardIndex = _maxNumberOfBoardSO - 1;
            else if (_boardIndex >= _maxNumberOfBoardSO) _boardIndex = 0;

            // SET BUTTONS INACTIVE TO ALLOW BOARD TO LOAD
            _changeBoardDownButton.interactable = false;
            _changeBoardUpButton.interactable = false;
            StartCoroutine(ReactiveButtonsCoroutine());

            Main.BoardDesignSOIndex = _boardIndex;
            Game.Context.BoardMaterialsManager.Init(Main.Instance.BoardDesignSO);
        }

        private IEnumerator ReactiveButtonsCoroutine()
        {
            yield return new WaitForSeconds(1f);

            _changeBoardDownButton.interactable = true;
            _changeBoardUpButton.interactable = true;
        }

        private void OnClickExit()
        {
            _ifClickedExit = true;
        }

        private void AdjustLighting(float adjust)
        {
            _lightingIntensity += adjust;
            if (_lightingIntensity <= 0) _lightingIntensity = 0;

            _lightingIntensity = Mathf.Round(_lightingIntensity * 10.0f) * 0.1f;

            foreach (Light light in _lights) light.intensity = _lightingIntensity;

            _lightingLabelText.text = _lightingIntensity.ToString();
        }

        private int _boardIndex = 0;
        private int _maxNumberOfBoardSO = 0;
        private bool _ifClickedExit = false;

        private float _lightingIntensity = 3;

        public bool IfClickedExit { get => _ifClickedExit; }
    }
}