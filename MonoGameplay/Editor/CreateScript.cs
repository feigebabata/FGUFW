# if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using System.IO;
using FGUFW;
using FGUFW.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace FGUFW.MonoGameplay.Editor
{

    public static class CreateScript
    {
        const int MENU_ORDER = 10;

        [MenuItem("Assets/Create/MonoGameplay/Play",false,MENU_ORDER)]
        static void createPlay()
        {
            string createPath = EditorUtil.GetSeleceFolderPath()+"/Play.cs";
            var endNameEditAction = ScriptableObject.CreateInstance<CreateScriptHelper>();
            endNameEditAction.Callback = (filePath)=>
            {
                var scriptText = 
@"using System.Collections;
using System.Collections.Generic;
using FGUFW.MonoGameplay;
using UnityEngine;
using FGUFW;
using static FGUsing;

namespace |NAME_SPACE|
{
    public class |CLASS_NAME| : Play<|CLASS_NAME|>
    {
        public override IEnumerator OnCreating(Part play,Part parent)
        {
            //AddPart<MonoGameplayTestPart>();
            
            yield return base.OnCreating(this,this);
        }

        protected override void OnDispose()
        {
            base.OnDispose();
        }
    }
}

";

                var className = Path.GetFileName(filePath).Replace(".cs","");
                scriptText = scriptText.Replace("|CLASS_NAME|",className);

                MonoGameplaySettingsProvider.SettingData.NameSpace = className.Replace("Play","");
                MonoGameplaySettingsProvider.SettingData.PlayName = className;
                MonoGameplaySettingsProvider.SettingData.Save();

                EditorSettings.projectGenerationRootNamespace = MonoGameplaySettingsProvider.SettingData.NameSpace;

                scriptText = scriptText.Replace("|NAME_SPACE|",MonoGameplaySettingsProvider.SettingData.NameSpace);

                return scriptText;
            };
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,endNameEditAction,createPath,null,null);
        }

        [MenuItem("Assets/Create/MonoGameplay/Part",false,MENU_ORDER)]
        static void createPart()
        {
            string createPath = EditorUtil.GetSeleceFolderPath()+"/Part.cs";
            var endNameEditAction = ScriptableObject.CreateInstance<CreateScriptHelper>();
            endNameEditAction.Callback = (filePath)=>
            {
                var scriptText = 
@"using System.Collections;
using System.Collections.Generic;
using FGUFW.MonoGameplay;
using UnityEngine;
using FGUFW;
using static FGUsing;

namespace |NAME_SPACE|
{
    
    public class |CLASS_NAME| : Part
    {
        private |PLAY_NAME| _play;

        public override IEnumerator OnCreating(Part play,Part parent)
        {
            _play = play as |PLAY_NAME|;
            addListener();
            yield return base.OnCreating(play,parent);
        }

        protected override void OnDispose()
        {
            removeListener();
            base.OnDispose();
        }

        private void addListener()
        {

        }

        private void removeListener()
        {
            
        }

    }
}

";

                var className = Path.GetFileName(filePath).Replace(".cs","");
                scriptText = scriptText.Replace("|CLASS_NAME|",className);
                scriptText = scriptText.Replace("|NAME_SPACE|",MonoGameplaySettingsProvider.SettingData.NameSpace);
                scriptText = scriptText.Replace("|PLAY_NAME|",MonoGameplaySettingsProvider.SettingData.PlayName);

                return scriptText;
            };
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,endNameEditAction,createPath,null,null);
        }

        [MenuItem("Assets/Create/MonoGameplay/PartFolder",false,MENU_ORDER)]
        static void createPartFolder()
        {

            string createPath = EditorUtil.GetSeleceFolderPath()+"/Part";
            var endNameEditAction = ScriptableObject.CreateInstance<CreateFolderHelper>();
            endNameEditAction.Callback = (folderPath)=>
            {
                if(!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                    Directory.CreateDirectory(Path.Combine(folderPath,"Sprite"));
                }
                

                var scriptText = 
@"using System.Collections;
using System.Collections.Generic;
using FGUFW.MonoGameplay;
using UnityEngine;
using FGUFW;
using static FGUsing;

namespace |NAME_SPACE|
{
    [UIPanelLoader("""",(int)UIPanelSortOrder.Base)]
    public partial class |CLASS_NAME| : Part
    {
        private |PLAY_NAME| _play;
        private |CLASS_NAME|PanelComps _panelComps;

        public override IEnumerator OnCreating(Part play,Part parent)
        {
            _play = play as |PLAY_NAME|;
            yield return base.OnCreating(play,parent);
        }

        public override IEnumerator OnPreload()
        {
            yield return base.OnPreload();
            _panelComps = _uiPanel.Comp<|CLASS_NAME|PanelComps>();
            addListener();
        }

        protected override void OnDispose()
        {
            removeListener();
            base.OnDispose();
        }

        private void addListener()
        {
            _panelComps.TryAddAllBtnListener(this);
        }

        private void removeListener()
        {
            _panelComps.TryRemoveAllBtnListener();
        }

    }
}

";

                var className = Path.GetFileName(folderPath);
                scriptText = scriptText.Replace("|CLASS_NAME|",className);
                scriptText = scriptText.Replace("|NAME_SPACE|",MonoGameplaySettingsProvider.SettingData.NameSpace);
                scriptText = scriptText.Replace("|PLAY_NAME|",MonoGameplaySettingsProvider.SettingData.PlayName);
                File.WriteAllText(Path.Combine(folderPath,className+".cs"),scriptText);

                scriptText = 
@"using System;
using System.Collections;
using System.Collections.Generic;
using FGUFW.MonoGameplay;
using UnityEngine;

namespace |NAME_SPACE|
{
    public partial class |CLASS_NAME| : IPartConfig
    {
        [Serializable]
        public class Config
        {
            
        }

        public Config SelfConfig = new Config();        
        public object PartConfig 
        { 
            get
            {
                return SelfConfig;
            }
            set
            {
                SelfConfig = value as Config;
            }
        }

        public Type GetPartConfigType()
        {
            return typeof(Config);
        }
    }
}
";
                scriptText = scriptText.Replace("|CLASS_NAME|",className);
                scriptText = scriptText.Replace("|NAME_SPACE|",MonoGameplaySettingsProvider.SettingData.NameSpace);
                File.WriteAllText(Path.Combine(folderPath,className+"Config.cs"),scriptText);


                scriptText = 
@"using FGUFW;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace |NAME_SPACE|
{
    public class |CLASS_NAME|PanelComps : MonoBehaviour
    {
    }
}

";
                scriptText = scriptText.Replace("|CLASS_NAME|",className);
                scriptText = scriptText.Replace("|NAME_SPACE|",MonoGameplaySettingsProvider.SettingData.NameSpace);
                File.WriteAllText(Path.Combine(folderPath,className+"PanelComps.cs"),scriptText);

                var panelGO = new GameObject($"{className}Panel");
                
                var canvas = panelGO.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;

                var canvasGroup = panelGO.AddComponent<CanvasGroup>();
                panelGO.AddComponent<GraphicRaycaster>();

                var canvasScaler = panelGO.AddComponent<CanvasScaler>();
                canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
                canvasScaler.referenceResolution = MonoGameplaySettingsProvider.SettingData.PanelSize;

                var uiPanel = panelGO.AddComponent<UIPanel>();
                uiPanel.SortOrder = ((UIPanelSortOrder)0).ts();
                canvas.sortingOrder = 0;

                var mask = new GameObject("Mask").AddComponent<Image>();
                mask.color = new Color32(0, 0, 0, 222);
                mask.transform.SetParent(panelGO.transform);
                var maskRT = mask.rectTransform;
                maskRT.anchorMin = Vector2.zero;
                maskRT.anchorMax = Vector2.one;
                maskRT.offsetMin = Vector2.zero;
                maskRT.offsetMax = Vector2.zero;

                var safeAreaGO = new GameObject("SafeArea");
                safeAreaGO.transform.SetParent(panelGO.transform);
                safeAreaGO.AddComponent<SafeAreaAdapter>();

                PrefabUtility.SaveAsPrefabAsset(panelGO,Path.Combine(folderPath,$"{className}Panel.prefab"));
                GameObject.DestroyImmediate(panelGO);

            };
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,endNameEditAction,createPath,null,null);
        }
    }


}

#endif