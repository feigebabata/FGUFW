using UnityEditor;
using UnityEngine;

namespace FGUFW.Editor
{
    public class StringInputWindow : EditorWindow
    {
        private string value = "";
        private System.Action<string> onConfirm;
        private bool firstFocus = true;

        public static void Show( string title, string defaultValue, System.Action<string> onConfirm)
        {
            var win = CreateInstance<StringInputWindow>();
            win.titleContent = new GUIContent(title);
            win.value = defaultValue;
            win.onConfirm = onConfirm;
            
            var size = new Vector2(300,90);
            // win.position = new Rect( Screen.width / 2, Screen.height / 2, 300, 90);
            win.minSize = size;
            win.maxSize = size;
            win.ShowUtility(); // 小工具窗口
        }

        private void OnGUI()
        {
            GUILayout.Space(5);

            GUI.SetNextControlName("TextField");
            value = EditorGUILayout.TextField(value);
            if (firstFocus)
            {
                EditorGUI.FocusTextInControl("TextField");
                firstFocus = false;
            }

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Confirm", GUILayout.Width(295), GUILayout.Height(55)))
            {
                onConfirm?.Invoke(value);
                Close();
            }
            GUILayout.EndHorizontal();
        }
    }
}
