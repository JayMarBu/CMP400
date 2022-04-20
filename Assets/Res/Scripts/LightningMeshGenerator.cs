using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct TesslInfo
{
	public int count;
	public Vector3[] segmentPositions;
}

public struct JitterSegment
{
	public Vector3 centrePoint;
	public int[] segmentVertIndices;
}

public struct CapsuleData
{
	public int verticalSegments;
	public float r;
	public float h;
	public int[] triangles;
	public Vector3[] vertices;
	public JitterSegment[] jitterSegements;
}

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class LightningMeshGenerator : MonoBehaviour
{
    [SerializeField] public int segmentsPerCapsule = 24;
    [SerializeField] public int verticalPerCapsule = 2;
    [SerializeField] public bool useManualSegmentCount = false;

    public void GenerateMesh(List<LineSegment> lineSegments)
    {
		List<Vector3> vertices = new List<Vector3>();
		List<int> triangles = new List<int>();

		// - Generate Mesh -

		int offset = 0;
		foreach (var segment in lineSegments)
        {
			// Generate Capsule
			var data = GenerateCapsule(segment, offset);

			// Jitter Capsule
			if(GenerationManager.Instance.Params.jitterGeometry)
            {
				JitterCapsule(segment, ref data);
            }

			vertices.AddRange(data.vertices);
			triangles.AddRange(data.triangles);
			offset += data.vertices.Length;
        }

		// - Assign Mesh -

		MeshFilter mf = gameObject.GetComponent<MeshFilter>();
		Mesh mesh = mf.sharedMesh;
		if (!mesh)
		{
			mesh = new Mesh();
			mf.sharedMesh = mesh;
		}
		mesh.Clear();

		mesh.name = "ProceduralCapsule";

		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();

		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
		mesh.Optimize();
	}

	public CapsuleData GenerateCapsule(LineSegment lineSegment, int offset)
    {
		var data = new CapsuleData();

		// set mesh parameters from line segment
		float height = (lineSegment.length + lineSegment.d) * 1.005f;
		float radius = lineSegment.d * 0.5f;

		// calculate tessellation points
		var tesslInfo = GenerateCapsuleTessellationPointCount(height, radius);

		int segments = segmentsPerCapsule;

		Quaternion rot = Quaternion.FromToRotation(Vector3.up, lineSegment.direction);
		Vector3 position = lineSegment.centre - transform.position ;

		// make segments an even number
		if (segments % 2 != 0)
			segments++;

		if(GenerationManager.Instance.Params.jitterMode != TeselationMod.None)
			segments++;
		//segments += tesslInfo.count;

		// extra vertex on the seam
		int points = segments + 1;

		// calculate points around a circle
		float[] pX = new float[points];
		float[] pZ = new float[points];
		float[] pY = new float[points];
		float[] pR = new float[points];

		float calcH = 0f;
		float calcV = 0f;

		for (int i = 0; i < points; i++)
		{
			pX[i] = Mathf.Sin(calcH * Mathf.Deg2Rad);
			pZ[i] = Mathf.Cos(calcH * Mathf.Deg2Rad);
			pY[i] = Mathf.Cos(calcV * Mathf.Deg2Rad);
			pR[i] = Mathf.Sin(calcV * Mathf.Deg2Rad);

			calcH += 360f / (float)segments;
			calcV += 180f / (float)segments;
		}

		// - Vertices -
		int vertTesslOffset = (points + 1) * Mathf.Max(0, tesslInfo.count-1);
		data.vertices = new Vector3[points * (points + 1) + vertTesslOffset];
		int ind = 0;

		// Y-offset is half the height minus the diameter
		float yOff = (height - (radius * 2f)) * 0.5f;
		if (yOff < 0)
			yOff = 0;

		// Top Hemisphere
		int top = Mathf.CeilToInt((float)points * 0.5f);

		for (int y = 0; y < top; y++)
		{
			for (int x = 0; x < points; x++)
			{
				data.vertices[ind] = new Vector3(pX[x] * pR[y], pY[y], pZ[x] * pR[y]) * radius;
				data.vertices[ind].y = yOff + data.vertices[ind].y;
				data.vertices[ind] = rot * data.vertices[ind];
				data.vertices[ind] += position;

				ind++;
			}
		}

		// middle rings
		GenerateVerticalSegments(
			ref data,
			ref ind,
			height, radius, yOff,
			tesslInfo,
			points,
			rot, position,
			pX, pZ, pY[top-1], pR[top-1]);

		// Bottom Hemisphere
		int btm = Mathf.FloorToInt((float)points * 0.5f);

		for (int y = btm; y < points; y++)
		{
			for (int x = 0; x < points; x++)
			{
				data.vertices[ind] = new Vector3(pX[x] * pR[y], pY[y], pZ[x] * pR[y]) * radius;
				data.vertices[ind].y = -yOff + data.vertices[ind].y;
				data.vertices[ind] = rot * data.vertices[ind];
				data.vertices[ind] += position;

				ind++;
			}
		}


		// - Triangles -
		int triTesslOffset = (segments + 1) * Mathf.Max(0, tesslInfo.count-1);
		data.triangles = new int[(segments * (segments + 1) * 2 * 3)+(triTesslOffset * 2 * 3)];

		for (int y = 0, t = 0; y < segments + Mathf.Max(1, tesslInfo.count); y++)
		{
			for (int x = 0; x < segments; x++, t += 6)
			{
				data.triangles[t + 0] = ((y + 0) * (segments + 1)) + x + 0 + offset;
				data.triangles[t + 1] = ((y + 1) * (segments + 1)) + x + 0 + offset;
				data.triangles[t + 2] = ((y + 1) * (segments + 1)) + x + 1 + offset;

				data.triangles[t + 3] = ((y + 0) * (segments + 1)) + x + 1 + offset;
				data.triangles[t + 4] = ((y + 0) * (segments + 1)) + x + 0 + offset;
				data.triangles[t + 5] = ((y + 1) * (segments + 1)) + x + 1 + offset;
			}
		}

		return data;
	}

	void GenerateVerticalSegments(
		ref CapsuleData data,							// CapsuleData reference
		ref int ind,									// Index counter
		float height, float radius, float yOff,			// LineSegement dimensions
		TesslInfo tesslInfo,							// tessellation info
		int points,										// the number of points in the circle
		Quaternion rot, Vector3 position,				// global position modifiers
		float[] pX, float[] pZ, float pY, float pR		// pre-generated circle points
		)
    {
		float cyllinderHeight = height - (radius * 2);
		float halfHeight = height * 0.5f; 
		for (int y = 0; y < tesslInfo.count; y++)
		{
			for (int x = 0; x < points; x++)
			{
				float xx, yy, zz;

				xx = pX[x] * pR * radius;
				xx += tesslInfo.segmentPositions[y].x;

				zz = pZ[x] * pR * radius;
				zz += tesslInfo.segmentPositions[y].z;

				yy = halfHeight - (tesslInfo.segmentPositions[y].y * height);

				data.vertices[ind] = new Vector3(xx, yy, zz);
				data.vertices[ind] = rot * data.vertices[ind];
				data.vertices[ind] += position;

				ind++;
			}
		}
	}

	TesslInfo GenerateCapsuleTessellationPointCount(float height, float radius)
    {
		switch (GenerationManager.Instance.Params.jitterMode)
		{
			case TeselationMod.Jitter:
				return CalculateJitterPointCount(height, radius);

			case TeselationMod.Random_Offset:
				return CalculateRandomOffsetPointCount(height, radius);
		}

		return new TesslInfo();
	}

	TesslInfo CalculateJitterPointCount(float height, float radius)
    {
		return new TesslInfo();
    }

	TesslInfo CalculateRandomOffsetPointCount(float height, float radius)
    {
		TesslInfo info = new TesslInfo();
		
		info.count = (useManualSegmentCount)?
			verticalPerCapsule :
			Mathf.RoundToInt(height / GenerationManager.Instance.Params.jitterUnit);

		info.segmentPositions = new Vector3[info.count];

		int subSegmentCount		= info.count + 1;
		float subSegmentLength	= 1f / subSegmentCount;
		for (int i = 1; i <= info.count; i++)
        {
			//var offset = 

			info.segmentPositions[i-1] = new Vector3(0,subSegmentLength * i,0);
        }

		return info;
    }

	public void JitterCapsule(LineSegment lineSegment, ref CapsuleData meshData)
    {
		
    }

}
