using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using FGUFW.Editor;
using System.Linq;

namespace FGUFW.ULogEditor
{
    public static class ULogEditor
    {
        const string logEnabled = "Conditional/ULog/Enabled";

        [MenuItem(logEnabled)]
        public static void ULogEnable()
        {
            bool enabled = Menu.GetChecked(logEnabled);

            var defines = EditorUtil.GetScriptingDefineSymbols().ToList();

            if (!enabled)
            {
                defines.Add(ULog.Conditional_Log);
                defines.Add("UNITY_ASSERTIONS");
            }
            else
            {

                defines.Remove(ULog.Conditional_Log);
                defines.Remove("UNITY_ASSERTIONS");
            }

            EditorUtil.SetScriptingDefineSymbols(defines.ToArray());

            Menu.SetChecked(logEnabled, !enabled);
        }

        [MenuItem(logEnabled, true)]
        public static bool ULogEnableCheck()//先刷新状态
        {
            string[] defines = EditorUtil.GetScriptingDefineSymbols();

            var enabled = defines.IndexOf(ULog.Conditional_Log) != -1;

            Menu.SetChecked(logEnabled, enabled);
            return true;
        }

        //------------------------------------------------------------------------------
        
        const string IgnoreLog = "Conditional/ULog/Ignore LogType.Log"; 

        [MenuItem(IgnoreLog)]  
        public static void ULogIgnoreLog()  
        {  
            bool enabled = Menu.GetChecked(IgnoreLog);  

            var defines = EditorUtil.GetScriptingDefineSymbols().ToList();

            if(!enabled)
            {
                defines.Add(ULog.Conditional_IgnoreLog);
            }
            else
            {

                defines.Remove(ULog.Conditional_IgnoreLog);
            }

            EditorUtil.SetScriptingDefineSymbols(defines.ToArray());

            Menu.SetChecked(IgnoreLog, !enabled);  
        }  

        [MenuItem(IgnoreLog,true)]  
        public static bool ULogIgnoreLogCheck()//先刷新状态
        {  
            string[] defines = EditorUtil.GetScriptingDefineSymbols();

            var enabled = defines.IndexOf(ULog.Conditional_IgnoreLog) != -1;

            Menu.SetChecked(IgnoreLog, enabled);  
            return true;  
        }

    }
}