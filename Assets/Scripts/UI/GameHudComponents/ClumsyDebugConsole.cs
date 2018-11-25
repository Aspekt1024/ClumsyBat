using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ClumsyBat.UI.GameHudComponents
{
    public class ClumsyDebugConsole : MonoBehaviour
    {
        public int LogSize = 5;
        public Text DebugConsoleText;

        private static ClumsyDebugConsole instance;

        private string[] messages;
        private int index = 0;
        private StringBuilder sb = new StringBuilder();

        public void Start()
        {
            if (instance != null)
            {
                Debug.LogError($"Multiple {nameof(DebugConsole)}s detected in scene. There should only be one!");
                return;
            }
            instance = this;
            messages = new string[LogSize];

            if (GameStatics.GameManager.DebugMode)
            {
                Log("Debug log activated");
            }
            else
            {
                DebugConsoleText.gameObject.SetActive(false);
            }
        }

        public static void Log(string message)
        {
            if (!GameStatics.GameManager.DebugMode) return;
            instance.LogMessage(message);
        }

        private void LogMessage(string message)
        {
            messages[index] = message;
            UpdateDebugMessages();

            index++;
            if (index == LogSize)
            {
                index = 0;
            }
        }

        private void UpdateDebugMessages()
        {
            sb.Clear();
            for (int i = 0; i < LogSize; i++)
            {
                int cycledIndex = index + i + 1;
                if (cycledIndex >= LogSize)
                {
                    cycledIndex -= LogSize;
                }
                if (string.IsNullOrEmpty(messages[cycledIndex])) continue;
                sb.AppendLine(messages[cycledIndex]);
            }
            DebugConsoleText.text = sb.ToString();
        }
    }
}
