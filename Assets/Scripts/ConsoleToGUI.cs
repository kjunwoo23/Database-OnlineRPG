using UnityEngine;

namespace DebugStuff
{
    public class ConsoleToGUI : MonoBehaviour
    {
        //#if !UNITY_EDITOR
        static string myLog = "";

        bool tab;
        private string output;
        private string stack;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
                if (tab) tab = false;
                else tab = true;
        }
        void OnEnable()
        {
            Application.logMessageReceived += Log;
        }

        void OnDisable()
        {
            Application.logMessageReceived -= Log;
        }

        public void Log(string logString, string stackTrace, LogType type)
        {
            output = logString;
            stack = stackTrace;
            myLog = output + "\n" + myLog;
            if (myLog.Length > 5000)
            {
                myLog = myLog.Substring(0, 4000);
            }
        }

        void OnGUI()
        {
            //if (!Application.isEditor) //Do not display in editor ( or you can use the UNITY_EDITOR macro to also disable the rest)
            {
                if (tab)
                    myLog = GUI.TextArea(new Rect(0, Screen.height * 0.7f, Screen.width * 0.83f, Screen.height * 0.3f), myLog);
            }
        }
        //#endif
    }
}