using System;
using System.Collections.Generic;
using System.IO;
using LitJson;
using UnityEngine;

namespace FGUFW.Gameplay
{
    /// <summary>
    /// Part的存档工具
    /// </summary>
    public static class PartSaveUtility
    {
        static Dictionary<string,object> partSaves;
        static string filePath;

        [RuntimeInitializeOnLoadMethod]
        static void initialize()
        {
            filePath = Path.Combine(Application.persistentDataPath, "PartSaves.json");

            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                partSaves = JsonMapper.ToObject<Dictionary<string,object>>(json);
            }
            else
            {
                partSaves = new();
            }

            Application.quitting += quiting;
        }

        private static void quiting()
        {
            Application.quitting -= quiting;
            Save();
        }

        public static object Get(Type type)
        {
            var key = type.FullName;

            object partSaveData = default;
            if(!partSaves.TryGetValue(key,out partSaveData))
            {
                partSaveData = Activator.CreateInstance(type);
            }
            return partSaveData;
        }

        public static string ToJson()
        {
            return JsonMapper.ToJson(partSaves);
        }

        public static void Save()
        {
            var json = ToJson();

            File.WriteAllText(filePath,json);
        }
    }
}