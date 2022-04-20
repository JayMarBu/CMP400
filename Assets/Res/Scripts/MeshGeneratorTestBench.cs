using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGeneratorTestBench : MonoBehaviour
{
    [SerializeField] Transform m_startPoint;
    [SerializeField] Transform m_midPoint;
    [SerializeField] Transform m_endPoint;
    [SerializeField] Transform m_endPoint2;

    [SerializeField] LightningMeshGenerator meshGenerator;

    private void OnGUI()
    {
        if (GUI.Button(new Rect(10, 40, 150, 20), "Generate test mesh"))
        {
            GenerateMesh();
        }
    }

    public void GenerateMesh()
    {
        List<LineSegment> lines = new List<LineSegment>();

        var lineSegment1 = new LineSegment();

        lineSegment1.p1 = m_startPoint.position;
        lineSegment1.p2 = m_midPoint.position;
        lineSegment1.d = 1f;

        var lineSegment2 = new LineSegment();

        lineSegment2.p1 = m_midPoint.position;
        lineSegment2.p2 = m_endPoint.position;
        lineSegment2.d = 1f;

        var lineSegment3 = new LineSegment();

        lineSegment3.p1 = m_midPoint.position;
        lineSegment3.p2 = m_endPoint2.position;
        lineSegment3.d = 0.7f;

        lines.Add(lineSegment1);
        lines.Add(lineSegment2);
        lines.Add(lineSegment3);

        meshGenerator.GenerateMesh(lines);
    }

}
