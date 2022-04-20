using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaileyetalGen : MonoBehaviour
{
    [SerializeField ] GenerationManager m_genManager;
    [SerializeField, HideInInspector] LightningMeshGenerator m_meshGenerator;

    [SerializeField, HideInInspector] Transform m_startPoint;
    [SerializeField, HideInInspector] Transform m_endPoint;

    public Vector3 startPos { get { return m_startPoint.position; } }
    public Vector3 endPos { get { return m_endPoint.position; } }

    public GenerationParameters genParams 
    { 
        get { return m_genManager.Params; }
        set { m_genManager.Params = value; }
    }

    // private values
    private Vector3 m_initialDirection;
    private RangePair m_halfedAngle;

    [SerializeField] private List<LineSegment> m_finishedList;
    private Queue<LineSegment> m_segmentQueue;

    public void Clear()
    {
        m_initialDirection = (endPos - startPos).normalized;
        m_halfedAngle = genParams.Angle / 2;


        m_finishedList = new List<LineSegment>();
        m_segmentQueue = new Queue<LineSegment>();
    }

    public void GenerateMesh() => m_meshGenerator.GenerateMesh(m_finishedList);

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

        m_meshGenerator.GenerateMesh(m_finishedList);
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
