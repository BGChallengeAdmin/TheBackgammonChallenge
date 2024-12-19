using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Backgammon
{
    public class StatisticsResetUI : MonoBehaviour
    {
        [SerializeField] private Text _resetText = null;
        [SerializeField] private Text _partialResetButtonText = null;
        [SerializeField] private Text _fullResetButtonText = null;
        [SerializeField] private Text _backButtonText = null;

        private void OnEnable()
        {
            LanguageScriptableObject languageSO = Main.Instance.WorldRegionObj.LanguageSO;

            if (languageSO != null ) 
            {
                var resetText = languageSO.resetLine1 + "\n\nA";
                resetText += " '" + languageSO.resetPartial + "' " + languageSO.resetLine2 + "\n\nA";
                resetText += " '" + languageSO.resetFull + "' " + languageSO.resetLine3 + "\n";
                resetText += languageSO.resetLine4;
                _resetText.text = resetText;

                _partialResetButtonText.text = languageSO.resetPartial;
                _fullResetButtonText.text = languageSO.resetFull;
                _backButtonText.text = languageSO.Back;
            }

            Back = false;
            OK = false;
            PartialReset = false;
            FullReset = false;
        }

        public void OnClickBack()
        {
            Back = true;
        }

        public void OnClickOK()
        {
            OK = true;
        }

        public void OnClickPartialReset()
        {
            PartialReset = true;
        }

        public void OnClickFullReset()
        {
            FullReset = true;
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

        public bool PartialReset
        {
            get => paritalReset;
            set => paritalReset = value;
        }

        public bool FullReset
        {
            get => fullReset;
            set => fullReset = value;
        }

        private bool back = false;
        private bool ok = false;
        private bool paritalReset = false;
        private bool fullReset = false;
    }
}
