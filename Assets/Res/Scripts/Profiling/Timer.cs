using System.Diagnostics;
using System;

public class Timer : IDisposable
{
    private Stopwatch sw;
    string name;

    public Timer()
    {
        name = "stopwatch";
        sw = new Stopwatch();
        sw.Start();
    }

    public Timer(string in_name)
    {
        name = in_name;
        sw = new Stopwatch();
        sw.Start();
    }

    public void Dispose() => Stop();

    public void Stop()
    {
        sw.Stop();
        ShowTime(sw.ElapsedTicks, "Timer: " + name + ", elapsed in: ");
    }

    public static void ShowTime(double ticks, string message)
    {
        double milliseconds = (ticks / Stopwatch.Frequency) * 1000;
        double nanoseconds = (ticks / Stopwatch.Frequency) * 1000000000;
        UnityEngine.Debug.Log(message + "\n " + milliseconds + "ms" + " [" + nanoseconds + "ns]");
    }

    public TimePair GetTime()
    {
        TimePair tp = new TimePair();
        tp.milliseconds = (sw.ElapsedTicks / Stopwatch.Frequency) * 1000;
        tp.nanoseconds = (sw.ElapsedTicks / Stopwatch.Frequency) * 1000000000;
        return tp;
    }
}

public struct TimePair
{
    public double milliseconds;
    public double nanoseconds;

    public TimePair(double ms, double ns)
    {
        milliseconds    = ms;
        nanoseconds     = ns;
    }
}