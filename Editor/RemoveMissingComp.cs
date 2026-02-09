using UnityEditor;
using UnityEngine;

namespace FGUFW.Editor
{

    public static class RemoveMissingComp
    {
        [MenuItem("GameObject/Remove Missing Comp")]
        static void Execute()
        {
            if (Selection.gameObjects == default)
            {
                foreach (var obj in Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                {
                    GameObjectUtility.RemoveMonoBehavioursWithMissingScript(obj);
                }
            }
            else
            {
                foreach (var item in Selection.gameObjects)
                {
                    removeMiss(item);
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

        }

        static void removeMiss(GameObject obj)
        {
            GameObjectUtility.RemoveMonoBehavioursWithMissingScript(obj);


            foreach (Transform item in obj.transform)
            {
                removeMiss(item.gameObject);
            }
        }
    }

}