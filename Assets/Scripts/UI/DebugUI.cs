using TMPro;
using UnityEngine;

namespace ClumsyBat.UI
{
    public class DebugUI : MonoBehaviour
    {
#pragma warning disable 649
        [SerializeField] private TextMeshProUGUI text;
#pragma warning disable 649

        private void Awake()
        {
            SetText("");
        }

        public void SetText(string message)
        {
            text.text = message;
        }
    }
}