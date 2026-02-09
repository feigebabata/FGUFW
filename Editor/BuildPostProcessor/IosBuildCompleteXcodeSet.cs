#if UNITY_IOS
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEngine;

namespace FGUFW.Editor
{
    public class IosBuildCompleteXcodeSet : IPostprocessBuildWithReport
    {
        public int callbackOrder => 0;

        const string menuItemName = "EditorUtils/Build/enable IosBuildCompleteXcodeSet";
        [MenuItem(menuItemName)]
        public static void ULogEnable()
        {
            bool enabled = Menu.GetChecked(menuItemName);

            EditorPrefs.SetBool("IosBuildCompleteXcodeSet",!enabled);

            Menu.SetChecked(menuItemName, !enabled);
        }

        [MenuItem(menuItemName, true)]
        public static bool MenuLogOutCheck()//先刷新状态
        {

            var enabled = EditorPrefs.GetBool("IosBuildCompleteXcodeSet");

            Menu.SetChecked(menuItemName, enabled);
            return true;
        }

        public void OnPostprocessBuild(BuildReport report)
        {
            var enabled = EditorPrefs.GetBool("IosBuildCompleteXcodeSet");

            var outputPath = report.summary.outputPath;
            Debug.Log($"打包结束 {report.summary.result}, IosBuildCompleteXcodeSet:{enabled}\n{outputPath}");
            if (!enabled) return;

            if (report.summary.result == BuildResult.Succeeded || report.summary.result == BuildResult.Cancelled) return;
            // 在这里添加打包完成后的处理逻辑

            var configPath = Path.Combine(Application.dataPath, "FGUFW/Editor/BuildPostProcessor/IosBuildCompleteXcodeSet.txt");

            var pathToBuildProject = report.summary.outputPath;
            string projectPath = PBXProject.GetPBXProjectPath(pathToBuildProject);
            PBXProject project = new PBXProject();
            project.ReadFromFile(projectPath);
            string unityFrameworkTarget = project.GetUnityFrameworkTargetGuid();

            string plistPath = Path.Combine(pathToBuildProject, "Info.plist");
            var plist = new UnityEditor.iOS.Xcode.PlistDocument();
            plist.ReadFromFile(plistPath);

            foreach (var line in File.ReadAllLines(configPath))
            {
                if (line.IsNull()) continue; //空行跳过
                if (line.StartsWith("//")) continue; //注释跳过

                if (line.StartsWith("AddFramework:"))
                {
                    project.AddFrameworkToProject(unityFrameworkTarget, line.Replace("AddFramework:", string.Empty), false);
                }
                else if (line.StartsWith("AddPlistString:"))
                {
                    var items = line.Replace("AddPlistString:", string.Empty).Split(':');
                    if (items.Length != 2)
                    {
                        Debug.LogError($"无法识别:{line}");
                        continue;
                    }
                    plist.root.SetString(items[0], items[1]);
                }
            }

            project.WriteToFile(projectPath);

            // 写入修改后的 plist
            plist.WriteToFile(plistPath);

            Debug.Log("以执行完IosBuildCompleteXcodeSet!");
            
        }
    }

}
#endif