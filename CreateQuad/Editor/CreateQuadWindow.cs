using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using FGUFW;
using System;
using UnityEditor.UIElements;

namespace FGUFW.CreateQuad
{
    public class CreateQuadWindow : EditorWindow
    {
        [MenuItem("Assets/CreateTrail")]
        private static void createTrail()
        {
            int layer = 16;
            var mesh = MeshHelper.CreateTrail(layer);
            var path = $"Assets/Trail_{layer}.asset";
            AssetDatabase.CreateAsset(mesh,path);
            AssetDatabase.Refresh();
        }


        [SerializeField]
        private VisualTreeAsset m_VisualTreeAsset = default;

        [MenuItem("Assets/CreateQuadMesh")]
        public static void ShowExample()
        {
            CreateQuadWindow wnd = GetWindow<CreateQuadWindow>();
            wnd.titleContent = new GUIContent("创建Quad模型");
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            // Instantiate UXML
            VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
            root.Add(labelFromUXML);

            root.Q<Button>("confirm_Btn").clicked += onClickConfirm_Btn;
        }

        private void onClickConfirm_Btn()
        {
            var sizeField = rootVisualElement.Q<Vector2Field>("sizeField");
            var pivotField = rootVisualElement.Q<Vector2Field>("pivotField");
            var nameField = rootVisualElement.Q<TextField>("nameField");

            var mesh = MeshHelper.CreateQuad(sizeField.value,pivotField.value);
            var path = $"Assets/{nameField.value}.asset";
            AssetDatabase.CreateAsset(mesh,path);
            AssetDatabase.Refresh();
        }
    }
}
