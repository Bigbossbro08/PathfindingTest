using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[Serializable]
public class PathfindingGrid
{
    [SerializeField] private uint size = 50;
    [SerializeField] private float tileSize = 48;
    public Tile[,] tiles = new Tile[50, 50];
    [SerializeField] private Vector3 origin = Vector3.zero;

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
                    Debug.DrawLine(tiles[i, j].GetPositionIn3Dspace() + Vector3.right * (tileSize / 3) + Vector3.forward * (tileSize / 3), tiles[i, j].GetPositionIn3Dspace() + Vector3.right * (tileSize / 3) - Vector3.forward * (tileSize / 3), Color.black);
                    Debug.DrawLine(tiles[i, j].GetPositionIn3Dspace() - Vector3.right * (tileSize / 3) + Vector3.forward * (tileSize / 3), tiles[i, j].GetPositionIn3Dspace() - Vector3.right * (tileSize / 3) - Vector3.forward * (tileSize / 3), Color.black);
                    Debug.DrawLine(tiles[i, j].GetPositionIn3Dspace() + Vector3.forward * (tileSize / 3) + Vector3.right * (tileSize / 3), tiles[i, j].GetPositionIn3Dspace() + Vector3.forward * (tileSize / 3) - Vector3.right * (tileSize / 3), Color.black);
                    Debug.DrawLine(tiles[i, j].GetPositionIn3Dspace() - Vector3.forward * (tileSize / 3) + Vector3.right * (tileSize / 3), tiles[i, j].GetPositionIn3Dspace() - Vector3.forward * (tileSize / 3) - Vector3.right * (tileSize / 3), Color.black);

                    // Trying to drawout a heatmap generation for flowfield pathfinding.
                    Handles.Label(tiles[i, j].GetPositionIn3Dspace(), tiles[i, j].heatCost.ToString());

                    // Making more easier to point out our unit and target again.
                    Color color = Color.red;
                    if (i == unitIndex.x && j == unitIndex.y)
                        color = Color.blue;
                    else if (i == targetindex.x && j == targetindex.y)
                        color = Color.green;

