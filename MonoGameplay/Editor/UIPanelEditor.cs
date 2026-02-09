using UnityEditor;
using UnityEngine;

namespace FGUFW.MonoGameplay.Editor
{
    [CustomEditor(typeof(UIPanel))]
    public class UIPanelEditor : UnityEditor.Editor
    {
        private UIPanel _target;
        private SerializedProperty _alphaCurve;
        private SerializedProperty _scaleCurve;
        private SerializedProperty _moveCurve;
        private SerializedProperty _moveVector;
        private SerializedProperty _switingTime;
        private SerializedProperty _panelSortOrder;

        void OnEnable()
        {
            _target = target as UIPanel;

            _alphaCurve = serializedObject.FindProperty("AlphaCurve");
            _scaleCurve = serializedObject.FindProperty("ScaleCurve");
            _moveCurve = serializedObject.FindProperty("MoveCurve");
            _moveVector = serializedObject.FindProperty("MoveVector");
            _switingTime = serializedObject.FindProperty("SwitingTime");
            _panelSortOrder = serializedObject.FindProperty("SortOrder");
        }

        void OnDisable()
        {
            _target = default;
        }

        public override void OnInspectorGUI()
        {

            UIPanelSortOrder orderEnum = (UIPanelSortOrder)0;
            try
            {
                orderEnum = _target.SortOrder.ToEnum<UIPanelSortOrder>();
            }
            catch (System.Exception)
            {
            }

            var order = EditorGUILayout.EnumPopup("SortOrder", orderEnum).ts();

            _target.SwitingEffect = (UIPanel.Effect)EditorGUILayout.EnumFlagsField("Switing",_target.SwitingEffect);

            base.OnInspectorGUI();

            if(_target.SwitingEffect != UIPanel.Effect.Nothing)
            {
                EditorGUILayout.PropertyField(_switingTime);
            }

            if((_target.SwitingEffect & UIPanel.Effect.Alpha) == UIPanel.Effect.Alpha)
            {
                EditorGUILayout.PropertyField(_alphaCurve);
            }
            if((_target.SwitingEffect & UIPanel.Effect.Scale) == UIPanel.Effect.Scale)
            {
                EditorGUILayout.PropertyField(_scaleCurve);
            }
            if((_target.SwitingEffect & UIPanel.Effect.Move) == UIPanel.Effect.Move)
            {
                EditorGUILayout.PropertyField(_moveCurve);
                EditorGUILayout.PropertyField(_moveVector);
            }

            if (order != _target.SortOrder)
            {
                _target.SortOrder = order;
                _target.Comp<Canvas>().sortingOrder = order.ToEnum<UIPanelSortOrder>().ti();
            }


            serializedObject.ApplyModifiedProperties();

        }
    }

}
