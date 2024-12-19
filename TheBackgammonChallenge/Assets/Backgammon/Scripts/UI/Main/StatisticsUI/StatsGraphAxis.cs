using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsGraphAxis : MonoBehaviour
{
    public Text fromText;
    public Text toText;

    public void SetGraphAxisText(float fromVal, float toVal, bool setTextHorizontal = false, float rotation = 0f)
    {
        fromText.text = fromVal >= 0 ? fromVal.ToString("F0") : string.Empty;
        toText.text = toVal >= 1000 ? ((toVal / 100).ToString("F0")  + "00") : toVal.ToString("F0");
        
        if (setTextHorizontal)
        {
            fromText.GetComponentInParent<RectTransform>().transform.localRotation = Quaternion.Euler(0f, 0f, rotation);
            toText.GetComponentInParent<RectTransform>().transform.localRotation = Quaternion.Euler(0f, 0f, rotation);

            if (rotation > 0f)
            {
                // BAR HAS BEEN ROTATED - SWITCH TEXT VALUES
                toText.text = fromVal >= 0 ? fromVal.ToString("F0") : string.Empty;
                fromText.text = toVal >= 1000 ? ((toVal / 100).ToString("F0") + "00") : toVal.ToString("F0");
            }

            var horizPosOffset = -10f;
            var fromPos = fromText.GetComponentInParent<RectTransform>().transform.localPosition;
            var toPos = toText.GetComponentInParent<RectTransform>().transform.localPosition;

            fromText.GetComponentInParent<RectTransform>().transform.localPosition = new Vector3(fromPos.x, fromPos.y + horizPosOffset);
            toText.GetComponentInParent<RectTransform>().transform.localPosition = new Vector3(toPos.x, toPos.y + horizPosOffset);
        }
    }
}
