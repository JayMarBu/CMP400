using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct GasProperties
{
    public static GasProperties Air = new GasProperties(11, 4, 0.2f, 0.02f);
    public static GasProperties N2 = new GasProperties(9, 3, 0.12f, 0.03f);

    [SerializeField] public RangePair L;
    [SerializeField] public RangePair A;

    public GasProperties(RangePair l, RangePair a)
    {
        L = l;
        A = a;
    }

    public GasProperties(float l_mean, float l_std, float a_mean, float a_std)
    {
        L.mean = l_mean;
        L.std = l_std;

        A.mean = a_mean;
        A.std = a_std;
    }
}

public enum TeselationMod
{
    None            = 0,
    Jitter          = 1,
    Random_Offset   = 2
}

[System.Serializable]
public class GenerationParameters
{
    public static readonly float nv = 3f / 100f;

    [Header("Universal parameters")]
    [Tooltip("the initial voltage of the system"), SerializeField]                          public float V_init;
    [Tooltip("the gas pressure at the starting point"), SerializeField]                     public float P_init;
    [Tooltip("the gas pressure gradient"), SerializeField]                                  public float P_m;

    [Tooltip("the mean angle and standard deviation between two segments"), SerializeField] public RangePair Angle;

    [Tooltip("properties relating to the local atmosphere"), SerializeField]                public GasProperties gasProperties;

    [Header("Bailey et al parameters")]
    [Tooltip("maximum amount of recursions allowed"), SerializeField]                       public int maxRecursionDepth;

    [Header("Jitter and Fork parameters")]
    [Tooltip("the number of iterations to be used"), SerializeField]                        public int iterations;

    [Header("Render Parameters")]
    [SerializeField]                                                                        public float diameterThinner;

    [Space]

    [SerializeField, Min(0.01f)]                                                            public float jitterPerUnit;
    [SerializeField]                                                                        public float jitterSizeModifier;
    [SerializeField, Min(1)]                                                                public int jitterMaxDepth;
    [SerializeField, Min(1)]                                                                public int jitterMinDepth;
    [SerializeField]                                                                        public TeselationMod jitterMode;

    [Header("Read-only values")]
    [ReadOnly, Tooltip("the starting diameter of the system"), SerializeField]              public float D_init;

    public void SetTestData(TestParameters p)
    {
        maxRecursionDepth   = p.maxRecursionDepth;
        iterations          = p.iterations;

        jitterPerUnit       = p.jitterPerUnit;
        jitterMaxDepth      = p.jitterMaxDepth;
        jitterMinDepth      = p.jitterMinDepth;
        jitterMode          = p.jitterMode;
    }
}

public class GenerationManager : Singleton<GenerationManager>
{
    [SerializeField] public GenerationParameters Params;

    [SerializeField, HideInInspector] private BaileyetalGen m_baileyEtAlGenerator;
    [SerializeField, HideInInspector] private LightningGenerator m_jitterAndForkGenerator;

    bool hasGenerated = false;

    private void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 150, 20), "Generate All"))
        {
            hasGenerated = true;
            GenerateAll();
        }

        if (hasGenerated) 
        {
            if (GUI.Button(new Rect(10, 40, 150, 20), "Re-Generate Mesh"))
            {
                ReGenerateAllMeshes();
            }
        }
    }

    private void OnValidate()
    {
        Params.D_init = GenerationParameters.nv * Params.V_init;
    }

    private void Start()
    {
        hasGenerated = false;
    }

    public void GenerateAll()
    {
        GenerateBaileyEtAl();
        GenerateJitterAndFork();
    }

    public void ReGenerateAllMeshes()
    {
        ReGenerateBaileyEtAlMesh();
        ReGenerateJitterAndForkMesh();
    }

    public void Clear()
    {
        m_baileyEtAlGenerator.Clear();
        m_jitterAndForkGenerator.Clear();
    }

    public void GenerateBaileyEtAl()
    {
        Params.D_init = GenerationParameters.nv * Params.V_init;
        m_baileyEtAlGenerator.Generate();
    }

    public void GenerateJitterAndFork()
    {
        Params.D_init = GenerationParameters.nv * Params.V_init;
        m_jitterAndForkGenerator.GenerareLightning();
    }

    public void ReGenerateBaileyEtAlMesh()
    {
        Params.D_init = GenerationParameters.nv * Params.V_init;
    }

    public void ReGenerateJitterAndForkMesh()
    {
        Params.D_init = GenerationParameters.nv * Params.V_init;
    }
}
