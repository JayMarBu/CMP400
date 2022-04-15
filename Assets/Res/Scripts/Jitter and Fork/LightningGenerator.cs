using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*struct JFLineSegment
{
    public Vector3 p1;
    public Vector3 p2;
    public Color colour;

    public JFLineSegment(int inVal = 0)
    {
        p1 = Vector3.zero;
        p2 = Vector3.zero;
        colour = Color.white;
    }

    public void SetByPosition(Vector3 start_point, Vector3 end_point) { p1 = start_point; p2 = end_point; }
    public void SetByDirection(Vector3 start_point, Vector3 direction_vector, float length)
    {
        p1 = start_point;
        p2 = p1 + (Vector3.Normalize(direction_vector) * length);
    }
    //LineSegment Transform(Transform transform);

    public void SetColour(Color in_colour) { colour = in_colour; }
};*/

public class LightningGenerator : MonoBehaviour
{
    [SerializeField, HideInInspector] Transform m_startPoint;
    [SerializeField, HideInInspector] Transform m_endPoint;

    [SerializeField] private List<LineSegment> m_lines;

    public float Angle;
    public float StandardDeviation;
    public int Iterations;

    private void Start()
    {
        GenerareLightning();
    }

    public void Clear()
    {
        if(m_lines != null && m_lines.Count > 0)
            m_lines.Clear();
    }

    public void GenerareLightning()
    {
        List<LineSegment> previousLayerLines;
        List<LineSegment> currentLayerLines = new List<LineSegment>();

        List<List<LineSegment>> lines = new List<List<LineSegment>>();

        LineSegment line = new LineSegment();
        line.p1 = m_startPoint.position;
        line.p2 = m_endPoint.position;

        currentLayerLines.Add(line);

        for (int i = 0; i < Iterations; i++)
        {
            previousLayerLines = new List<LineSegment>(currentLayerLines);
            currentLayerLines.Clear();

            for (int l = 0; l < previousLayerLines.Count; l++ )
            {
                int counter = l % 2;

                float r = 0.25f + (Mathf.InverseLerp(0f, (float)Iterations, (float)i)*0.75f);
                float gb = Mathf.InverseLerp(0, (float)Iterations + 1, (float)i + 1);

                Vector3 startPos = previousLayerLines[l].p1;
                Vector3 endPos = previousLayerLines[l].p2;

                float baseAngle = CalculateBaseAngle(previousLayerLines[i]);

                // Jitter
                float length = CalculateBaseLength(startPos, endPos);

                // TODO: something funky is happening with these angles
                float angle = BoxMuller.Generate(Angle, StandardDeviation) - Angle;
                float segLen = CalculateSegmentLength(length, angle);

                float trueAngle = (counter == 0) ? angle + baseAngle : angle - baseAngle;

                Vector3 splitPos = CalculateSplitPoint(startPos, segLen, trueAngle);

                // Fork
                Vector3 forkPos = CalculateForkPoint(splitPos, endPos, segLen *0.5f);

                // build line segment
                LineSegment[] newLines = new LineSegment[3];

                newLines[0] = new LineSegment(startPos, splitPos);
                newLines[1] = new LineSegment(splitPos, endPos);
                newLines[2] = new LineSegment(splitPos, forkPos);

                currentLayerLines.Add(newLines[0]);
                currentLayerLines.Add(newLines[1]);
                currentLayerLines.Add(newLines[2]);
            }

            if(i != Iterations - 1)
                lines.Add(currentLayerLines);
        }

        m_lines = currentLayerLines;
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

    Vector3 CalculateSplitPoint(Vector3 startPos, float segment_length, float angle)
	{
		/*
		 xlen = opp
		 ylen = adj
		 segment_length = hyp
		 angle = theta

		 opp = hyp * sin(theta)
		 adj = hyp * cos(theta)
		*/
		float xlen, ylen, zlen;

        Vector2 xz = Random.insideUnitCircle.normalized;

        xlen = segment_length * Mathf.Sin(Mathf.Deg2Rad * angle);
        ylen = segment_length * Mathf.Cos(Mathf.Deg2Rad * angle);

        return new Vector3(startPos.x - (xlen*xz.x), startPos.y - ylen, startPos.z - (xlen*xz.y));
	}

    Vector3 CalculateForkPoint(Vector3 splitPos, Vector3 endPos, float len)
    {
        /*
		 xlen = opp
		 ylen = adj
		 segment_length = hyp
		 angle = theta

		 opp = hyp * sin(theta)
		 adj = hyp * cos(theta)
		*/

        Vector3 dirVec = Vector3.Normalize(splitPos - endPos);
        dirVec.y *= -1;
        Vector3 forkPos = splitPos + (dirVec * len);

        return forkPos;
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
