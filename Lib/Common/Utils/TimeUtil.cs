namespace Common.Utils
{
    public class TimeUtil
    {
        /// <summary>
        /// 时间戳
        /// </summary>
        public static double timestamp { get => GetTimestamp(System.DateTime.Now); }

        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <returns>返回DateTime 时间戳</returns>
        public static System.DateTime GetTime(long timeStamp)
        {
            System.DateTime dateTimeStart = System.TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            long lTime = timeStamp * 10000000;
            System.TimeSpan toNow = new System.TimeSpan(lTime);
            return dateTimeStart.Add(toNow);
        }

        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <param name="time">DateTime</param>
        /// <returns>double 时间戳</returns>
        public static double GetTimestamp(System.DateTime time)
        {
            System.DateTime startTime = System.TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return (time - startTime).TotalSeconds;
        }

        /// <summary>
        /// 将秒数转换为 {0}小时{1}分钟{2}秒 
        /// </summary>
        public static string GetTimeDown(uint second)
        {
            string str = "";
            int hour = UnityEngine.Mathf.FloorToInt(second / 3600);
            int min = UnityEngine.Mathf.FloorToInt((second - 3600 * hour) / 60);
            int sec = (int)(second % 60);
            if (hour != 0)
            {
                str += Lang.Get("LANG_COMMON_TIME_H", hour);
            }
            if (min != 0)
            {
                str += Lang.Get("LANG_COMMON_TIME_M", min);
            }
            if (sec != 0)
            {
                str += Lang.Get("LANG_COMMON_TIME_S", sec);
            }
            return str;
        }
    }
}
