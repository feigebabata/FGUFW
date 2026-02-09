using System;
using UnityEngine;

namespace FGUFW
{
    public static class DateTimeExtensions
    {
        private static long prevTick = -1;
        private static DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long UnixMilliseconds(this DateTime dateTime)
        {
            return Convert.ToInt64((dateTime.ToUniversalTime() - epoch).TotalMilliseconds);
        }

        /// <summary>
        /// 存记录
        /// </summary>
        public static void SetRecord(this DateTime self)
        {
            prevTick = self.Ticks;
        }

        /// <summary>
        /// 获取距上次SetRecord的时间 毫秒
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static long GetRecordTime(this DateTime self)
        {
            if (prevTick == -1) throw new Exception("未调用SetRecord");
            long delta = self.Ticks - prevTick;
            return delta / 10000;
        }

        public static string SecondTickName(this DateTime dateTime)
        {
            return DateTime.Now.ToString("yyyyMMddHHmmss");
        }

        public static DateTime ToDate(this string self)
        {
            if (self.IsNull()) return DateTime.Now;
            return DateTime.Parse(self);
        }

        public static TimeSpan NowSpan(this DateTime self)
        {
            return DateTime.Now - self;
        }

        public static string Normal(this DateTime self)
        {
            return self.ToString("yyyy-MM-dd HH:mm:ss");
        }

    }
}