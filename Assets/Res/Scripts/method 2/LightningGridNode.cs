using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Node
{
    [SerializeField] public float phi = 0.15f;
    public Vector3Int position;

    public Node()
    {
        phi = 0.15f;
    }

    public Node(Vector3Int p)
    {
        phi = 0.15f;
        position = p;
    }

    public Node (float f, Vector3Int p)
    {
        phi = f;
        position = p;
    }
}
