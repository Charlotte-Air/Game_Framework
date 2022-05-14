using System;
using System.Runtime.InteropServices;

class Time
{
    [DllImport("kernel32.dll")]
    private static extern bool QueryPerformanceCounter([In, Out] ref long lpPerformanceCount);

    [DllImport("kernel32.dll")]
    private static extern bool QueryPerformanceFrequency([In, Out] ref long lpFrequency);

    private static long freq = 0;
    private static float _time = 0;
    private static long lastTick = 0;
    private static float _deltaTime = 0;
    private static long _frameCount = 0; //通过的帧总数
    private static long startupTicks = 0;

    /// <summary>
    /// 帧开始的时间(只读)
    /// 这是以秒为单位的时间
    /// 从游戏开始
    /// </summary> 
    public static float time { get => _time; }
    public static float deltaTime { get => _deltaTime; }
    public static long frameCount { get => _frameCount; }
    /// <summary>
    /// 时间戳
    /// </summary>
    static public long ticks
    {
        get
        {
            long f = freq;
            if (f == 0)
            {
                if (QueryPerformanceFrequency(ref f))
                    freq = f;
                else
                    freq = -1;
            }
            if (f == -1)
            {
                return Environment.TickCount * 10000;
            }
            long c = 0;
            QueryPerformanceCounter(ref c);
            return (long) (((double) c) * 1000 * 10000 / ((double) f));
        }
    }

    /// <summary>
    /// 从开始到现在的实时时间(只读)
    /// </summary>
    public static float realtimeSinceStartup
    {
        get
        {
            long _ticks = ticks;
            return (_ticks - startupTicks) / 10000000f;
        }
    }
    static Time() => startupTicks = ticks;

    public static void Tick()
    {
        long _ticks = ticks;
        _frameCount++;
        if (_frameCount == long.MaxValue)
        {
            _frameCount = 0;
        }
        if (lastTick == 0)
        {
            lastTick = _ticks;
        }
        _deltaTime = (_ticks - lastTick) / 10000000f;
        _time = (_ticks - startupTicks) / 10000000f;
        lastTick = _ticks;
    }
}
