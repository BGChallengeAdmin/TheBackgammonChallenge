using UnityEngine;
using UnityEngine.UI;

namespace Backgammon
{
    public class ButtonHighlight : MonoBehaviour
    {
        Image image = null;
        Color defaultColour;

        protected void Awake()
        {
            image = GetComponent<Image>();
            defaultColour = image.color;
        }

        public void Select(bool ifOn = true)
        {
            if (image is null) return;

            float x = 0xbf / 255.0f;
            image.color = (ifOn ? new Color(0, 0, 1.0f) : new Color(x, x, x));
        }

        internal void SetColourOrDefault(Color? colour = null)
        {
            if (image is null) return;

            if (colour is null) image.color = defaultColour;
            else image.color = colour.Value;
        }
    }
}