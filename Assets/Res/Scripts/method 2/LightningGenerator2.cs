using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class LightningGenerator2 : MonoBehaviour
{
    [SerializeField] GridManager m_gridManager;
    public List<Node> m_lightningNodes;

    public int steps = 1;
    public int enviromentalIterations = 25;

    public float mu = 1;

    public Grid<Node> grid
    { get { return m_gridManager.grid; } }

    void Start()
    {
        Reset();
    }

    public void Generate()
    {
        for (int i = 0; i < steps; i++)
            Step();
    }

    public void Step()
    {
        for (int i = 0; i < enviromentalIterations; i++)
            SolvePotential();

        AddGrowthSite();
    }

    public void AddGrowthSite()
    {
        List<Node> potentialNodes = new List<Node>();

        float phiSum = 0;

        foreach(var node in m_lightningNodes)
        {
            AddPotentialNode(potentialNodes, node,  1, 0, 0, ref phiSum);
            AddPotentialNode(potentialNodes, node, -1, 0, 0, ref phiSum);
            AddPotentialNode(potentialNodes, node, 0,  1, 0, ref phiSum);
            AddPotentialNode(potentialNodes, node, 0, -1, 0, ref phiSum);
            AddPotentialNode(potentialNodes, node, 0, 0,  1, ref phiSum);
            AddPotentialNode(potentialNodes, node, 0, 0, -1, ref phiSum);
        }

        int n = potentialNodes.Count;

        float[] weights = new float[n];

        for (int i = 0; i < n; i++)
        {
            float phi = potentialNodes[i].phi;

            weights[i] = Mathf.Pow(phi, mu) / Mathf.Pow(phiSum, mu);
        }

        float totalWeight = weights.Sum();

        float rndNum = Random.Range(0, totalWeight);
        int selectedIndex = -1;

        int j = 0;
        foreach(var weight in weights)
        {
            if(rndNum <= weight)
            {
                selectedIndex = j;
                break;
            }

            rndNum = rndNum - weight;

            j++;
        }

        Node selectedNode = potentialNodes[selectedIndex];

        selectedNode.phi = 0;
        selectedNode.isLightning = true;

        m_lightningNodes.Add(selectedNode);
    }

    void AddPotentialNode(List<Node> potentialNodes, Node node, int x, int y, int z, ref float phiSum)
    {
        Vector3Int pos = node.Offset(x, y, z);

        if (grid.IsValidNode(pos) && !grid[pos].isLightning && !potentialNodes.Contains(grid[pos]))
        {
            potentialNodes.Add(grid[node.Offset(x, y, z)]);
            phiSum += grid[node.Offset(x, y, z)].phi;
        }
    }

    public void SolvePotential()
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

        for (int i = 0; i < grid.Width; i++)
        {
            for (int j = 0; j < grid.Height; j++)
            {
                for (int k = 0; k < grid.Depth; k++)
                {
                    grid[i, j, k] = new Node(new Vector3Int(i, j, k));
                }
            }
        }

        m_lightningNodes = new List<Node>();

        grid[m_gridManager.startPos] = new Node(0, m_gridManager.startPos);
        grid[m_gridManager.endPos] = new Node(1, m_gridManager.endPos);

        m_lightningNodes.Add(grid[m_gridManager.startPos]);

        grid[m_gridManager.startPos].isLightning = true;

        for (int i = 0; i < enviromentalIterations; i++)
            SolvePotential();
    }
}
