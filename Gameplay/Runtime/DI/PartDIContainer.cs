using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace FGUFW.Gameplay
{
    /// <summary>
    /// Part的依赖注入容器
    /// </summary>
    public abstract class PartDIContainer:MonoBehaviour
    {
        // protected bool initialed;
        protected Dictionary<Type,object> defaultDependency= new();

        public async UniTask TryInject(Part part,CancellationToken partTaskCancellationToken)
        {
            // await waitInitialed();

            foreach (FieldInfo f_info in part.GetType().GetFields (BindingFlags.Instance | BindingFlags.Public  | BindingFlags.NonPublic))
            {
                if(partTaskCancellationToken.IsCancellationRequested)break;

                var injectConfig = f_info.GetCustomAttribute<InjectAttribute>();
                if(injectConfig!=default)
                {
                    await injectField(part,f_info,injectConfig.Field,partTaskCancellationToken);
                }
            }
        }

        private async UniTask injectField(Part part,FieldInfo f_info, InjectField field,CancellationToken partTaskCancellationToken)
        {
            switch (field)
            {
                case InjectField.Default:
                    injectDefaultField(part,f_info);
                break;
                case InjectField.UI:
                    await injectUIField(part,f_info,partTaskCancellationToken);
                break;
                case InjectField.Save:
                    injectSaveField(part,f_info);
                break;
            }
        }

        
        // private async UniTask waitInitialed()
        // {
        //     while (!initialed)
        //     {
        //         await UniTask.DelayFrame(1);
        //     }
        // }

        protected abstract UniTask injectUIField(Part part,FieldInfo f_info,CancellationToken partTaskCancellationToken);

        protected void injectDefaultField(Part part,FieldInfo f_info)
        {
            var fieldType = f_info.FieldType;
            object fieldData = default;
            
            if(!defaultDependency.TryGetValue(fieldType,out fieldData))
            {
                Debug.LogWarning($"{name}未注册{fieldType.FullName}!");
                return;
            }
            f_info.SetValue(part,fieldData);
        }

        protected void injectSaveField(Part part,FieldInfo f_info)
        {
            var fieldType = f_info.FieldType;
            object fieldData = PartSaveUtility.Get(fieldType);
            f_info.SetValue(part,fieldData);
        }

        public static void DestroyPlay<T>() where T: Play
        {
            var play = GameObject.FindFirstObjectByType<T>(FindObjectsInactive.Include);
            Destroy(play.gameObject);
        }

        public static void CreatePlay<T>()
        {
            var playName = typeof(T).Name;
            var key = $"Assets/Develop/{playName}/{playName}.unity";
            
            AssetHelper.LoadSceneAsync(key);
        }
    }
}