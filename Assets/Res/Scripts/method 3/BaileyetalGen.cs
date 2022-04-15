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


        [SerializeField] public RangePair L;
        [SerializeField] public RangePair A;
        //[SerializeField] public float L_mean;
        //[SerializeField] public float L_std;

        //[SerializeField] public float A_mean;
        //[SerializeField] public float A_std;

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
        public static readonly float nv = 3f/100f;

        [Header("Editable values")]
        [Tooltip("the initial voltage of the system"), SerializeField]                          public float V_init;
        [Tooltip("the gas pressure at the starting point"), SerializeField]                     public float P_init;
        [Tooltip("the gas pressure gradient"), SerializeField]                                  public float P_m;

        [Tooltip("the mean angle and standard deviation between two segments"), SerializeField] public RangePair Angle;
        //[Tooltip("the mean angle between two segments"), SerializeField]                        public float Angle_mean;
        //[Tooltip("the standard deviation of the angle between two segments"), SerializeField]   public float Angle_std;


        [Tooltip("properties relating to the local atmosphere"), SerializeField]                public GasProperties gasProperties;

        [Tooltip("maximum amount of recursions allowed"), SerializeField]                       public int maxRecursionDepth;

        [Header("Read-only values")]
        [ReadOnly, Tooltip("the starting diameter of the system"), SerializeField]              public float D_init;
    }

    [SerializeField] public GenerationParameters genParams;

    // private values
    private Vector3 m_initialDirection;
    private RangePair m_halfedAngle;

    [SerializeField] private List<LineSegment> m_finishedList;
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
        m_halfedAngle = genParams.Angle / 2;


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
        line.L = line.d * BoxMuller.Generate(genParams.gasProperties.L);

        line.p1 = startPos;
        line.p2 = startPos + (m_initialDirection * line.L);

        line.p = genParams.P_init;
        line.d_min = (1 / line.p) * BoxMuller.Generate(genParams.gasProperties.A);

        m_segmentQueue.Enqueue(line);

        // recurse generating more
        GenerateChildren();
    }

    public void GenerateChildren(int depth = 0)
    {
        if (m_segmentQueue.Count <= 0)
            return;

        depth++;

        if (depth >= genParams.maxRecursionDepth)
            return;

        LineSegment parentLine = m_segmentQueue.Dequeue();

        var line1 = GenerateNewSegment(parentLine);

        if (line1.d > line1.d_min)
            m_segmentQueue.Enqueue(line1);

        var line2 = GenerateNewSegment(parentLine);

        if (line2.d > line2.d_min)
            m_segmentQueue.Enqueue(line2);

        m_finishedList.Add(parentLine);

        GenerateChildren(depth);
    }

    LineSegment GenerateNewSegment(LineSegment parentLine)
    {
        // generate perpendicular offset on cone
        Vector3 D = parentLine.direction;

        Vector3 unitCirclePoint = Random.insideUnitCircle.normalized;
        var rot = Quaternion.FromToRotation(Vector3.forward, D);
        Vector3 UnitConePoint = rot * unitCirclePoint;

        Vector3 dirVector = (parentLine.p2 + UnitConePoint) - (parentLine.p2);
        dirVector.Normalize();

        // generate angle
        float theta = Mathf.Deg2Rad * BoxMuller.Generate(m_halfedAngle);

        // create new line segment
        LineSegment line = new LineSegment();

        line.p      = CalculatePressure(parentLine.p2.y);
        line.d_min  = CalculateMinDiameter(line.p);

        line.d      = CalculateDiameter(parentLine.d, line.d_min, parentLine.d_min);

        line.L      = line.d * BoxMuller.Generate(genParams.gasProperties.L);

        line.p1     = parentLine.p2;
        line.p2     = GenerateConeSegment(line.p1, D, dirVector, theta, line.L); ;

        return line;
    }

    float CalculatePressure(float y) 
        => genParams.P_init - genParams.P_m * (y - startPos.y);

    float CalculateMinDiameter(float p) 
        => (1 / p) * BoxMuller.Generate(genParams.gasProperties.A);
    
    float CalculateDiameter(float d_parent, float d_newMin, float d_parentMin)
        // constant is sqrt(1/2)
        => 0.70710678118f * d_parent * (d_newMin / d_parentMin);

    Vector3 GenerateConeSegment(Vector3 startPoint, Vector3 direction, Vector3 perpendicularVec, float theta, float L)
    {
        float paraOffset    = L * Mathf.Cos(theta);
        float perpOffset    = L * Mathf.Sin(theta);

        Vector3 paraVec     = direction * paraOffset;
        Vector3 perpVec     = perpendicularVec * perpOffset;

        return startPoint + paraVec + perpVec;
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
                Gizmos.color = Color.white;
                Gizmos.DrawLine(line.p1, line.p2);
                Gizmos.DrawSphere(line.p2, 0.1f);
            }
        }
    }
}
