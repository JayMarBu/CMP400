using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TestParameters
{
    [SerializeField] public int maxRecursionDepth;
    [SerializeField] public int iterations;

    [Header("Render Parameters")]

    [SerializeField, Min(0.01f)] public float jitterPerUnit;
    [SerializeField, Min(1)] public int jitterMaxDepth;
    [SerializeField, Min(1)] public int jitterMinDepth;
    [SerializeField] public TeselationMod jitterMode;

    public TestParameters()
    {
        maxRecursionDepth   = 100;
        iterations          = 3;
        jitterPerUnit       = 2;
        jitterMaxDepth      = 5;
        jitterMinDepth      = 6;
        jitterMode          = TeselationMod.Jitter;
    }
}

[System.Serializable]
public class PerformanceTest
{
    [SerializeField] public string name         = "";
    [SerializeField] public int testIterations  = 10;

    [System.Serializable]
    public class Header
    {
        [SerializeField] public string name             = "";
        [SerializeField] public List<string> subHeaders = new List<string>(){ "ms", "ns"};

        public Header()
        {
            name        = "";
            subHeaders  = new List<string>() { "ms", "ns" };
        }
    }

    [SerializeField] public List<Header> headers = new List<Header>() 
    {
        new Header(){name = "generation"},
        new Header(){name = "mesh"}
    };

    [SerializeField] public TestParameters genParams = new TestParameters();

    public enum TestType
    {
        JF,
        BEA
    }

    [SerializeField] public TestType type;

    public PerformanceTest()
    {
        name            = "";
        testIterations  = 10;
        headers         = new List<Header>()
        {
            new Header(){name = "generation"},
            new Header(){name = "mesh"}
        };
        genParams       = new TestParameters();
    }
}
