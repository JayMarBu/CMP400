using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public struct DebugColourModifiers
{
    [SerializeField] public bool drawGridInfoAsColour;
    [SerializeField, Min(1)] public float colourFadeModifier;
    [SerializeField] public bool useAlphaModifier;

    public DebugColourModifiers(float fade)
    {
        drawGridInfoAsColour = false;
        colourFadeModifier = fade;
        useAlphaModifier = true;
    }
}

public class GridManager : MonoBehaviour
{
    [SerializeField] private Grid<Node> m_grid;
    [SerializeField] Color m_gridColour;
    [SerializeField] Color m_gridTextColour;

    [SerializeField, HideInInspector] Transform m_startPoint;
    [SerializeField, HideInInspector] Transform m_endPoint;

    [SerializeField] public bool m_drawGrid = true;
    [SerializeField] public bool m_drawGridInfo = true;
    [SerializeField] public DebugColourModifiers m_drawGridInfoAsColour;

    public Grid<Node> grid
    { get { return m_grid; } }

    public Vector3Int startPos
    { get { return m_grid.WorldToGrid(m_startPoint.position); } }

    public Vector3Int endPos
    { get { return m_grid.WorldToGrid(m_endPoint.position); } }

    private void OnValidate()
    {
        m_grid.OriginPosition = transform.position;
    }

    public void InitialiseGrid()
    {
        m_grid.Init();
        //m_grid[m_grid.WorldToGrid(m_startPoint.position)] = new Node(0);
        //m_grid[m_grid.WorldToGrid(m_endPoint.position)] = new Node(1);
    }

    public void ResetGrid()
    {
        m_grid.Init();
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
                    Vector3 pos = new Vector3(o.x + x * m_grid.CellSize, o.y + y * m_grid.CellSize, o.z + z * m_grid.CellSize);

                    //Handles.Label(pos, "0");

                    bool drawSquare = m_drawGrid;
                    
                    Gizmos.color = m_gridColour;

                    if (m_grid.WorldToGrid(m_startPoint.position) == new Vector3Int(x, y, z))
                    {
                        Gizmos.color = Color.blue;
                        drawSquare = true;
                    }

                    if (m_grid.WorldToGrid(m_endPoint.position) == new Vector3Int(x, y, z))
                    {
                        Gizmos.color = Color.red;
                        drawSquare = true;
                    }

                    if(drawSquare)
                        Gizmos.DrawWireCube(pos, new Vector3(m_grid.CellSize, m_grid.CellSize, m_grid.CellSize));
                    

                    if (m_drawGridInfo)
                    {
                        GUI.color = m_gridTextColour;

                        if (m_grid != null && m_grid[x, y, z] != null)
                            Handles.Label(pos, m_grid[x, y, z].phi.ToString());
                    }

                    if (m_drawGridInfoAsColour.drawGridInfoAsColour && m_grid != null && m_grid[x, y, z] != null)
                    {
                        float phi = 1 - m_grid[x, y, z].phi;

                        float a = Mathf.Exp((phi * m_drawGridInfoAsColour.colourFadeModifier) - m_drawGridInfoAsColour.colourFadeModifier);

                        Gizmos.color = new Color( phi, phi, phi, (m_drawGridInfoAsColour.useAlphaModifier)?a:1);

                        Gizmos.DrawCube(pos, new Vector3(m_grid.CellSize / 3, m_grid.CellSize / 3, m_grid.CellSize / 3));
                    }
                }
            }
        }
    }
}
