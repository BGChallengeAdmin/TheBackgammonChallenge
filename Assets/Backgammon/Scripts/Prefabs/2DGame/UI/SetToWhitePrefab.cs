using UnityEngine;
using UnityEngine.UI;

public class SetToWhitePrefab : MonoBehaviour
{
    [SerializeField] Image _image;

    internal void SetToWhite()
    {
        _image.color = Color.white;
    }
}
