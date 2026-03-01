using System;
using System.Collections.Generic;
using System.IO;
using LitJson;
using UnityEngine;

namespace FGUFW.Gameplay
{
    
    public static class PartConfigUtility
    {
        static Dictionary<string,object> partConfigs;
        static string filePath;

        [RuntimeInitializeOnLoadMethod]
        static void initialize()
        {
            filePath = Path.Combine(Application.persistentDataPath, "PartConfigs.json");

            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                partConfigs = JsonMapper.ToObject<Dictionary<string,object>>(json);
            }
            else
            {
                partConfigs = new();
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

            object partData = default;
            if(!partConfigs.TryGetValue(key,out partData))
            {
                partData = Activator.CreateInstance(type);
            }
            return partData;
        }

        public static string ToJson()
        {
            return JsonMapper.ToJson(partConfigs);
        }

        public static void Save()
        {
            var json = ToJson();

            File.WriteAllText(filePath,json);
        }
    }
}