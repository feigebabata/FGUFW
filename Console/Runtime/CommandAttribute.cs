using System;
using UnityEngine;

namespace FGUFW.Console
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public sealed class CommandAttribute : Attribute
    {
        public readonly string Alias;
        public readonly string Description;
        public readonly FindObjectsInactive Inactive;
        public readonly MonoTargetType Target;

        public CommandAttribute(string alias=default,string description=default,MonoTargetType monoTargetType = MonoTargetType.First,FindObjectsInactive inactive = FindObjectsInactive.Exclude)
        {
            Alias = alias;
            Description = description;
            Target = monoTargetType;
            Inactive = inactive;
        }

    }
}