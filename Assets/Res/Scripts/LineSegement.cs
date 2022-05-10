using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class LineSegment
{
    [ReadOnly] public float d;
    [ReadOnly] public float d_min;
    [ReadOnly] public float L;
    [ReadOnly] public float p;

    [ReadOnly] public Vector3 p1;
    [ReadOnly] public Vector3 p2;

    public LineSegment()
    {
        
    }

    public LineSegment(Vector3 start_point, Vector3 end_point)
    {
        p1 = start_point;
        p2 = end_point;
    }

    public Vector3 direction
    {
        get { return Vector3.Normalize(p2-p1); }
    }

    public float length
    {
        get { return Vector3.Magnitude(p2-p1); }
    }

    public Vector3 centre
    {
        get { return Vector3.Lerp(p1, p2, 0.5f); }
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
