using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using FGUFW;

namespace FGUFW.MonoGameplay.Editor
{
/*
为避免UIPanel加载时显示在游戏中 预制件的Canvas需要默认隐藏
在预制件打开的时候自动激活Canvas
*/
    public static class UIPanelPrefabChanged
    {
        private static string _gObjPath;

        [InitializeOnLoadMethod]
        static void initialized()
        {
            PrefabStage.prefabStageOpened -= prefabStageOpened;
            PrefabStage.prefabStageOpened += prefabStageOpened;
            
            PrefabStage.prefabStageClosing -= prefabStageClosing;
            PrefabStage.prefabStageClosing += prefabStageClosing;

        }


        private static void prefabStageClosing(PrefabStage stage)
        {
            var gObj = stage.prefabContentsRoot;
            if(gObj.Comp<UIPanel>() == default) return;

            _gObjPath = stage.assetPath;

            gObj = AssetDatabase.LoadAssetAtPath<GameObject>(_gObjPath);

            var canvas = gObj.Comp<Canvas>();
            canvas.enabled = false;
            EditorUtility.SetDirty(canvas);
            EditorUtility.SetDirty(gObj);
            AssetDatabase.SaveAssetIfDirty(gObj);

        
            // EditorApplication.delayCall += delayCall;
        }

        // private static void delayCall()
        // {
        //     EditorApplication.delayCall -= delayCall;
            
            

        // }

        private static void prefabStageOpened(PrefabStage stage)
        {
            
            var gObj = stage.prefabContentsRoot;
            if(gObj.Comp<UIPanel>() == default) return;
            
            gObj.Comp<Canvas>().enabled = true;
        }

    }
}