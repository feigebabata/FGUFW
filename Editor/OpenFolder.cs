using System;
using System.Collections.Generic;
using System.IO;
using FGUFW.Platform;
using UnityEditor;
using UnityEngine;

namespace FGUFW.Editor
{
    public static class OpenFolder
    {
        [MenuItem("文件夹/持续存储地址")]
        static void openPersistentDataPath()
        {
            // Open(Application.persistentDataPath);
            EditorUtility.RevealInFinder(Application.persistentDataPath);
        }

        [MenuItem("文件夹/项目根目录")]
        static void openDataPath()
        {
            // Open(Application.dataPath);
            EditorUtility.RevealInFinder(Application.dataPath);
        }

        [MenuItem("文件夹/清空持续存储地址")]
        static void cleanPersistentDataPath()
        {
            FileHelper.ClearDirectory(Application.persistentDataPath);
            Debug.LogWarning("已清空持续存储地址!");
        }


        public static void Open(string path)
        {
            #if UNITY_EDITOR_WIN
            WinPlatform.OpenExplorer(path);
            #endif
        }

    }
}