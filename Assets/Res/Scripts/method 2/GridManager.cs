using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
class Node
{
    [SerializeField] public int val = 0;
}

public class GridManager : MonoBehaviour
{
    [SerializeField] Grid<Node> m_grid;
    [SerializeField] Color m_gridColour;

    [SerializeField, HideInInspector] Transform m_startPoint;
    [SerializeField, HideInInspector] Transform m_endPoint;

    private void OnValidate()
    {
        m_grid.OriginPosition = transform.position;
    }

    private void OnDrawGizmos()
    {
        Vector3 o = m_grid.OriginPosition;

        for (int x = 0; x < m_grid.Width; x++)
        {
            for (int y = 0; y < m_grid.Height; y++)
            {
                for (int z = 0; z < m_grid.Depth; z++)
                {
                    Gizmos.color = m_gridColour;

                    if(m_grid.WorldToGrid(m_startPoint.position) == new Vector3Int(x,y,z))
                        Gizmos.color = Color.blue;

                    if (m_grid.WorldToGrid(m_endPoint.position) == new Vector3Int(x, y, z))
                        Gizmos.color = Color.red;

                    Gizmos.DrawWireCube(new Vector3(o.x + x * m_grid.CellSize, o.y + y * m_grid.CellSize, o.z + z * m_grid.CellSize), new Vector3(m_grid.CellSize, m_grid.CellSize, m_grid.CellSize));
                }
            }
        }
    }
}
