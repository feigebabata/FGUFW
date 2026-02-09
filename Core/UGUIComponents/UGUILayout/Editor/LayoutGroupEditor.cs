// using UnityEngine;
// using UnityEditor;

// namespace FGUFW.UGUILayout.Editor
// {
//     [CustomEditor(typeof(LayoutGroup))]
//     public class LayoutGroupEditor : UnityEditor.Editor
//     {
//         private LayoutGroup _target;

//         void OnEnable()
//         {
//             _target = target as LayoutGroup;    
//         }

//         public override void OnInspectorGUI()
//         {
//             base.OnInspectorGUI();
            
//             if(GUILayout.Button("Execute"))
//             {
//                 _target.Execute();
//             }
//         }
//     }
// }