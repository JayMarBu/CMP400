using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CapsuleData
{
	public int[] triangles;
	public Vector3[] vertices;
	public Vector2[] uvs;
}

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class LightningMeshGenerator : MonoBehaviour
{
    public float radius = 0.5f;

    public int segments = 24;

    private void OnValidate()
    {
        //m_filter = GetComponent<MeshFilter>();
        //m_renderer = GetComponent<MeshRenderer>();
    }

    public void GenerateMesh(List<LineSegment> lineSegments)
    {
		List<Vector3> vertices = new List<Vector3>();
		List<Vector2> uvs = new List<Vector2>();
		List<int> triangles = new List<int>();


		foreach (var segment in lineSegments)
        {
			var data = GenerateCapsule(segment);

			vertices.AddRange(data.vertices);
			uvs.AddRange(data.uvs);
			triangles.AddRange(data.triangles);
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
		mesh.uv = uvs.ToArray();
		mesh.triangles = triangles.ToArray();

		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
		mesh.Optimize();
	}

	public CapsuleData GenerateCapsule(LineSegment lineSegment)
    {
		var data = new CapsuleData();

		// set mesh parameters from line segment
		float height = lineSegment.length;
		Quaternion rot = Quaternion.FromToRotation(Vector3.up, lineSegment.direction);
		Vector3 position = lineSegment.centre - transform.position ;

		// make segments an even number
		if (segments % 2 != 0)
			segments++;

		segments++;

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


		// calculate capsule position


		// - Vertices and uvs -

		data.vertices = new Vector3[points * (points + 1)];
		data.uvs = new Vector2[data.vertices.Length];
		int ind = 0;

		// Y-offset is half the height minus the diameter
		float yOff = (height - (radius * 2f)) * 0.5f;
		if (yOff < 0)
			yOff = 0;

		// uv calculations
		float stepX = 1f / ((float)(points - 1));
		float uvX, uvY;

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

				uvX = 1f - (stepX * (float)x);
				uvY = (data.vertices[ind].y + (height * 0.5f)) / height;
				data.uvs[ind] = new Vector2(uvX, uvY);

				ind++;
			}
		}

		// middle rings
		for (int x = 0; x < points; x++)
		{
			data.vertices[ind] = new Vector3(pX[x] * pR[top-1], pY[top-1], pZ[x] * pR[top-1]) * radius;
			data.vertices[ind].y = 0;
			data.vertices[ind] = rot * data.vertices[ind];
			data.vertices[ind] += position;

			uvX = 1f - (stepX * (float)x);
			uvY = (data.vertices[ind].y + (height * 0.5f)) / height;
			data.uvs[ind] = new Vector2(uvX, uvY);

			ind++;
		}



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

				uvX = 1f - (stepX * (float)x);
				uvY = (data.vertices[ind].y + (height * 0.5f)) / height;
				data.uvs[ind] = new Vector2(uvX, uvY);

				ind++;
			}
		}


		// - Triangles -

		data.triangles = new int[(segments * (segments + 1) * 2 * 3)];

		for (int y = 0, t = 0; y < segments + 1; y++)
		{
			for (int x = 0; x < segments; x++, t += 6)
			{
				data.triangles[t + 0] = ((y + 0) * (segments + 1)) + x + 0;
				data.triangles[t + 1] = ((y + 1) * (segments + 1)) + x + 0;
				data.triangles[t + 2] = ((y + 1) * (segments + 1)) + x + 1;

				data.triangles[t + 3] = ((y + 0) * (segments + 1)) + x + 1;
				data.triangles[t + 4] = ((y + 0) * (segments + 1)) + x + 0;
				data.triangles[t + 5] = ((y + 1) * (segments + 1)) + x + 1;
			}
		}

		return data;
	}
    
}
