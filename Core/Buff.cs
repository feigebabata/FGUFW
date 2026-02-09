using System;
using System.Collections.Generic;

namespace FGUFW
{
    [Serializable]
    public class Buff
    {
        public string Id;
        public string Name;
        public string Description;

        /// <summary>
        /// buff类型 一般转enum后使用
        /// </summary>
        public int Type;

        /// <summary>
        /// 不够用就改成object 但是会装箱
        /// </summary>
        public float BaseValue;

        /// <summary>
        /// 持续时长 -1:永久
        /// </summary>
        public float Duration;

        /// <summary>
        /// 同id处理 >1:可叠加上限,1:不变,0:时间刷新,-1:可重复不叠加,-2:可叠加无上限,-3:时间叠加
        /// </summary>
        public int Overlap;

        /// <summary>
        /// 0:并行,其他同层只能存在一个 LayerWeight>当前层则替换
        /// </summary>
        public int Layer;
        public int LayerWeight;

        /// <summary>
        /// 扩展部分 自己加
        /// </summary>
        public object Expandsion;

        /// <summary>
        /// 起始时间 需要时重置
        /// </summary>
        public float StartWorldTime;

        /// <summary>
        /// 叠加时修改
        /// </summary>
        public int StackCount = 1;


        public virtual float Value()
        {
            return StackCount * BaseValue;
        }

        public bool Timeout(float worldTime)
        {
            if(Duration==-1)return false;

            return worldTime-StartWorldTime>=Duration;
        }


        /// <summary>
        /// 0:成功添加,1:不可变,2:同层没能覆盖,3:未知
        /// </summary>
        public static int Add(List<Buff> ls,Buff newBuff)
        {
            int idx = default;
            if(newBuff.Layer!=0)//同层不能共存
            {
                idx = ls.FindIndex(buf=>buf.Layer==newBuff.Layer);
                if(idx!=-1)
                {
                    if(newBuff.LayerWeight>ls[idx].LayerWeight)
                    {
                        ls[idx] = newBuff;
                        return 0;
                        
                    }
                    else
                    {
                        return 2;
                    }
                }
            }

            idx = ls.FindIndex(buf=>buf.Id==newBuff.Id);

            if(idx==-1)//无重复
            {
                ls.Add(newBuff);
                return 0;
            }

            var buff = ls[idx];

            if(buff.Overlap==-3)//时间叠加
            {
                buff.StartWorldTime += buff.Duration;

                return 0;
            }
            else if(buff.Overlap==-2)//叠加无上限
            {
                buff.StackCount++;
                buff.StartWorldTime = newBuff.StartWorldTime;

                return 0;
            }
            else if(buff.Overlap==-1)//可重复 不叠加
            {
                ls.Add(newBuff);
                return 0;
            }
            else if(buff.Overlap==0)//刷新时间
            {
                buff.StartWorldTime = newBuff.StartWorldTime;
                return 0;
            }
            else if(buff.Overlap==1)//不变
            {
                return 1;
            }
            else if(buff.Overlap>1)//叠加有上限
            {
                if(buff.StackCount<buff.Overlap)
                {
                    buff.StackCount++;
                }   
                buff.StartWorldTime = newBuff.StartWorldTime;
                return 0;
            }
            return 3;
        }

        public static float GetValue(List<Buff> ls,int bufType)
        {
            float val = default;
            foreach (var buf in ls)
            {
                if(buf.Type == bufType) val+=buf.Value();                
            }
            return val;
        }

        public static int RemoveAllTimeout(List<Buff> ls,float worldTime)
        {
            int rmCount = 0;
            for (int i = 0; i < ls.Count; i++)
            {
                if(ls[i].Timeout(worldTime))
                {
                    ls.RemoveAtSwapBack(i);
                    i--;
                    
                    rmCount++;
                }
            }
            return rmCount;
        }

    }
}