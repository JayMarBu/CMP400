using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningGenerator : MonoBehaviour
{
    [SerializeField] GenerationManager m_genManager;
    [SerializeField, HideInInspector] LightningMeshGenerator m_meshGenerator; 

    public GenerationParameters genParams
    {
        get { return m_genManager.Params; }
        set { m_genManager.Params = value; }
    }

    [SerializeField, HideInInspector] Transform m_startPoint;
    [SerializeField, HideInInspector] Transform m_endPoint;

    [SerializeField] private List<LineSegment> m_lines;

    private RangePair m_halfedAngle;

    private void Start()
    {
        //GenerareLightning();
    }

    public void Clear()
    {
        if(m_lines != null && m_lines.Count > 0)
            m_lines.Clear();

        m_halfedAngle = genParams.Angle / 2;
    }

    public void GenerateMesh() => m_meshGenerator.GenerateMesh(m_lines);

    public void GenerareLightning()
    {
        var timer = new Timer("generation");

        Clear();

        List<LineSegment> previousLayerLines;
        List<LineSegment> currentLayerLines = new List<LineSegment>();

        List<List<LineSegment>> lines = new List<List<LineSegment>>();

        LineSegment line = new LineSegment();
        line.d = genParams.D_init;
        line.p = genParams.P_init;
        line.d_min = (1 / line.p) * BoxMuller.Generate(genParams.gasProperties.A);
        line.p1 = m_startPoint.position;
        line.p2 = m_endPoint.position;

        currentLayerLines.Add(line);

        for (int i = 0; i < genParams.iterations; i++)
        {
            previousLayerLines = new List<LineSegment>(currentLayerLines);
            currentLayerLines.Clear();

            for (int l = 0; l < previousLayerLines.Count; l++ )
            {
                int counter = l % 2;

                Vector3 startPos = previousLayerLines[l].p1;
                Vector3 endPos = previousLayerLines[l].p2;

                float baseAngle = CalculateBaseAngle(previousLayerLines[i]);

                // Jitter
                float length = previousLayerLines[l].length;

                // TODO: something funky is happening with these angles
                float angle = BoxMuller.Generate(genParams.Angle) - genParams.Angle.mean;
                float segLen = CalculateSegmentLength(length, angle);

                Vector3 splitPos = CalculateSplitPoint(previousLayerLines[l]);

                // Fork
                Vector3 forkPos = CalculateForkPoint(startPos, splitPos, endPos, segLen *0.75f);

                // build line segment
                LineSegment[] newLines = new LineSegment[3];

                newLines[0]         = new LineSegment(startPos, splitPos);
                newLines[0].p       = previousLayerLines[l].p;
                newLines[0].d_min   = previousLayerLines[l].d_min;
                newLines[0].d       = CalculateDiameter(previousLayerLines[l].d, newLines[0].d_min, newLines[0].d_min, splitPos.y);
                newLines[0].L       = newLines[0].length;

                newLines[1]         = new LineSegment(splitPos, endPos);
                newLines[1].p       = CalculatePressure(splitPos.y);
                newLines[1].d_min   = CalculateMinDiameter(newLines[0].p);
                newLines[1].d       = CalculateDiameter(newLines[0].d, newLines[1].d_min, newLines[0].d_min, endPos.y);
                newLines[1].L       = newLines[1].length;

                newLines[2]         = new LineSegment(splitPos, forkPos);
                newLines[2].p       = CalculatePressure(splitPos.y);
                newLines[2].d_min   = CalculateMinDiameter(newLines[0].p);
                newLines[2].d       = CalculateDiameter(newLines[0].d, newLines[2].d_min, newLines[0].d_min, forkPos.y);
                newLines[2].L       = newLines[2].length;

                currentLayerLines.Add(newLines[0]);
                currentLayerLines.Add(newLines[1]);
                currentLayerLines.Add(newLines[2]);
            }

            if(i != genParams.iterations - 1)
                lines.Add(currentLayerLines);
        }

        m_lines = currentLayerLines;

        timer.Dispose();

        m_meshGenerator.GenerateMesh(m_lines);
    }

    float CalculatePressure(float y)
        => genParams.P_init - genParams.P_m * (y - m_startPoint.position.y);

    float CalculateMinDiameter(float p)
        => (1 / p) * BoxMuller.Generate(genParams.gasProperties.A);

    float CalculateDiameter(float d_parent, float d_newMin, float d_parentMin, float y)
    {
        // constant is sqrt(1/2)
        var d = 0.70710678118f * d_parent * (d_newMin / d_parentMin);

        return d + (d*0.2f* Mathf.InverseLerp(m_endPoint.position.y, m_startPoint.position.y, y));
    }

    float CalculateBaseAngle(LineSegment line)
    {
        float x = line.p1.x - line.p2.x;
        float y = line.p1.y - line.p2.y;
        float z = line.p1.z - line.p2.z;

        float theta = Mathf.Atan(x/y);

        return -1 * (Mathf.Rad2Deg *theta);
    }

    float CalculateBaseLength(Vector3 startPos, Vector3 endPos)
	{
        float x = startPos.x - endPos.x;
        float y = startPos.y - endPos.y;
        float z = startPos.z - endPos.z;

        float length = Mathf.Sqrt((x * x) + (y * y) + (z * z));

		return length;
	}

    float CalculateSegmentLength(float baseLength, float angle)
	{

        float ang = Mathf.Deg2Rad * angle;
		return (baseLength* 0.5f) / Mathf.Cos(ang);
	}

    Vector3 CalculateSplitPoint(LineSegment parentLine)
	{
        Vector3 D = parentLine.direction;

        float du = Vector3.Dot(D, Vector3.up);
        float df = Vector3.Dot(D, Vector3.forward);
        Vector3 v1 = Mathf.Abs(du) < Mathf.Abs(df) ? Vector3.up : Vector3.forward;

        // cross v1 with D. the new vector is perpendicular to both v1 and A.
        Vector3 v2 = Vector3.Cross(v1, D);

        // rotate v2 around D by a random amount
        float degrees = Random.Range(0.0f, 360.0f);
        v2 = Quaternion.AngleAxis(degrees, D.normalized) * v2;

        float rand = Random.Range(0.25f, 1.25f);

        return parentLine.centre + (v2.normalized * parentLine.length * 0.25f * rand);
	}

    Vector3 CalculateForkPoint(Vector3 startPos, Vector3 splitPos, Vector3 endPos, float len)
    {
        Vector3 parDir = (splitPos - startPos).normalized;

        //return splitPos + (parDir * len);

        Vector3 unitCirclePoint = Random.insideUnitCircle;
        var rot = Quaternion.FromToRotation(Vector3.forward, parDir);
        Vector3 unitConePoint = rot * unitCirclePoint;

        Vector3 dirVector = (splitPos + unitConePoint) - (splitPos);
        dirVector.Normalize();

        float theta = Mathf.Deg2Rad * BoxMuller.Generate(m_halfedAngle);

        return GenerateConeSegment(splitPos, parDir, dirVector, theta, len);
    }

    Vector3 GenerateConeSegment(Vector3 startPoint, Vector3 direction, Vector3 perpendicularVec, float theta, float L)
    {
        float paraOffset = L * Mathf.Cos(theta);
        float perpOffset = L * Mathf.Sin(theta);

        Vector3 paraVec = direction * paraOffset;
        Vector3 perpVec = perpendicularVec * perpOffset;

        return startPoint + paraVec + perpVec;
    }

    private void OnDrawGizmos()
    {
        if (m_lines == null || m_lines.Count <= 0)
            return;

        Gizmos.color = Color.white;

        foreach (var line in m_lines)
        {
            Gizmos.DrawLine(line.p1, line.p2);
        }
    }
}
