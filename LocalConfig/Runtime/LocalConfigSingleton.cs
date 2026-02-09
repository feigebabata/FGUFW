using System;
using LitJson;
using UnityEngine;

namespace FGUFW.LocalConfig
{
    public abstract class LocalConfigSingleton<C> : MonoSingleton<LocalConfigSingleton<C>>
    {
        public C ConfigData;

        private static string fileName;
        protected override void Init()
        {
            base.Init();

            fileName = $"{this.GetType().FullName}.json";
            
            AppDomain.CurrentDomain.UnhandledException -= OnUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

            Load();
        }

        public override void Dispose()
        {
            AppDomain.CurrentDomain.UnhandledException -= OnUnhandledException;
            base.Dispose();
            Save();
        }

        protected override bool IsDontDestroyOnLoad()
        {
            return true;
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Save();
        }

        public void Load()
        {
            var jsonText = FileHelper.LocaReadText(fileName);

            if (jsonText.IsNull())
            {
                ConfigData = Activator.CreateInstance<C>();
                Save();
            }
            else
            {
                ConfigData = jsonText.ToObject<C>();
            }
        }

        public void Save()
        {
            var jsonText = ConfigData.ToJson();
            FileHelper.LocalWrite(fileName, jsonText);
        }


    }
}