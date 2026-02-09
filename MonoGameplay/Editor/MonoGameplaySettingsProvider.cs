using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System;
using UnityEditor.UIElements;
using System.IO;

namespace FGUFW.MonoGameplay.Editor
{
    // [Serializable]
    // [FilePath("ProjectSettings/MonoGameplaySettings.asset", FilePathAttribute.Location.ProjectFolder)]
    // public class MonoGameplaySettings : ScriptableSingleton<MonoGameplaySettings>
    // {
    //     [SerializeField]
    //     public string NameSpace = "TestSpace";

        
    //     [SerializeField]
    //     public string PlayName = "TestPlay";


    //     internal void Save() { Save(true); }
    //     private void OnDisable() { Save(); }
    //     internal SerializedObject GetSerializedObject() { return new SerializedObject(this); }
    // }

    [Serializable]
    public class MonoGameplaySettings
    {
        public string NameSpace = "TestSpace";
        public string PlayName = "TestPlay";
        public Vector2 PanelSize = new Vector2(1920, 1080);

        public static string FilePath => Application.dataPath.Replace("Assets", "ProjectSettings/MonoGameplaySettings.json");


        public void Save()
        {
            File.WriteAllText(FilePath,JsonUtility.ToJson(this,true));
        }

        public static MonoGameplaySettings Load()
        {
            if(File.Exists(FilePath))
            {
                return JsonUtility.FromJson<MonoGameplaySettings>(File.ReadAllText(FilePath));
            }
            var data = new MonoGameplaySettings();
            File.WriteAllText(FilePath,JsonUtility.ToJson(data));
            return data;
        }

        
    }

    public class MonoGameplaySettingsProvider : SettingsProvider
    {
        private static MonoGameplaySettings _settingData;
        public static MonoGameplaySettings SettingData
        {
            get
            {
                if(_settingData==null)
                {
                    _settingData = MonoGameplaySettings.Load();
                }
                return _settingData;
            }
        }


        public MonoGameplaySettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null) : base(path, scopes, keywords)
        {
        }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            
        }

        public override void OnGUI(string searchContext)
        {
            EditorGUI.BeginChangeCheck();

            // EditorGUILayout.LabelField("MonoGameplay", EditorStyles.boldLabel);

            SettingData.NameSpace = EditorGUILayout.TextField("NameSpace",SettingData.NameSpace);
            SettingData.PlayName = EditorGUILayout.TextField("PlayName",SettingData.PlayName);
            SettingData.PanelSize = EditorGUILayout.Vector2Field("PanelSize",SettingData.PanelSize);

            if (EditorGUI.EndChangeCheck())
            {
               SettingData.Save();
               EditorSettings.projectGenerationRootNamespace = SettingData.NameSpace;
               
            }
        }

        [SettingsProvider()]
        public static SettingsProvider Launcher()
        {
            return new MonoGameplaySettingsProvider("FGUFW/MonoGameplay",SettingsScope.Project)
            {

            };
        }
    
    }
}
