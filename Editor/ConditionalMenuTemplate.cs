// using System.Linq;
// using FGUFW;
// using FGUFW.Editor;
// using UnityEditor;

// static class ConditionalMenuTemplate
// {
//     const string menuPath = "Conditional/menu name"; //修改
//     const string conditional = "conditional"; //修改
    
//     [MenuItem(menuPath)]
//     static void ConditionalEnable()
//     {
//         bool enabled = Menu.GetChecked(menuPath);

//         var defines = EditorUtil.GetScriptingDefineSymbols().ToList();

//         if (!enabled)
//         {
//             defines.Add(conditional);
//         }
//         else
//         {

//             defines.Remove(conditional);
//         }

//         EditorUtil.SetScriptingDefineSymbols(defines.ToArray());

//         Menu.SetChecked(menuPath, !enabled);
//     }

//     [MenuItem(menuPath, true)]
//     static bool MenuConditionalCheck()//先刷新状态
//     {
//         string[] defines = EditorUtil.GetScriptingDefineSymbols();

//         var enabled = defines.IndexOf(conditional) != -1;

//         Menu.SetChecked(menuPath, enabled);
//         return true;
//     }
// }