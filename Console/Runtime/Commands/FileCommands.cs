using System.IO;
using UnityEngine;
using LitJson;

namespace FGUFW.Console
{
    public static class FileCommands
    {
        [Command("file.save","保存文本到路径; path,text")]
        public static void Save(object arg1,object arg2)
        {
            var path = ConsoleUtility.Input2Arg<string>(arg1).ToString();
            var text = ConsoleUtility.Input2Arg<string>(arg2).ToString();
            File.WriteAllText(path,text);
        }

        [Command("file.savejson","保存对象Json文本到路径; path,object")]
        public static void SaveJson(object arg1,object arg2)
        {
            var path = ConsoleUtility.Input2Arg<string>(arg1).ToString();
            var text = JsonMapper.ToJson(arg2);
            File.WriteAllText(path,text);
        }

    }    
}