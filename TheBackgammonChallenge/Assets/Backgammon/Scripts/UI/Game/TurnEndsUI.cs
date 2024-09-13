using UnityEngine;
using TMPro;

namespace Backgammon
{
    public class TurnEndsUI : MonoBehaviour
    {
        [Header("TEXT FIELDS")]
        [SerializeField] TMP_Text _turnEndsText;

        internal void SetActive(bool active)
        {
            this.gameObject.SetActive(active);
        }
    }
}