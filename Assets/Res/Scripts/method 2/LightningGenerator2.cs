using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningGenerator2 : MonoBehaviour
{
    [SerializeField] GridManager m_gridManager;
    //[SerializeField] List<Node> m_nodes;

    public int steps = 1;

    public Grid<Node> grid
    { get { return m_gridManager.grid; } }

    void Start()
    {
        m_gridManager.ResetGrid();
        grid.Populate(() => new Node());
        grid[m_gridManager.startPos] = new Node(0, m_gridManager.startPos);
        grid[m_gridManager.endPos] = new Node(1, m_gridManager.endPos);

        //m_nodes.Add(grid[m_gridManager.startPos]);
        //m_nodes.Add(grid[m_gridManager.endPos]);
    }

    public void Generate()
    {
        for (int i = 0; i < steps; i++)
            Step();
    }

    public void Step()
    {
        float n = 1f / 4f;

        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                for (int z = 0; z < grid.Depth; z++)
                {
                    if (grid[x, y, z].phi >= 1 || grid[x, y, z].phi <= 0)
                        continue;

                    float edgeCount = 0;
                    float edgeSum = 0;

                    checkEdge(x + 1, y, z, ref edgeCount, ref edgeSum);
                    checkEdge(x - 1, y, z, ref edgeCount, ref edgeSum);
                    checkEdge(x, y + 1, z, ref edgeCount, ref edgeSum);
                    checkEdge(x, y - 1, z, ref edgeCount, ref edgeSum);
                    checkEdge(x, y, z + 1, ref edgeCount, ref edgeSum);
                    checkEdge(x, y, z - 1, ref edgeCount, ref edgeSum);

                    grid[x, y, z].phi = (1f / edgeCount) * edgeSum;

                        /*grid[x, y, z].phi = 
                        n * 
                        (
                            ((grid[x + 1, y, z] == null) ? 0f : grid[x + 1, y, z].phi) +
                            ((grid[x - 1, y, z] == null) ? 0f : grid[x - 1, y, z].phi) +
                            ((grid[x, y + 1, z] == null) ? 0f : grid[x, y + 1, z].phi) +
                            ((grid[x, y - 1, z] == null) ? 0f : grid[x, y - 1, z].phi) //+
                            //((grid[x, y, z + 1] == null) ? 0.1f : grid[x, y, z + 1].phi) +
                            //((grid[x, y, z - 1] == null) ? 0.1f : grid[x, y, z - 1].phi)
                        );*/
                }
            }
        }
    }

    void checkEdge(int x, int y, int z, ref float count, ref float sum)
    {
        if (grid[x, y, z] != null)
        {
            count += 1f;
            sum += grid[x, y, z].phi;
        }
    }

    public void Reset()
    {
        m_gridManager.ResetGrid();
    }
}
