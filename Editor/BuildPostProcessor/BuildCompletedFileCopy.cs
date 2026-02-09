using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace FGUFW.Editor
{
    public class BuildCompletedFileCopy : IPostprocessBuildWithReport
    {
        public int callbackOrder => 0;

        const string menuItemName = "EditorUtils/Build/enable BuildCompletedFileCopy";
        [MenuItem(menuItemName)]
        public static void ULogEnable()
        {
            bool enabled = Menu.GetChecked(menuItemName);

            EditorPrefs.SetBool("BuildCompletedFileCopy",!enabled);

            Menu.SetChecked(menuItemName, !enabled);
        }

        [MenuItem(menuItemName, true)]
        public static bool MenuLogOutCheck()//先刷新状态
        {

            var enabled = EditorPrefs.GetBool("BuildCompletedFileCopy");

            Menu.SetChecked(menuItemName, enabled);
            return true;
        }

        public void OnPostprocessBuild(BuildReport report)
        {
            var enabled = EditorPrefs.GetBool("BuildCompletedFileCopy");

            var outputPath = report.summary.outputPath;
            Debug.Log($"打包结束 {report.summary.result}, BuildCompletedFileCopy:{enabled}\n{outputPath}");
            if (!enabled) return;

            if (report.summary.result == BuildResult.Succeeded || report.summary.result == BuildResult.Cancelled) return;
            // 在这里添加打包完成后的处理逻辑

            checkBuildCompletedFileCopy(outputPath);
        }

        private void checkBuildCompletedFileCopy(string outputPath)
        {
            var configPath = Path.Combine(Application.dataPath, "FGUFW/Editor/BuildPostProcessor/BuildCompletedFileCopy.txt");

            var configs = deConfig(configPath, outputPath);

            foreach (var item in configs)
            {
                if (item.IsFile)
                {
                    FileHelper.CopyFile(item.FromPath, item.ToPath);
                }
                else
                {
                    FileHelper.CopyDirectory(item.FromPath, item.ToPath);
                }
            }

            Debug.Log($"BuildCompletedFileCopy结束 执行配置共计:{configs.Count}");

        }

        List<ItemData> deConfig(string configPath,string buildOutPath)
        {
            var configs = new List<ItemData>();
            var textLines = File.ReadAllLines(configPath);
            foreach (var line in textLines)
            {
                if (line.IsNull()) continue; //空行跳过
                if (line.StartsWith("//")) continue; //注释跳过

                var items = line.Split(" > ");
                if (items == default || items.Length != 2)
                {
                    Debug.LogError($"无法解析:{line}");
                    continue;
                }

                var itemData = new ItemData();
                bool isFile = false;

                //源地址
                var fromPath = dePath(items[0], buildOutPath);
                if (File.Exists(fromPath))
                {
                    isFile = true;
                }
                else if (Directory.Exists(fromPath))
                {
                    isFile = false;
                }
                else
                {
                    Debug.LogError($"fromPath不存在:{fromPath}");
                    continue;
                }

                //目标地址
                var toPath = dePath(items[1], buildOutPath);
                if (File.Exists(toPath))
                {
                    if (!isFile)
                    {
                        Debug.LogError($"格式不统一:{line}");
                        continue;
                    }
                }
                else if (Directory.Exists(toPath))
                {
                    if (isFile)
                    {
                        Debug.LogError($"格式不统一:{line}");
                        continue;
                    }
                }
                else
                {
                    Debug.LogError($"toPath不存在:{toPath}");
                    continue;
                }

                itemData.IsFile = isFile;
                itemData.FromPath = fromPath;
                itemData.ToPath = toPath;

                configs.Add(itemData);

            }
            return configs;
        }

        string dePath(string item,string buildOutPath)
        {
            if (item.StartsWith("Local:"))
            {
                var directory = Path.GetDirectoryName(Application.dataPath);
                return Path.Combine(directory, item.Replace("Local:", string.Empty));
            }
            else if (item.StartsWith("BuildOutPath:"))
            {
                return Path.Combine(buildOutPath, item.Replace("BuildOutPath:", string.Empty));
            }
            return item;
        }

        public struct ItemData
        {
            public bool IsFile;
            public string FromPath, ToPath;
        }
    }
}