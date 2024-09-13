using System.Collections;
using TMPro;
using UnityEngine;

namespace Backgammon
{
    public class GameWonAnimationPrefab : MonoBehaviour
    {
        [Header("TRANSFORMS")]
        [SerializeField] Transform _highlightTransform;

        [Header("TEXT FIELDS")]
        [SerializeField] TMP_Text _pointsScoredText;

        internal void SetActive(bool active)
        {
            this.gameObject.SetActive(active);
            if(!active) _highlightTransform.gameObject.SetActive(active);
        }

        internal void SetPointsScoredText(Vector3 position, int points, float highlightDelaySeconds)
        {
            this.transform.position = position;
            SetActive(true);

            _pointsScoredText.text = points.ToString();

            StartCoroutine(DelayHighlightCortoutine(highlightDelaySeconds));
        }

        private IEnumerator DelayHighlightCortoutine(float highlightDelay)
        {
            yield return new WaitForSeconds(highlightDelay);

            _highlightTransform.gameObject.SetActive(true);
        }

        internal void Reset()
        {
            _pointsScoredText.text = "0";
            SetActive(false);
        }
    }
}