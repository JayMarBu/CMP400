using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[ExecuteInEditMode]
public class MullerTestSpawner : MonoBehaviour
{
    private List<GameObject> m_testPoints = new List<GameObject>();

    public GameObject m_pointPrefab;

    public int spawnCount = 50;
    public float mean = 1;
    public float standardDeviation = 0.5f;
    public Vector2 calculatedMean = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Spawn()
    {
        for(int i = 0; i < spawnCount; i++)
        {
            Tuple<float, float> result = BoxMuller.Generate2D(mean, standardDeviation);

            var obj = Instantiate(m_pointPrefab, new Vector3((float)result.Item1, 0, (float)result.Item2), Quaternion.identity);
            m_testPoints.Add(obj);
        }

        float xSum = 0;
        float ySum = 0;

        for (int i = 0; i < m_testPoints.Count; i++)
        {
            xSum += m_testPoints[i].transform.position.x;
            ySum += m_testPoints[i].transform.position.z;
        }

        calculatedMean.x = xSum / m_testPoints.Count;
        calculatedMean.y = ySum / m_testPoints.Count;
    }

    public void Clear()
    {
        foreach(var obj in m_testPoints)
        {
            DestroyImmediate(obj);
        }

        m_testPoints.Clear();
        calculatedMean = Vector2.zero;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawLine(Vector3.zero, new Vector3(100, 0, 0));
        Gizmos.DrawLine(Vector3.zero, new Vector3(0, 0, 100));
    }
}
