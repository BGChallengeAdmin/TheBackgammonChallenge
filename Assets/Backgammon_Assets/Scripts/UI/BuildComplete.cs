using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Backgammon_Asset
{
    public class BuildComplete : MonoBehaviour
    {
        public Text headerText = null;

        private void Start()
        {
            headerText.text = "";
        }

        public void SetHeaderText(string header)
        {
            headerText.text = header;
        }
    }
}