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

[System.Serializable]
public class GenerationParameters
{
    public static readonly float nv = 3f / 100f;

    [Header("Editable values")]
    [Tooltip("the initial voltage of the system"), SerializeField] public float V_init;
    [Tooltip("the gas pressure at the starting point"), SerializeField] public float P_init;
    [Tooltip("the gas pressure gradient"), SerializeField] public float P_m;

    [Tooltip("the mean angle and standard deviation between two segments"), SerializeField] public RangePair Angle;

    [Tooltip("properties relating to the local atmosphere"), SerializeField] public GasProperties gasProperties;

    [Tooltip("maximum amount of recursions allowed"), SerializeField] public int maxRecursionDepth;

    [Header("Read-only values")]
    [ReadOnly, Tooltip("the starting diameter of the system"), SerializeField] public float D_init;
}

public class GenerationManager : Singleton<GenerationManager>
{
    [SerializeField] public GenerationParameters Params;

    [SerializeField, HideInInspector] private BaileyetalGen m_baileyEtAlGenerator;
    [SerializeField, HideInInspector] private LightningGenerator m_jitterAndForkGenerator;

    /*private void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 150, 100), "Generate All"))
        {
            print("You clicked the button!");
            GenerateAll();
        }
    }*/

    private void OnValidate()
    {
        Params.D_init = GenerationParameters.nv * Params.V_init;
    }

    public void GenerateAll()
    {
        GenerateBaileyEtAl();
        GenerateJitterAndFork();
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
}
