using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Backgammon
{
    public class StatisticsResetUIConfirm : MonoBehaviour
    {
        [SerializeField] private Text _resetConfirmText = null;
        [SerializeField] private Text _resetConfirmYesText = null;
        [SerializeField] private Text _resetConfirmNoText = null;

        private void OnEnable()
        {
            LanguageScriptableObject languageSO = Main.Instance.WorldRegionObj.LanguageSO;

            if (languageSO != null)
            {
                _resetConfirmText.text = languageSO.resetConfirmText;
                _resetConfirmYesText.text = languageSO.Yes;
                _resetConfirmNoText.text = languageSO.Back;
            }

            Back = false;
            OK = false;
        }

        public void OnClickBack()
        {
            Back = true;
        }

        public void OnClickOK()
        {
            OK = true;
        }

        // GETTERS && SETTERS

        public bool Back
        {
            get => back;
            set => back = value;
        }

        public bool OK
        {
            get => ok;
            set => ok = value;
        }

        private bool back = false;
        private bool ok = false;
    }
}