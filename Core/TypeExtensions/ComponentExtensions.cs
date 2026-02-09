using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.Playables;
using System.Collections.Generic;
using System.Reflection;

namespace FGUFW
{
    public static class ComponentExtensions
    {
        
        public static void AutoRefField(this Component self)
        {
            var type = self.GetType();
            var fields = type.GetFields(BindingFlags.Public|BindingFlags.Instance);
            var compType = typeof(Component);
            var gObjType = typeof(GameObject);

            List<Transform> targetCache = new List<Transform>();
            List<float> similarValues = new List<float>();

            foreach (var field in fields)
            {
                var fieldType = field.FieldType;
                var fieldName = field.Name;

                if(fieldType.IsSubclassOf(compType))
                {
                    targetCache.Clean();
                    similarValues.Clean();
                    self.transform.FindSimilar(fieldName,targetCache,similarValues);

                    foreach (var itemT in targetCache)
                    {
                        var comp = itemT.GetComponent(fieldType);
                        if(comp!=default)
                        {
                            field.SetValue(self,comp);
                            break;
                        }
                    }
                }
                else if(gObjType == fieldType)
                {
                    targetCache.Clean();
                    similarValues.Clean();
                    self.transform.FindSimilar(fieldName,targetCache,similarValues);
                    if(targetCache.Count>0)
                    {
                        field.SetValue(self,targetCache[0].gameObject);
                    }
                }
                

            }
        }

        public static T Comp<T>(this Component self)
        {
            return self.GetComponent<T>();
        }

        public static T Comp<T>(this GameObject self)
        {
            return self.GetComponent<T>();
        }

        public static Transform GetChild(this Component self,int childIndex)
        {
            return self.transform.GetChild(childIndex);
        }

        public static T GetChild<T>(this Component self,int childIndex)
        {
            return self.transform.GetChild(childIndex).GetComponent<T>();
        }

        public static void SetActive(this Component self,bool b)
        {
            self.gameObject.SetActive(b);
        }

        public static void Destroy(this Component self)
        {
            GameObject.Destroy(self);
        }

        public static void DestroyGObj(this Component self)
        {
            GameObject.Destroy(self.gameObject);
        }

#if UNITY_EDITOR

        [MenuItem("CONTEXT/MonoBehaviour/AutoRefField")]
        static void autoRefField(MenuCommand command)
        {
            var comp = (MonoBehaviour)command.context;
            comp.AutoRefField();
            EditorUtility.SetDirty(comp);
        }

#endif
        
    }
}