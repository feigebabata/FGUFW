using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using FGUFW.Editor;
using System.Linq;

namespace FGUFW.Editor
{
    public static class TestSDS
    {
        //-------------------------------------------------------------------------------

        const string testEnabled = "Conditional/TEST_ENABLED";
        const string testSDS = "TEST_ENABLED";
        [MenuItem(testEnabled)]
        public static void ULogEnable()
        {
            bool enabled = Menu.GetChecked(testEnabled);

            var defines = EditorUtil.GetScriptingDefineSymbols().ToList();

            if (!enabled)
            {
                defines.Add(testSDS);
            }
            else
            {

                defines.Remove(testSDS);
            }

            EditorUtil.SetScriptingDefineSymbols(defines.ToArray());

            Menu.SetChecked(testEnabled, !enabled);
        }
        [MenuItem(testEnabled, true)]
        public static bool MenuLogOutCheck()//先刷新状态
        {
            string[] defines = EditorUtil.GetScriptingDefineSymbols();

            var enabled = defines.IndexOf(testSDS) != -1;

            Menu.SetChecked(testEnabled, enabled);
            return true;
        }

    }
}