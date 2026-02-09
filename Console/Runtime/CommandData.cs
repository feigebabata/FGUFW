using UnityEngine;
using System;
using System.Reflection;

namespace FGUFW.Console
{
    public class CommandData
    {
        public readonly string Key;
        public readonly string Description;
        public readonly MethodInfo Method;
        public readonly MonoTargetType Target;
        public readonly FindObjectsInactive Inactive;
        public readonly Type TargetType;
        public readonly bool IsStatic;

        public CommandData(string key,string description,MethodInfo methodInfo,MonoTargetType monoTargetType,Type targetType,bool isStatic,FindObjectsInactive inactive)
        {
            Key = key;
            Method = methodInfo;
            Description = description;
            Target = monoTargetType;
            TargetType = targetType;
            IsStatic = isStatic;
            Inactive = inactive;
        }

    }
}