using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyProfiler : Singleton<MyProfiler>
{
    [SerializeField]            string testName;
    [SerializeField, Min(10)]   int testDuration = 100;
    [SerializeField, Min(1)]    int frameBreak = 5;

    int currentTestDuration     = 0;
    int currentFrameBreak       = 0;
    enum TestType
    {
        JF,
        BEA,
        BOTH
    }
    [SerializeField] TestType testType;
    [ReadOnly] public bool testInProgress = false;

    public CSVWriter writer;

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

        currentTestDuration = 0;
        currentFrameBreak = 0;

        testInProgress = true;

        writer = new CSVWriter(testName + ".csv");

        writer.WriteLine("generation,,mesh,,", false);
        writer.WriteLine("ms, ns, ms, ns,");
    }

    void ContinueTesting()
    {
        switch (testType)
        {
            case TestType.JF:
                GenerationManager.Instance.GenerateJitterAndFork();
                break;
            case TestType.BEA:
                GenerationManager.Instance.GenerateBaileyEtAl();
                break;
            case TestType.BOTH:
                GenerationManager.Instance.GenerateAll();
                break;
        }

        currentTestDuration++;
        if (currentTestDuration >= testDuration)
        {
            testInProgress = false;
            // Finish Test

        }
    }
}
