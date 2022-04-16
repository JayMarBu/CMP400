using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGeneratorTestBench : MonoBehaviour
{
    [SerializeField] Transform m_startPoint;
    [SerializeField] Transform m_endPoint;

    [SerializeField] LightningMeshGenerator meshGenerator;

    [SerializeField] LineSegment lineSegment;

    private void OnGUI()
    {
        if (GUI.Button(new Rect(10, 40, 150, 20), "Generate test mesh"))
        {
            GenerateMesh();
        }
    }

    private void Update()
    {
        lineSegment.p1 = m_startPoint.position;
        lineSegment.p2 = m_endPoint.position;
    }

    public void GenerateMesh()
    {
        List<LineSegment> lines = new List<LineSegment>();

        lineSegment.p1 = m_startPoint.position;
        lineSegment.p2 = m_endPoint.position;

        lines.Add(lineSegment);

        meshGenerator.GenerateMesh(lines);
    }

}
