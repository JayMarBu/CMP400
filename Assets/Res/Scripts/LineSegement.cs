using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LineSegment
{
    [HideInInspector] public LineSegment parent;
    [HideInInspector] public List<LineSegment> child;

    [ReadOnly] public float d;
    [ReadOnly] public float d_min;
    [ReadOnly] public float L;
    [ReadOnly] public float p;

    [ReadOnly] public Vector3 p1;
    [ReadOnly] public Vector3 p2;

    public Vector3 V;

    public LineSegment()
    {
        
    }

    public LineSegment(Vector3 start_point, Vector3 end_point)
    {
        SetByPosition(start_point, end_point);
    }

    public void SetByPosition(Vector3 start_point, Vector3 end_point) { p1 = start_point; p2 = end_point; }

    public Vector3 direction
    {
        get { return Vector3.Normalize(p2-p1); }
    }
}

[System.Serializable]
public struct RangePair
{
    [SerializeField] public float mean;
    [SerializeField] public float std;

    public RangePair(float m, float s)
    {
        mean = m;
        std = s;
    }

    public RangePair(float m)
    {
        mean = m;
        std = 0;
    }

    public static RangePair operator *(RangePair a, float b) => new RangePair(a.mean * b, a.std * b);

    public static RangePair operator /(RangePair a, float b)
    {
        if(b == 0)
            throw new DivideByZeroException();
        return new RangePair(a.mean/b, a.std/b);
    }
}
