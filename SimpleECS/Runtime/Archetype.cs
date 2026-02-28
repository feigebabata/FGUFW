using System;
using Unity.Collections;

namespace FGUFW.SimpleECS
{
    //Entity组合模式 不够再手动扩容
    public struct Archetype
    {
        public long Bit_0_63; // 0-63 位
        public long Bit_64_127; // 64-127 位
        public const int Length = 128; // 明确标明上限

        public void Add(int bitIndex)
        {
            if (bitIndex < 64)
            {
                Bit_0_63 |= 1L << bitIndex;
            }
            else if (bitIndex < 128)
            {
                Bit_64_127 |= 1L << (bitIndex - 64);
            }
        }

        public void Remove(int bitIndex)
        {
            if (bitIndex < 64)
            {
                Bit_0_63 &= ~(1L << bitIndex);
            }
            else if (bitIndex < 128)
            {
                Bit_64_127 &= ~(1L << (bitIndex - 64));
            }
        }

        public readonly bool HasAll(Archetype filter)
        {
            var has = (Bit_0_63 & filter.Bit_0_63) == filter.Bit_0_63;
            has = has && (Bit_64_127 & filter.Bit_64_127) == filter.Bit_64_127;
            return has;
        }
    }
}