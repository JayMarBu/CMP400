using System;
using UnityEngine;

// GENERIC GRID DATA CLASS **********************************************************************************************************************
[System.Serializable]
public class Grid<T> where T : class, new()
{
    // Grid Settings
    [SerializeField, Min(1)] private int width = 2;
    [SerializeField, Min(1)] private int height = 2;
    [SerializeField, Min(1)] private int depth = 2;

    [SerializeField, Min(0)] private float cellSize = 1;
    [SerializeField] private Vector3 originPosition = Vector3.zero;

    // Public Getters and Setters
    public int Width { get { return width; } }
    public int Height { get { return height; } }
    public int Depth { get { return depth; } }

    public Vector3Int Bounds { get { return new Vector3Int(width, height, depth); } }

    public float CellSize { get { return cellSize; } }
    public Vector3 OriginPosition { get { return originPosition; } set { originPosition = value; } }

    // Grid Array
    [SerializeField] private T[,,] grid;

    // CONSTRUCTORS ***************************************************************************
    public Grid(int in_width, int in_height, int in_depth, float in_cellSize, Vector3 in_originPosition)
    {
        width = in_width;
        height = in_height;
        depth = in_depth;
        cellSize = in_cellSize;
        originPosition = in_originPosition;
    }

    public Grid()
    {
        width = 2;
        height = 2;
        depth = 2;
        cellSize = 1f;
        originPosition = Vector3.zero;
    }

    public Grid(Grid<T> other)
    {
        this.width = other.width;
        this.height = other.height;
        this.depth = other.depth;
        this.cellSize = other.cellSize;
        this.originPosition = other.originPosition;
    }

    // OPERATOR OVERLOADS *********************************************************************
    public T this[int x, int y, int z]
    {
        get { return GetNode(x, y, z); }
        set { grid[x, y, z] = value; }
    }

    public T this[Vector3Int pos]
    {
        get { return GetNode(pos); }
        set { grid[pos.x, pos.y, pos.z] = value; }
    }

    public T this[GridNodePosition pos]
    {
        get { return GetNode(pos.grid); }
        set { grid[pos.grid.x, pos.grid.y, pos.grid.z] = value; }
    }

    // INITIALISATION METHODS *****************************************************************
    virtual public void Init()
    {
        grid = new T[width, height, depth];//.Populate(()=> new T());
    }

    // NODE INFORMATION METHODS ***************************************************************
    public bool NodeExists(Vector3Int pos) => NodeExists(pos.x, pos.y, pos.z);

    public bool NodeExists(int x, int y, int z)
    {
        if (grid == null)
            return false;

        // the generic equivalent to a null check.
        // god damn almost always reference types >:[
        //if (EqualityComparer<T>.Default.Equals(grid[x, y, z], default(T)))
        //    return false;

        if (grid[x, y, z] == null)
            return false;

        return true;
    }

    virtual public bool NodeExistsAt(Vector3 pos)
    {
        if (
            pos.x < originPosition.x || pos.x >= width + originPosition.x ||    // x alignment
            pos.y < originPosition.y || pos.y >= height + originPosition.y ||   // y alignment
            pos.z < originPosition.z || pos.z >= depth + originPosition.z       // z alignment
            )
            return false;
        return true;
    }

    protected T GetNode(Vector3Int pos) => GetNode(pos.x, pos.y, pos.z);

    virtual protected T GetNode(int x, int y, int z)
    {
        if (IsValidNode(x,y,z))
        {
            if (NodeExists(x, y, z))
                return grid[x, y, z];
            return null;
        }

        return null;
    }

    public bool IsValidNode(Vector3Int pos) => IsValidNode(pos.x, pos.y, pos.z);
    public bool IsValidNode(int x, int y, int z)
    {
        return (x >= 0 && y >= 0 && z >= 0 && x < width && y < height && z < depth);
    }


    // COORDINATE SPACE CONVERSION METHODS ****************************************************
    public Vector3Int WorldToGrid(Vector3 pos) => WorldToGrid(pos.x, pos.y, pos.z);

    virtual public Vector3Int WorldToGrid(float x, float y, float z)
    {
        if (x < originPosition.x || x >= width + originPosition.x || y < originPosition.y || y >= height + originPosition.y || z < originPosition.z || z >= depth + originPosition.z)
            return Vector3Int.one * -1;

        x -= originPosition.x;
        y -= originPosition.y;
        z -= originPosition.z;

        int g_x = Mathf.FloorToInt(x / cellSize);
        int g_y = Mathf.FloorToInt(y / cellSize);
        int g_z = Mathf.FloorToInt(z / cellSize);

        return new Vector3Int(g_x, g_y, g_z);
    }

