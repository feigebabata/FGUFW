using UnityEngine;

namespace FGUFW.Console
{
    public static class ApplicationCommands
    {
        [Command("quit","关闭应用;")]
        public static void Quit()
        {
            Application.Quit();
        }

        [Command("open-url","打开网址; url")]
        public static void OpenURL(object arg1)
        {
            var url = ConsoleUtility.Input2Arg<string>(arg1).ToString();
            Application.OpenURL(url);
        }

        [Command("get-framerate","获取目标帧率;")]
        public static int GetFrameRate()
        {
            return Application.targetFrameRate;
        }

        [Command("set-framerate","设置目标帧率; int")]
        public static void SetFrameRate(object arg1)
        {
            var frameRate = (int)ConsoleUtility.Input2Arg<int>(arg1);
            Application.targetFrameRate = frameRate;
        }

        [Command("systemlanguage","获取系统语言;")]
        public static SystemLanguage GetSystemLanguage()
        {
            return Application.systemLanguage;
        }
        
        [Command("后台运行","bool")]
        public static void RunInBackground(object arg1)
        {
            var b = (bool)ConsoleUtility.Input2Arg<bool>(arg1);
            Application.runInBackground = b;
        }

        [Command("app-path","获取应用持续存储路径;")]
        public static string GetAppSavePath()
        {
            return Application.persistentDataPath;
        }

    }
    
}