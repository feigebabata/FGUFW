using System;

namespace FGUFW.Gameplay
{
    /// <summary>
    /// 标记字段被自动注入
    /// </summary>
    public class InjectAttribute : Attribute
    {
        public InjectField Field = InjectField.Default;

        public InjectAttribute()
        {
            
        }

        public InjectAttribute(InjectField field)
        {
            Field = field;
        }
    }

    public enum InjectField
    {
        /// <summary>
        /// 默认 一般直接new
        /// </summary>
        Default,

        /// <summary>
        /// UI
        /// </summary>
        UI,
        
        /// <summary>
        /// 存档
        /// </summary>
        Save
    }
}