    public Vector3 ToWorld(Vector3Int pos) => ToWorld(pos.x, pos.y, pos.z);

    virtual public Vector3 ToWorld(int x, int y, int z)
    {
        Vector3 pos = ToCell(x, y, z);
        return new Vector3(pos.x + (cellSize / 2), pos.y + (cellSize / 2), pos.z + (cellSize / 2));
    }

    public static Vector3 GridToWorld(Vector3 originPos, int width, int height, int depth, float cellsize, Vector3Int pos) => GridToWorld(originPos, width, height, depth, cellsize, pos.x, pos.y, pos.z);

    public static Vector3 GridToWorld(Vector3 originPos, int width, int height, int depth, float cellSize, int x, int y, int z)
    {
        if (x >= 0 && y >= 0 && z >= 0 && x < width && y < height && z < depth)
            return new Vector3(originPos.x + x * cellSize, originPos.y + y * cellSize, originPos.z + z * cellSize);
        Debug.LogWarning("attempting to receive node outside of grid bounds");
        return Vector3.zero;
    }

    public Vector3 ToCell(Vector3Int pos) => ToCell(pos.x, pos.y, pos.z);

    virtual public Vector3 ToCell(int x, int y, int z)
    {
        if (IsValidNode(x,y,z))
            return new Vector3(originPosition.x + x * cellSize, originPosition.y + y * cellSize, originPosition.z + z * cellSize);
        Debug.LogWarning("attempting to receive node outside of grid bounds");
        return Vector3.zero;
    }

    // MISC HELPER METHODS ********************************************************************
    virtual public GridNodePosition GetNodePosition(int x, int y, int z)
    {
        GridNodePosition pos = new GridNodePosition();
        pos.grid = new Vector3Int(x, y, z);
        pos.world = ToWorld(x, y, z);
        return pos;
    }

    public T GetNodeRelative(GridNodePosition pos, int x_offset, int y_offest, int z_offset) => GetNodeRelative(pos.grid.x, pos.grid.y, pos.grid.z, x_offset, y_offest, z_offset);

    public T GetNodeRelative(GridNodePosition pos, Vector3Int offset) => GetNodeRelative(pos.grid.x, pos.grid.y, pos.grid.z, offset.x, offset.y, offset.z);

    public T GetNodeRelative(Vector3Int initPos, int x_offset, int y_offest, int z_offset) => GetNodeRelative(initPos.x, initPos.y, initPos.z, x_offset, y_offest, z_offset);

    public T GetNodeRelative(int x, int y, int z, Vector3Int offset) => GetNodeRelative(x, y, z, offset.x, offset.y, offset.z);

    public T GetNodeRelative(Vector3Int initPos, Vector3Int offset) => GetNodeRelative(initPos.x, initPos.y, initPos.z, offset.x, offset.y, offset.z);

    virtual public T GetNodeRelative(int x, int y, int z, int x_offset, int y_offest, int z_offset)
    {
        if (x + x_offset >= 0 && y + y_offest >= 0 && x + x_offset < width && y + y_offest < height)
            return grid[x + x_offset, y + y_offest, z + z_offset];
        return null;
    }

    public T WorldToNode(float x, float y, float z) => WorldToNode(new Vector3(x, y, z));

    virtual public T WorldToNode(Vector3 pos)
    {
        return this[WorldToGrid(pos)];
    }

    virtual public int Area { get { return width * height * depth; } }

    virtual public void DrawGizmos(Color drawGridColour, Color drawGridOutlineColour)
    {
        Gizmos.color = drawGridOutlineColour;

        Vector3 o = originPosition;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    Gizmos.DrawWireCube(new Vector3(o.x + x * cellSize, o.y + y*cellSize, o.z + z*cellSize), new Vector3(cellSize, cellSize, cellSize));
                }
            }
        }
    }
}

// GRID NODE POSITION DATA CLASS ****************************************************************************************************************
[System.Serializable]
public class GridNodePosition
{
    public Vector3 world;
    public Vector3Int grid;

    public GridNodePosition()
    {
        world = Vector3.zero;
        grid = Vector3Int.zero;
    }

    public GridNodePosition(GridNodePosition copy)
    {
        world = copy.world;
        grid = copy.grid;
    }
}
