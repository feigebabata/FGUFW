using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using LitJson;
using UnityEngine;

namespace FGUFW.MonoGameplay
{
    public interface IPartConfig
    {
        object PartConfig{get;set;}
        Type GetPartConfigType();
    }

    public static class PartConfigUtility
    {
        static JsonData partConfigJsonData;
        static Dictionary<string,object> partConfigs = new Dictionary<string, object>();
        static string filePath;

        [RuntimeInitializeOnLoadMethod]
        static void initialize()
        {
            filePath = Path.Combine(Application.persistentDataPath, "PartConfigs.json");

            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                partConfigJsonData = JsonMapper.ToObject(json);
            }
            else
            {
                partConfigJsonData = new JsonData();
            }

            Application.quitting += quiting;
        }

        private static void quiting()
        {
            Application.quitting -= quiting;
            Save();
        }

        public static JsonData GetPartConfigJsonData(Type type)
        {
            var key = type.FullName;
            if(partConfigJsonData.ContainsKey(key))
            {
                return partConfigJsonData[key];
            }
            return default;
        }

        public static void Set(object obj)
        {
            var key = obj.GetType().FullName;
            if(partConfigs.ContainsKey(key))
            {
                partConfigs[key] = obj;
            }
            else
            {
                partConfigs.Add(key,obj);
            }
        }

        public static object Get(Type type)
        {
            var key = type.FullName;
            if(partConfigs.ContainsKey(key))
            {
                return partConfigs[key];
            }
            return default;
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