                    Debug.DrawRay(tiles[i, j].GetPositionIn3Dspace(), new Vector3(tiles[i, j].direction.x, 0, tiles[i, j].direction.y), color);
                }

            }
        }

        // Drawing out our neighbor tiles for our unit as X mark
        Dictionary<Tile, Vector2Int> neightborTiles = GetNeightbor(unitIndex);
        foreach (KeyValuePair<Tile, Vector2Int> neightborTile in neightborTiles)
        {
            Debug.DrawLine(neightborTile.Key.GetPositionIn3Dspace(), neightborTile.Key.GetPositionIn3Dspace() + (Vector3.forward + Vector3.right) * 0.2f, Color.yellow);
            Debug.DrawLine(neightborTile.Key.GetPositionIn3Dspace(), neightborTile.Key.GetPositionIn3Dspace() + (Vector3.forward + Vector3.left) * 0.2f, Color.yellow);
            Debug.DrawLine(neightborTile.Key.GetPositionIn3Dspace(), neightborTile.Key.GetPositionIn3Dspace() + (Vector3.back + Vector3.right) * 0.2f, Color.yellow);
            Debug.DrawLine(neightborTile.Key.GetPositionIn3Dspace(), neightborTile.Key.GetPositionIn3Dspace() + (Vector3.back + Vector3.left) * 0.2f, Color.yellow);
        }

        // Drawing out our neightbor tiles for our target as X mark.
        neightborTiles = GetNeightbor(targetindex);
        foreach (KeyValuePair<Tile, Vector2Int> neightborTile in neightborTiles)
        {
            Debug.DrawLine(neightborTile.Key.GetPositionIn3Dspace(), neightborTile.Key.GetPositionIn3Dspace() + (Vector3.forward + Vector3.right) * 0.2f, Color.yellow);
            Debug.DrawLine(neightborTile.Key.GetPositionIn3Dspace(), neightborTile.Key.GetPositionIn3Dspace() + (Vector3.forward + Vector3.left) * 0.2f, Color.yellow);
            Debug.DrawLine(neightborTile.Key.GetPositionIn3Dspace(), neightborTile.Key.GetPositionIn3Dspace() + (Vector3.back + Vector3.right) * 0.2f, Color.yellow);
            Debug.DrawLine(neightborTile.Key.GetPositionIn3Dspace(), neightborTile.Key.GetPositionIn3Dspace() + (Vector3.back + Vector3.left) * 0.2f, Color.yellow);
        }
    }

    public void GenerateTiles(Vector3 _origin)
    {
        // Basic tile generation.
        origin = _origin; // origin position for the tile generation.
        tiles = new Tile[size, size]; // Going with square size for now.
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                float x = origin.x + ((i * tileSize) + (tileSize / 2));
                float y = origin.z + ((j * tileSize) + (tileSize / 2));
                tiles[i, j] = new Tile(new Vector2(x, y));
            }
        }
    }

    // Not sure if using Dictionary is a good idea but it helps to keep the track of neightbor tiles atleast.
    private Dictionary<Tile, Vector2Int> GetNeightbor(Vector2Int tilePoint)
    {
        Dictionary<Tile, Vector2Int> neightbortiles = new Dictionary<Tile, Vector2Int>();

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

                    neightbortiles.Add(neighborTile, new Vector2Int(i, j));
                }
            }
        }

        return neightbortiles;
    }

    // Trying to generate heatmap.
    private void HeatmapGeneration(Vector2Int tilePoint)
    {

        if (tilePoint.x > size - 1 || tilePoint.x < 0)
        {
            Debug.Log("Out of index");
            return;
        }

        if (tilePoint.y > size - 1 || tilePoint.y < 0)
        {
            Debug.Log("Out of index");
            return;
        }

        // Doing those same stuffs as finding neighbors...
        tiles[tilePoint.x, tilePoint.y].heatCost = 0;

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                    continue;

                // Same old index checking...
                int checkX = tilePoint.x + i, checkY = tilePoint.y + j;

                if (checkX >= 0 && checkX < size && checkY >= 0 && checkY < size)
                {
                    // If neighboring tile's heat cost is same as center node then we change the heat cost.
                    if (tiles[tilePoint.x, tilePoint.y].heatCost == tiles[checkX, checkY].heatCost)
                    {
                        // This is working as intended
                        tiles[checkX, checkY].heatCost = tiles[tilePoint.x, tilePoint.y].heatCost + 1;

                        // This part causes out of sync error. Haven't figured a way out to get neighbor nodes' heatmap generation yet.
                        // Plan is to do some sorta neighbor check to keep the value same.
                        // Next finding values will be changed only.


                        //var neighbors = GetNeightbor(new Vector2Int(checkX, checkY));

                        //foreach (var neighbor in neighbors)
                        //{
                        //    if (neighbor.Value.x > size -1)
                        //        continue;
                        //    if (neighbor.Value.x < 0)
                        //        continue;
                        //    if (neighbor.Value.y > size)
                        //        continue;
                        //    if (neighbor.Value.y < 0)
                        //        continue;

                        //    HeatmapGeneration(neighbor.Value);
                        //}
                    }
                }
            }
        }
    }

    public void FindPath(Vector3 target)
    {
        // First we need to generate a heatmap
        
        HeatmapGeneration(GetIndexFromGridPosition(target));
    }

    public Vector2Int GetIndexFromGridPosition(Vector3 position)
    {
        // Made this by reversing from the grid tile generation. Gave some headache for a while. Used basic algebra to get index value. Index check done as well. Maybe not in the best way and don't wanna touch it either.
        int i = Mathf.RoundToInt(((2 * position.x) - (2 * origin.x) - tileSize) / (tileSize * 2));
        if (i > size - 1)
            i = (int)size - 1;
        else if (i < 0)
            i = 0;

        int j = Mathf.RoundToInt(((2 * position.z) - (2 * origin.z) - tileSize) / (tileSize * 2));
        if (j > size - 1)
            j = (int)size - 1;
        else if (j < 0)
            j = 0;

        return new Vector2Int(i, j);
    }

    public Vector3 GetOrigin() // I think I should just public the variable.
    {
        return origin;
    }

    public Vector3 GetGridPositionFromIndex(Vector2Int index)
    {
        return tiles[index.x, index.y].GetPositionIn3Dspace();
    }
}

public class Tile
{
    public Vector2 position = Vector2.zero;
    public Vector2 direction = Vector2.zero; // Kept for now. Might remove it and use angle instead. Then convert the direction from that.
    public int heatCost = 0; // Planned for flow field. Not sure if it's the right way.

    public Tile()
    {
        position = Vector2.zero;
    }

    public Tile(Vector2 _position)
    {
        position = _position;
    }

    public Vector3 GetPositionIn3Dspace()
    {
        return new Vector3(position.x, 0, position.y);
    }
}