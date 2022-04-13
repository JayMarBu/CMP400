using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaileyetalGen : MonoBehaviour
{
    [SerializeField] Transform m_startPoint;
    [SerializeField] Transform m_endPoint;

    public Vector3 startPos { get { return m_startPoint.position; } }
    public Vector3 endPos { get { return m_endPoint.position; } }

    [System.Serializable]
    public struct GasProperties
    {
        public static GasProperties Air     = new GasProperties(11, 4, 0.2f, 0.02f);
        public static GasProperties N2      = new GasProperties(9, 3, 0.12f, 0.03f);

        [SerializeField] public float L_mean;
        [SerializeField] public float L_std;

        [SerializeField] public float A_mean;
        [SerializeField] public float A_std;

        public GasProperties(float l_mean, float l_std, float a_mean, float a_std)
        {
            L_mean = l_mean;
            L_std = l_std;

            A_mean = a_mean;
            A_std = a_std;
        }
    }

    [System.Serializable]
    public class GenerationParameters
    {
        public static readonly float nv = 3f/100f;

        [Header("Editable values")]
        [Tooltip("the initial voltage of the system"), SerializeField]                          public float V_init;
        [Tooltip("the gas pressure at the starting point"), SerializeField]                     public float P_init;
        [Tooltip("the gas pressure gradient"), SerializeField]                                  public float P_m;

        [Tooltip("the mean angle between two segments"), SerializeField]                        public float Angle_mean;
        [Tooltip("the standard deviation of the angle between two segments"), SerializeField]   public float Angle_std;


        [Tooltip("properties relating to the local atmosphere"), SerializeField]                public GasProperties gasProperties;

        [Tooltip("maximum amount of recursions allowed"), SerializeField]                       public int maxRecursionDepth;

        [Header("Read-only values")]
        [ReadOnly, Tooltip("the starting diameter of the system"), SerializeField]              public float D_init;
    }

    [SerializeField] public GenerationParameters genParams;

    // private values
    private Vector3 m_initialDirection;

    private List<LineSegment> m_finishedList;
    private Queue<LineSegment> m_segmentQueue;

    private void OnValidate()
    {
        genParams.D_init = GenerationParameters.nv * genParams.V_init;
    }

    private void Awake()
    {
        
    }

    public void Clear()
    {
        m_initialDirection = (endPos - startPos).normalized;
        
        m_finishedList = new List<LineSegment>();
        m_segmentQueue = new Queue<LineSegment>();
    }

    public void Generate()
    {
        Clear();

        // calculate initial values
        genParams.D_init = GenerationParameters.nv * genParams.V_init;

        // generate first line segment
        LineSegment line = new LineSegment();

        line.d = genParams.D_init;
        line.L = line.d * BoxMuller.Generate(genParams.gasProperties.L_mean, genParams.gasProperties.L_std);

        line.p1 = startPos;
        line.p2 = startPos + (m_initialDirection * line.L);

        line.p = genParams.P_init;
        line.d_min = (1 / line.p) * (BoxMuller.Generate(genParams.gasProperties.A_mean, genParams.gasProperties.A_std));

        m_finishedList.Add(line);

        // recurse generating more
        GenerateChildren(line, 0);
    }

    public void GenerateChildren(LineSegment parentLine, int depth)
    {
        depth++;

        if (depth >= genParams.maxRecursionDepth)
            return;

        // generate cone


    }

    private void OnDrawGizmos()
    {
        Gizmos.color = m_startPoint.GetComponent<LightningPointGizmo>().colour;
        Gizmos.DrawLine(startPos, endPos);

        Gizmos.color = Color.white;

        if(m_finishedList != null)
        {
            foreach (var line in m_finishedList)
            {
                Gizmos.DrawLine(line.p1, line.p2);
            }
        }
    }
}
