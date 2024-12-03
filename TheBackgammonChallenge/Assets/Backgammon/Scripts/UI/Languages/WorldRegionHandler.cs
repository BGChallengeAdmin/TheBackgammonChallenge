using System.Collections.Generic;
using UnityEngine;

namespace Backgammon
{
    public class WorldRegionHandler : MonoBehaviour
    {
        [SerializeField] LanguageScriptableObject[] _languageSOArray = null;
        [SerializeField] Sprite[] _worldFlagSprites = null;

        protected void Awake()
        {
            _languageSOByNameDict = new Dictionary<string, LanguageScriptableObject>();
            _flagIndexByNameDict = new Dictionary<string, int>();

            for (var language = 0; language < _languageSOArray.Length; language++)
            {
                _languageSOByNameDict.Add(_languageSOArray[language].shortCode, _languageSOArray[language]);
            }

            for (var country = 0; country < _worldFlagSprites.Length; country++)
            {
                _flagIndexByNameDict.Add(_worldFlagSprites[country].name, country);
            }
        }

        public void TrySetLanguageSOFromShortCode(string shortCode)
        {
            if (_languageSOByNameDict.ContainsKey(shortCode))
            {
                _languageShortCode = shortCode;
                _selectedLanguageSO = _languageSOByNameDict[shortCode];
            }
            else
            {
                _languageShortCode = "en_GB";
                _selectedLanguageSO = GetLanguageSOByShortName(_languageShortCode);
            }
        }

        private LanguageScriptableObject GetLanguageSOByShortName(string name)
        {
            LanguageScriptableObject language;
            return _languageSOByNameDict.TryGetValue(name, out language) ? language : null;
        }

        public Sprite GetFlagByCountryName(string countryName)
        {
            // PARSE TO LOWER CASE - NO SPACES
            if (countryName != null && countryName != "" && countryName != string.Empty)
            {
                var country = countryName.ToLower();
                country = country.Replace(" ", "");

                var countryIndex = -1;

                if (_flagIndexByNameDict.ContainsKey(country))
                    countryIndex = _flagIndexByNameDict[country];

                if (countryIndex >= 0) return _worldFlagSprites[countryIndex];
            }

            return null;
        }

        public string LanguageShortCode { get => _languageShortCode; set => _languageShortCode = value; }
        public LanguageScriptableObject LanguageSO { get => _selectedLanguageSO; }

        private string _languageShortCode = string.Empty;
        private LanguageScriptableObject _selectedLanguageSO = null;
        private Dictionary<string, LanguageScriptableObject> _languageSOByNameDict = null;
        private Dictionary<string, int> _flagIndexByNameDict = null;
    }
}