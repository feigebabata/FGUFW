
using UnityEngine;

namespace FGUFW.Console
{
    public static class TimeCommands
    {
        
        [Command("get-time")]
        public static float GetTime()
        {
            return Time.time;
        }

        [Command("set-timescale","设置时间缩放; float")]
        public static void SetTimeScale(object arg1)
        {
            var scale = (float)ConsoleUtility.Input2Arg<float>(arg1);
            Time.timeScale = scale;
        }
        
        [Command("get-timescale","获取时间缩放;")]
        public static float GetTimeScale()
        {
            return Time.timeScale;
        }

    }
}