using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

[Serializable]
public class PathfindingGrid
{
    [SerializeField] private uint size = 50;
    [SerializeField] private float tileSize = 48;
    public Tile[,] tiles = new Tile[50, 50];
    public Vector3 origin = Vector3.zero;
    [SerializeField] private float angle = 0;

    public void DebugTiles(Vector3 unitposition, Vector3 targetposition)
    {
        Vector2Int unitIndex = GetIndexFromGridPosition(unitposition);
        Vector3 unitPosition = GetGridPositionFromIndex(unitIndex);

        Vector2Int targetindex = GetIndexFromGridPosition(targetposition);
        Vector3 targetunitPosition = GetGridPositionFromIndex(targetindex);

        // Maybe wasn't a very good idea to make tile debugger like this.

        if (tiles.Length > 0)
        {
            // This gives our unit's position on grid. Just a check for grid index.
            Debug.DrawRay(unitPosition + Vector3.right * (tileSize / 7), Vector3.up, Color.cyan);
            Debug.DrawRay(unitPosition + Vector3.left * (tileSize / 7), Vector3.up, Color.cyan);
            Debug.DrawRay(unitPosition + Vector3.forward * (tileSize / 7), Vector3.up, Color.cyan);
            Debug.DrawRay(unitPosition + Vector3.back * (tileSize / 7), Vector3.up, Color.cyan);

            // This gives our target's position on grid. Just a check for grid index.
            Debug.DrawRay(targetunitPosition + Vector3.right * (tileSize / 7), Vector3.up, Color.green);
            Debug.DrawRay(targetunitPosition + Vector3.left * (tileSize / 7), Vector3.up, Color.green);
            Debug.DrawRay(targetunitPosition + Vector3.forward * (tileSize / 7), Vector3.up, Color.green);
            Debug.DrawRay(targetunitPosition + Vector3.back * (tileSize / 7), Vector3.up, Color.green);

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    // Draws tile for our grid
                    Vector3 pos = GetGridPositionFromIndex(new Vector2Int(i, j));
                    Vector3 dir = new Vector3(tiles[i, j].direction.x, 0, tiles[i, j].direction.y);

                    Debug.DrawLine(pos + Vector3.right * (tileSize / 3) + Vector3.forward * (tileSize / 3), pos + Vector3.right * (tileSize / 3) - Vector3.forward * (tileSize / 3), Color.black);
                    Debug.DrawLine(pos - Vector3.right * (tileSize / 3) + Vector3.forward * (tileSize / 3), pos - Vector3.right * (tileSize / 3) - Vector3.forward * (tileSize / 3), Color.black);
                    Debug.DrawLine(pos + Vector3.forward * (tileSize / 3) + Vector3.right * (tileSize / 3), pos + Vector3.forward * (tileSize / 3) - Vector3.right * (tileSize / 3), Color.black);
                    Debug.DrawLine(pos - Vector3.forward * (tileSize / 3) + Vector3.right * (tileSize / 3), pos - Vector3.forward * (tileSize / 3) - Vector3.right * (tileSize / 3), Color.black);

                    // Trying to drawout a heatmap generation for flowfield pathfinding.
                    if (tiles[i, j].distance != -2)
                        Handles.Label(pos, tiles[i, j].distance.ToString());

                    // Making more easier to point out our unit and target again.
                    Color color = Color.red;
                    if (i == unitIndex.x && j == unitIndex.y)
                        color = Color.blue;
                    else if (i == targetindex.x && j == targetindex.y)
                        color = Color.green;

                    Debug.DrawLine(pos, pos + dir * .4f, color);
                }

            }
        }

        // Drawing out our neighbor tiles for our unit as X mark
        List<Tile> neightborTiles = GetNeightborAStar(unitIndex);
        foreach (var neightborTile in neightborTiles)
        {
            var pos = GetGridPositionFromTile(neightborTile);
            Debug.DrawLine(pos, pos + (Vector3.forward + Vector3.right) * 0.2f, Color.yellow);
            Debug.DrawLine(pos, pos + (Vector3.forward + Vector3.left) * 0.2f, Color.yellow);
            Debug.DrawLine(pos, pos + (Vector3.back + Vector3.right) * 0.2f, Color.yellow);
            Debug.DrawLine(pos, pos + (Vector3.back + Vector3.left) * 0.2f, Color.yellow);
        }

        // Drawing out our neightbor tiles for our target as X mark.
        neightborTiles = GetNeightborAStar(targetindex);
        foreach (var neightborTile in neightborTiles)
        {
            var pos = GetGridPositionFromTile(neightborTile);
            Debug.DrawLine(pos, pos + (Vector3.forward + Vector3.right) * 0.2f, Color.yellow);
            Debug.DrawLine(pos, pos + (Vector3.forward + Vector3.left) * 0.2f, Color.yellow);
            Debug.DrawLine(pos, pos + (Vector3.back + Vector3.right) * 0.2f, Color.yellow);
            Debug.DrawLine(pos, pos + (Vector3.back + Vector3.left) * 0.2f, Color.yellow);
        }
    }

    public void UpdateFlowField(Vector3 position)
    {
        // Reset nodes
        foreach (Tile t in tiles)
        {
            if (t.distance != -2)
                t.distance = -1;
        }

        // We need a queue that'll track of tiles.
        Queue<Tile> queue = new Queue<Tile>();

        // Initialization...
        Tile targetTile = GetTileFromGridPosition(position);
        //queue.Enqueue(targetTile);    // This process is done in LocalOptimaFix. So no point adding here.

        LocalOptimaFix(targetTile, queue);
        CreateHeatmap(targetTile, queue);
        CreateVectorField();
    }

    private void LocalOptimaFix(Tile targetTile, Queue<Tile> queue)
    {
        Vector2Int index = GetIndexFromTile(targetTile);

        if (index.x + 1 > size - 1)
        {
            index.x = index.x - 1;
        }
        
        if (index.y + 1 > size - 1)
        {
            index.y = index.y - 1;
        }

        for (int i = 0; i <= 1; i++)
        {
            for (int j = 0; j <= 1; j++)
            {

                if (tiles[index.x + i, index.y + j].distance != -2)
                {
                    tiles[index.x + i, index.y + j].distance = 0;
                    queue.Enqueue(tiles[index.x + i, index.y + j]);
                }
            }
        }
    }

    private void CreateVectorField()
    {
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                Vector2 direction;

                int left, right, up, down;
                left = right = up = down = tiles[i, j].distance;

                if (i + 1 < size && tiles[i + 1, j].distance != -2)
                {
                    right = tiles[i + 1, j].distance;
                }

                if (i - 1 >= 0 && tiles[i - 1, j].distance != -2)
                {
                    left = tiles[i - 1, j].distance;
                }

                if (j + 1 < size && tiles[i, j + 1].distance != -2)
                {
                    up = tiles[i, j + 1].distance;
                }

                if (j - 1 >= 0 && tiles[i, j - 1].distance != -2)
                {
                    down = tiles[i, j - 1].distance;
                }

                direction.x = (float)left - right;
                direction.y = (float)down - up;

                if (direction.x < 0 && left == tiles[i, j].distance)
                    direction.x = 0.0f;
                else if (direction.x > 0 && right == tiles[i, j].distance)
                    direction.x = 0.0f;
                if (direction.y < 0 && down == tiles[i, j].distance)
                    direction.y = 0.0f;
                else if (direction.y > 0 && up == tiles[i, j].distance)
                    direction.y = 0.0f;

                tiles[i, j].direction = direction.normalized;
            }
        }
    }

    private void CreateHeatmap(Tile targetTile, Queue<Tile> queue)
    {
        // Getting neighbours and stuffs...
        List<Tile> neighbours = GetNeightborFlowFieldFromTile(targetTile);

        // Looping over neighbours...
        foreach(Tile t in neighbours)
        {
            t.distance = targetTile.distance + 1;
            queue.Enqueue(t);
        }

        // We have to continue there is no value on the queue
        if (queue.Count == 0)
            return;

        // We remove a value from the queue and add next value into it. It should continue until none of the values left.
        Tile next = queue.Dequeue();
        CreateHeatmap(next, queue);
    }

    public void GenerateTiles()
    {
        // Basic tile generation.
        tiles = new Tile[size, size]; // Going with square size for now.
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                tiles[i, j] = new Tile(new Vector2(i, j));
            }
        }
    }

    public List<Tile> GetNeightborAStar(Vector2Int tilePoint)
    {
        List<Tile> neightbortiles = new List<Tile>();

        // This'll help us to get the neightbor tiles. It'll be in 3x3 tile.
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                // i and j equals to 0 means it's our center tile. We ignore that.
                if (i == 0 && j == 0)
                    continue;

                // Getting the value for tile check.
                int checkX = tilePoint.x + i, checkY = tilePoint.y + j;

                if (checkX >= 0 && checkX < size && checkY >= 0 && checkY < size) // To check if the value doesn't go out of index.
                {
                    Tile neighborTile = tiles[checkX, checkY];

                    neightbortiles.Add(neighborTile);
                }
            }
        }

        return neightbortiles;
    }

    // Keeping this for now. Probably remove later.
    //private List<Tile> GetNeightborFlowField(Vector2Int tilePoint)
    //{
    //    List<Tile> neightbortiles = new List<Tile>();

    //    Tile neighborTile;
    //    if (tilePoint.x + 1 >= 0 && tilePoint.x + 1 < size && tilePoint.y >= 0 && tilePoint.y < size)
    //    {
    //        neighborTile = tiles[tilePoint.x + 1, tilePoint.y];
    //        neightbortiles.Add(neighborTile);
    //    }

    //    if (tilePoint.x - 1 >= 0 && tilePoint.x - 1 < size && tilePoint.y >= 0 && tilePoint.y < size)
    //    {
    //        neighborTile = tiles[tilePoint.x - 1, tilePoint.y];
    //        neightbortiles.Add(neighborTile);
    //    }

    //    if (tilePoint.x >= 0 && tilePoint.x < size && tilePoint.y + 1 >= 0 && tilePoint.y + 1 < size)
    //    {
    //        neighborTile = tiles[tilePoint.x, tilePoint.y + 1];
    //        neightbortiles.Add(neighborTile);
    //    }

    //    if (tilePoint.x >= 0 && tilePoint.x < size && tilePoint.y - 1 >= 0 && tilePoint.y - 1 < size)
    //    {
    //        neighborTile = tiles[tilePoint.x, tilePoint.y - 1];
    //        neightbortiles.Add(neighborTile);
    //    }

    //    return neightbortiles;
    //}

    private List<Tile> GetNeightborFlowFieldFromTile(Tile tile)
    {
        List<Tile> neighbours = new List<Tile>();
        Vector2Int tilePoint = GetIndexFromTile(tile); //new Vector2Int((int)tile.GetLocalPosition(angle).x, (int)tile.GetLocalPosition(angle).z);

        // Right
        if (tilePoint.x + 1 < size && tiles[tilePoint.x + 1, tilePoint.y].distance == -1)
        {
            neighbours.Add(tiles[tilePoint.x + 1, tilePoint.y]);
        }

        // Left
        if (tilePoint.x - 1 >= 0 && tiles[tilePoint.x - 1, tilePoint.y].distance == -1)
        {
            neighbours.Add(tiles[tilePoint.x - 1, tilePoint.y]);
        }

        // Up
        if (tilePoint.y + 1 < size && tiles[tilePoint.x, tilePoint.y + 1].distance == -1)
        {
            neighbours.Add(tiles[tilePoint.x, tilePoint.y + 1]);
        }

        // Down
        if (tilePoint.y - 1 >= 0 && tiles[tilePoint.x, tilePoint.y - 1].distance == -1)
        {
            neighbours.Add(tiles[tilePoint.x, tilePoint.y - 1]);
        }

        return neighbours;
    }

    public Tile GetTileFromGridPosition(Vector3 position)
    {
        Vector2Int index = GetIndexFromGridPosition(position);
        return tiles[index.x, index.y];
    }

    public Vector2Int GetIndexFromTile(Tile tile)
    {
        return GetIndexFromGridPosition(tile.GetLocalPosition());
    }

    public Vector3 GetGridPositionFromIndex(Vector2Int index)
    {
        Vector3 position;
        Vector3 localPosition = tiles[index.x, index.y].GetLocalPosition();
        position.x = origin.x + (tileSize * ((localPosition.x * Mathf.Cos(angle * Mathf.Deg2Rad)) - (localPosition.z * Mathf.Sin(angle * Mathf.Deg2Rad))));
        position.y = origin.y;
        position.z = origin.z + (tileSize * ((localPosition.x * Mathf.Sin(angle * Mathf.Deg2Rad)) + (localPosition.z * Mathf.Cos(angle * Mathf.Deg2Rad))));
        return position;
    }

    public Vector3 GetGridPositionFromTile(Tile tile)
    {
        Vector3 position;
        Vector3 localPosition = tile.GetLocalPosition();
        position.x = origin.x + (tileSize * ((localPosition.x * Mathf.Cos(angle * Mathf.Deg2Rad)) - (localPosition.z * Mathf.Sin(angle * Mathf.Deg2Rad))));
        position.y = origin.y;
        position.z = origin.z + (tileSize * ((localPosition.x * Mathf.Sin(angle * Mathf.Deg2Rad)) + (localPosition.z * Mathf.Cos(angle * Mathf.Deg2Rad))));
        return position;
    }

    public Vector2Int GetIndexFromGridPosition(Vector3 position)
    {
        //int i = Mathf.RoundToInt(Mathf.Cos(-angle * Mathf.Deg2Rad) * ((position.x - origin.x) / tileSize)); //Mathf.RoundToInt((2 * (position.x -  origin.x) - tileSize) / (tileSize * 2));
        position.x -= origin.x;
        position.z -= origin.z;
        float i = (position.x * Mathf.Cos(-angle * Mathf.Deg2Rad)) - (position.z * Mathf.Sin(-angle * Mathf.Deg2Rad));
        
        if (i > size - 1)
            i = (int)size - 1;
        else if (i < 0)
            i = 0;

        float j = (position.x * Mathf.Sin(-angle * Mathf.Deg2Rad)) + (position.z * Mathf.Cos(-angle * Mathf.Deg2Rad)); //= Mathf.RoundToInt((2 * (position.z - origin.z) - tileSize) / (tileSize * 2));
        
        if (j > size - 1)
            j = (int)size - 1;
        else if (j < 0)
            j = 0;

        return new Vector2Int(Mathf.RoundToInt(i), Mathf.RoundToInt(j));
    }
}

public class Tile
{
    private Vector2 localPosition = Vector2.zero; // Sanity check.
    public int distance = 0; // It is sometimes called wavePoint, heatcost but essentially the same thing.
    public Vector2 direction;

    public Tile()
    {
        localPosition = Vector2.zero;
    }

    public Tile(Vector2 _position)
    {
        localPosition = _position;
    }

    public Vector3 GetLocalPosition()
    {
        return new Vector3(localPosition.x, 0, localPosition.y);
    }
}