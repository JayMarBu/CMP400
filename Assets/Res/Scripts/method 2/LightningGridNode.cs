using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Node
{
    [SerializeField] public float phi = 0.15f;
    public Vector3Int position;

    public bool isLightning = false;

    public Node parent = null;
    public List<Node> children;

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

    public void AddChild(Node child)
    {
        if (children == null)
            children = new List<Node>();

        children.Add(child);

        child.parent = this;
    }

    public Vector3Int Offset(int x, int y, int z) => Offset(new Vector3Int(x, y, z));
    public Vector3Int Offset(Vector3Int offset)
    {
        return position + offset;
    }
}
