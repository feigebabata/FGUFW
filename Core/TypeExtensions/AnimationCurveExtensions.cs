using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.Playables;
using System.Text;

namespace FGUFW
{
    public static class AnimationCurveExtensions
    {
        
        public static string ToCodeText(this AnimationCurve self, string name)
        {
            if (self == default) return default;

            var codeText = new StringBuilder();
            codeText.AppendLine($"        AnimationCurve {name} = new AnimationCurve");
            codeText.AppendLine("        {");
            codeText.AppendLine("            keys = new Keyframe[]");
            codeText.AppendLine("            {");

            foreach (var item in self.keys)
            {
                codeText.AppendLine($"                new Keyframe{{time={item.time}f,value={item.value}f,inTangent={item.inTangent}f,outTangent={item.outTangent}f,inWeight={item.inWeight}f,outWeight={item.outWeight}f}},");
            }

            codeText.AppendLine("            },");
            codeText.AppendLine($"            preWrapMode = WrapMode.{self.preWrapMode},");
            codeText.AppendLine($"            postWrapMode = WrapMode.{self.postWrapMode},");
            codeText.AppendLine("        };");
            return codeText.ToString();
        }
        
    }
}