using UnityEngine;

namespace FGUFW.Console
{
    public static class SystemInfoCommands
    {
        [Command("device-name","设备名;")]
        public static string DeviceName()
        {
            return SystemInfo.deviceName;
        }        
    }
}