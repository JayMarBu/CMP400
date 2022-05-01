using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public struct TestResult
{
    TimePair generation;
    TimePair mesh;
}

public class MyProfiler : Singleton<MyProfiler>
{
    [ReadOnly] int testDuration     = 100;
    [SerializeField] int frameBreak = 5;

    int currentTestDuration         = 0;
    int currentFrameBreak           = 0;

    
    [ReadOnly] public bool testInProgress = false;

    public CSVWriter writer;
    public Queue<TimePair> timePairs = new Queue<TimePair>();

    [SerializeField] public List<PerformanceTest> Tests = new List<PerformanceTest>();

    PerformanceTest currentTest;

    void Update()
    {
        if (!testInProgress)
            return;

        currentFrameBreak++;

        if (currentFrameBreak > frameBreak)
        {
            currentFrameBreak = 0;
            ContinueTesting();
        }
        
    }

    public void BeginTest()
    {
        if (testInProgress)
            return;

        if (Tests.Count <= 0)
            return;

        currentTest = Tests[0];
        Tests.RemoveAt(0);

        testDuration = currentTest.testIterations;

        currentTestDuration = 0;
        currentFrameBreak = 0;

        testInProgress = true;

        writer = new CSVWriter(currentTest.name + ".csv");

        var headerSB = new StringBuilder(500);
        var subHeaderSB = new StringBuilder(500);

        GenerationManager.Instance.Params.SetTestData(currentTest.genParams);

        foreach(var header in currentTest.headers)
        {
            headerSB.Append(header.name);
            foreach (var subHeader in header.subHeaders)
            {
                headerSB.Append(",");
                subHeaderSB.Append(subHeader + ",");
            }
        }

        writer.WriteLine(headerSB.ToString(), false);
        writer.WriteLine(subHeaderSB.ToString());
    }

    void ContinueTesting()
    {
        switch (currentTest.type)
        {
            case PerformanceTest.TestType.JF:
                GenerationManager.Instance.GenerateJitterAndFork();
                break;
            case PerformanceTest.TestType.BEA:
                GenerationManager.Instance.GenerateBaileyEtAl();
                break;
        }

        WriteData();

        currentTestDuration++;
        if (currentTestDuration >= testDuration)
        {
            testInProgress = false;
            // Finish Test
            BeginTest();
        }
    }

    void WriteData()
    {
        var sb = new StringBuilder(500);
        for (int i = timePairs.Count; i > 0; i--)
        {
            var pair = timePairs.Dequeue();
            sb.Append(pair.milliseconds + "," + pair.nanoseconds + ",");
        }
        writer.WriteLine(sb.ToString());
    }
}
