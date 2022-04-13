using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineSegment
{
    public LineSegment parent;
    public List<LineSegment> child;

    public float d;
    public float d_min;
    public float L;
    public float p;

    public Vector3 p1;
    public Vector3 p2;

    public void SetByPosition(Vector3 start_point, Vector3 end_point) { p1 = start_point; p2 = end_point; }
}
