using System;

namespace FGUFW.MonoGameplay
{
    public class UIPanelLoaderAttribute:Attribute
    {
        public string PrefabPath;
        public int SortOrder;

        public UIPanelLoaderAttribute(string prefabPath, int sortOrder=0)
        {
            PrefabPath = prefabPath;
            SortOrder = sortOrder;
        }
    }

}