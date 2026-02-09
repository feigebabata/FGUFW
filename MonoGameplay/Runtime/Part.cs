using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using LitJson;
using UnityEngine;

namespace FGUFW.MonoGameplay
{
    public abstract class Part : MonoBehaviour,IPartUpdate,IPartPreload
    {
        public List<Part> SubParts = new List<Part>();

        protected UIPanel _uiPanel;

        public virtual IEnumerator OnCreating(Part play,Part parent)
        {
            foreach (var subPart in SubParts)
            {
                yield return subPart.OnCreating(play,this);
            }
        }

        protected virtual void OnDispose()
        {
            // savePartConfig();
            foreach (var subPart in SubParts)
            {
                subPart.OnDispose();
            }

            if(_uiPanel)
            {
                Destroy(_uiPanel.gameObject);
            }
            SubParts.Clear();

            Destroy(gameObject);
        }

        public T GetPart<T>() where T : Part
        {
            foreach (var subPart in SubParts)
            {
                if(subPart is T)
                {
                    return (T)subPart;
                }
            }
            return default;
        }

        public T AddPart<T>() where T : Part
        {
            var part = Create<T>(this);

            SubParts.Add(part);
            return part;
        }

        public virtual void OnUpdate(in PlayFrameData playFrameData)
        {
            foreach (var item in SubParts)
            {
                item.OnUpdate(in playFrameData);
            }
        }

        public virtual IEnumerator OnPreload()
        {
            yield return loadUIPanel();

            foreach (var subPart in SubParts)
            {
                yield return subPart.OnPreload();
            }
        }

        public static T Create<T>(Part parent) where T : Part
        {
            Transform tp = parent==default?null:parent.transform;
            var part = new GameObject(typeof(T).Name).AddComponent<T>();
            DontDestroyOnLoad(part.gameObject);
            part.transform.parent = tp;
            part.transform.localPosition = Vector3.zero;

            loadPartConfig(part);

            return part;
        }

        public static Part Create(Assembly assembly,string partTypeName)
        {
            Transform tp = default;
            Type partType = assembly.GetType(partTypeName);
            
            var part = new GameObject(partType.Name).AddComponent(partType) as Part;
            DontDestroyOnLoad(part.gameObject);
            part.transform.parent = tp;
            part.transform.localPosition = Vector3.zero;

            loadPartConfig(part);

            return part;
        }
        
#region UIPanel
        private IEnumerator loadUIPanel()
        {
            var uiPanelLoader = this.GetAttribute<UIPanelLoaderAttribute>();
            if (uiPanelLoader != null)
            {
                var path = uiPanelLoader.PrefabPath;
                var loader = AssetHelper.CopyAsync(path,null);
                yield return loader;
                GameObject go = loader.Result;
                DontDestroyOnLoad(go);
                _uiPanel = go.GetComponent<UIPanel>();
                _uiPanel.SetSortOrder(uiPanelLoader.SortOrder);
            }
        }

        public void ShowPanel()
        {
            _uiPanel.Show(this).Start(this);
        }

        public void HidePanel()
        {
            _uiPanel.Hide(this).Start(this);
        }
#endregion

#region IPartConfig
        static void loadPartConfig(Part self)
        {
            var partType = self.GetType();
            if(! typeof(IPartConfig).IsAssignableFrom(partType) )return;
            
            IPartConfig part = self as IPartConfig;
            var partConfigType = part.GetPartConfigType();

            var partConfig = PartConfigUtility.Get(partConfigType);

            if(partConfig==default)
            {
                var partConfigJsonData = PartConfigUtility.GetPartConfigJsonData(partConfigType);

                if(partConfigJsonData!=default)
                {
                    partConfig = JsonMapper.ToObject(partConfigJsonData.ToJson(),partConfigType);
                    part.PartConfig = partConfig;
                }

                PartConfigUtility.Set(part.PartConfig);
            }
            else
            {
                part.PartConfig = partConfig;
            }

        }

#endregion

    }
}
