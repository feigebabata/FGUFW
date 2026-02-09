using System;

namespace FGUFW
{
    public static class NumberExtensions
    {
        const string abbrCode = "KMBTqQsSOND"; //10^33

        /// <summary>
        /// 金融/统计类数值单位
        /// </summary>
        /// <param name="self"></param>
        /// <returns>余数和后缀</returns>
        public static (decimal,char) ToAbbr(this decimal self)
        {
            decimal util = 1000;
            char endCode = default;

            for (int i = 0; i < abbrCode.Length; i++)
            {
                if(self<util)break;

                self = self/util;
                endCode = abbrCode[i];
            }

            return (self,endCode);
        }

        public static (int,char) ToAbbr(this int self)
        {
            decimal num = self;
            var (n,c) = num.ToAbbr();
            return ((int)n,c);
        }

        public static (double,char) ToAbbr(this double self)
        {
            decimal num = (decimal)self;
            var (n,c) = num.ToAbbr();
            return ((double)n,c);
        }

        public static (float,char) ToAbbr(this float self)
        {
            decimal num = (decimal)self;
            var (n,c) = num.ToAbbr();
            return ((float)n,c);
        }

        public static int ti(this float self)
        {
            return (int)self;
        }

    }
}