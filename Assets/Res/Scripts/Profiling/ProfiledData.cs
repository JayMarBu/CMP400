using Unity.Profiling;
using System.Text;

public class ProfiledData
{
    bool enabled = false;
    string name;
    public string text;

    public int segmentCount;
    public int depth;
    public TeselationMod tesslMod;

    public readonly ProfilerMarker generationMarker;
    public readonly ProfilerMarker meshMarker;
    public readonly ProfilerMarker tesslMarker;

    public ProfilerRecorder generationRecorder;
    public ProfilerRecorder meshRecorder;
    public ProfilerRecorder tesslRecorder;

    public ProfiledData(string n)
    {
        name = n;
        generationMarker    = new ProfilerMarker(name + ": generation");
        meshMarker          = new ProfilerMarker(name + ": mesh");
        tesslMarker         = new ProfilerMarker(name + ": tessl");

        OnEnable();
    }

    ~ProfiledData()
    {
        OnDisable();
    }

    public void OnEnable()
    {
        if (enabled)
            return;

        enabled = true;
        generationRecorder  = ProfilerRecorder.StartNew(generationMarker);
        meshRecorder        = ProfilerRecorder.StartNew(meshMarker);
        tesslRecorder       = ProfilerRecorder.StartNew(tesslMarker);
    }

    public void OnDisable()
    {
        if (!enabled)
            return;

        enabled = false;
        generationRecorder.Dispose();
        meshRecorder.Dispose();
        tesslRecorder.Dispose();
    }

    public void CreateDebugText()
    {
        var sb = new StringBuilder(500);
        sb.AppendLine($"{name}: generation: {GetRecorderFrameAverage(generationRecorder) * (1e-6f):F1} ms");
        sb.AppendLine($"{name}: mesh: {GetRecorderFrameAverage(meshRecorder) * (1e-6f):F1} ms");
        sb.AppendLine($"{name}: tessl: {GetRecorderFrameAverage(tesslRecorder) * (1e-6f):F1} ms");
        text = sb.ToString();
    }

    static double GetRecorderFrameAverage(ProfilerRecorder recorder)
    {
        var samplesCount = recorder.Capacity;
        if (samplesCount == 0)
            return 0;

        double r = 0;
        unsafe
        {
            var samples = stackalloc ProfilerRecorderSample[samplesCount];
            recorder.CopyTo(samples, samplesCount);
            for (var i = 0; i < samplesCount; ++i)
                r += samples[i].Value;
            r /= samplesCount;
        }

        return r;
    }
}
