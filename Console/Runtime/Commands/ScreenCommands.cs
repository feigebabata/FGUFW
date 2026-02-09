
using UnityEngine;

namespace FGUFW.Console
{
    public static class ScreenCommands
    {
        
        [Command("截屏","保存截屏到路径; path")]
        public static void CaptureScreenshot(object arg1)
        {
            var path = ConsoleUtility.Input2Arg<string>(arg1).ToString();
            ScreenCapture.CaptureScreenshot(path);
        }
        
        [Command("fullscreen","全屏; bool")]
        public static void Fullscreen(object arg1)
        {
            var b = (bool)ConsoleUtility.Input2Arg<bool>(arg1);
            Screen.fullScreen = b;
        }
        
        [Command("获取屏幕方向")]
        public static ScreenOrientation GetOrientation()
        {
            return Screen.orientation;
        }
        
        [Command("设置屏幕方向","int")]
        public static void SetOrientation(object arg1)
        {
            var so = (ScreenOrientation)ConsoleUtility.Input2Arg<int>(arg1);
            Screen.orientation = so;
        }

    }
}