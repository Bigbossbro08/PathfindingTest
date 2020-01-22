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

    public void DebugTiles()
    {
        if (tiles.Length > 0)
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    Debug.DrawRay(tiles[i, j].GetPositionIn3Dspace(), Vector3.up, Color.red);
                    Debug.DrawLine(tiles[i, j].GetPositionIn3Dspace() + Vector3.right * (tileSize / 3) + Vector3.forward * (tileSize / 3), tiles[i, j].GetPositionIn3Dspace() + Vector3.right * (tileSize / 3) - Vector3.forward * (tileSize / 3), Color.blue);
                    Debug.DrawLine(tiles[i, j].GetPositionIn3Dspace() - Vector3.right * (tileSize / 3) + Vector3.forward * (tileSize / 3), tiles[i, j].GetPositionIn3Dspace() - Vector3.right * (tileSize / 3) - Vector3.forward * (tileSize / 3), Color.blue);
                    Debug.DrawLine(tiles[i, j].GetPositionIn3Dspace() + Vector3.forward * (tileSize / 3) + Vector3.right * (tileSize / 3), tiles[i, j].GetPositionIn3Dspace() + Vector3.forward * (tileSize / 3) - Vector3.right * (tileSize / 3), Color.blue);
                    Debug.DrawLine(tiles[i, j].GetPositionIn3Dspace() - Vector3.forward * (tileSize / 3) + Vector3.right * (tileSize / 3), tiles[i, j].GetPositionIn3Dspace() - Vector3.forward * (tileSize / 3) - Vector3.right * (tileSize / 3), Color.blue);
                }
            }
        }
    }
    public void DebugTiles(Vector3 playerposition, Vector3 targetposition)
    {
        Vector2Int playerindex = GetIndexFromGridPosition(playerposition);
        Vector3 unitPosition = GetGridPositionFromIndex(playerindex);

        Vector2Int targetindex = GetIndexFromGridPosition(targetposition);
        Vector3 targetunitPosition = GetGridPositionFromIndex(targetindex);

        if (tiles.Length > 0)
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    Debug.DrawRay(unitPosition + Vector3.right * (tileSize / 7), Vector3.up, Color.cyan);
                    Debug.DrawRay(unitPosition + Vector3.left * (tileSize / 7), Vector3.up, Color.cyan);
                    Debug.DrawRay(unitPosition + Vector3.forward * (tileSize / 7), Vector3.up, Color.cyan);
                    Debug.DrawRay(unitPosition + Vector3.back * (tileSize / 7), Vector3.up, Color.cyan);

                    Debug.DrawRay(targetunitPosition + Vector3.right * (tileSize / 7), Vector3.up, Color.green);
                    Debug.DrawRay(targetunitPosition + Vector3.left * (tileSize / 7), Vector3.up, Color.green);
                    Debug.DrawRay(targetunitPosition + Vector3.forward * (tileSize / 7), Vector3.up, Color.green);
                    Debug.DrawRay(targetunitPosition + Vector3.back * (tileSize / 7), Vector3.up, Color.green);

                    Debug.DrawLine(tiles[i, j].GetPositionIn3Dspace() + Vector3.right * (tileSize / 3) + Vector3.forward * (tileSize / 3), tiles[i, j].GetPositionIn3Dspace() + Vector3.right * (tileSize / 3) - Vector3.forward * (tileSize / 3), Color.black);
                    Debug.DrawLine(tiles[i, j].GetPositionIn3Dspace() - Vector3.right * (tileSize / 3) + Vector3.forward * (tileSize / 3), tiles[i, j].GetPositionIn3Dspace() - Vector3.right * (tileSize / 3) - Vector3.forward * (tileSize / 3), Color.black);
                    Debug.DrawLine(tiles[i, j].GetPositionIn3Dspace() + Vector3.forward * (tileSize / 3) + Vector3.right * (tileSize / 3), tiles[i, j].GetPositionIn3Dspace() + Vector3.forward * (tileSize / 3) - Vector3.right * (tileSize / 3), Color.black);
                    Debug.DrawLine(tiles[i, j].GetPositionIn3Dspace() - Vector3.forward * (tileSize / 3) + Vector3.right * (tileSize / 3), tiles[i, j].GetPositionIn3Dspace() - Vector3.forward * (tileSize / 3) - Vector3.right * (tileSize / 3), Color.black);

                    Handles.Label(tiles[i, j].GetPositionIn3Dspace(), tiles[i, j].heatCost.ToString());

                    if (i == playerindex.x && j == playerindex.y)
                        Debug.DrawRay(tiles[i, j].GetPositionIn3Dspace(), new Vector3(tiles[i, j].direction.x, 0, tiles[i, j].direction.y), Color.blue);
                    else if (i == targetindex.x && j == targetindex.y)
                        Debug.DrawRay(tiles[i, j].GetPositionIn3Dspace(), new Vector3(tiles[i, j].direction.x, 0, tiles[i, j].direction.y), Color.green);
                    else
                        Debug.DrawRay(tiles[i, j].GetPositionIn3Dspace(), new Vector3(tiles[i, j].direction.x, 0, tiles[i, j].direction.y), Color.red);
                    
                    if (tiles[i, j].heatCost == int.MaxValue)
                        Debug.DrawRay(tiles[i, j].GetPositionIn3Dspace(), Vector3.up, Color.black);
                }

            }
        }

        Dictionary<Tile, Vector2Int> playerneightborTiles = GetNeightbor(playerindex);
        foreach (KeyValuePair<Tile, Vector2Int> neightborTile in playerneightborTiles)
        {
            Debug.DrawLine(neightborTile.Key.GetPositionIn3Dspace(), neightborTile.Key.GetPositionIn3Dspace() + (Vector3.forward + Vector3.right) * 0.2f, Color.yellow);
            Debug.DrawLine(neightborTile.Key.GetPositionIn3Dspace(), neightborTile.Key.GetPositionIn3Dspace() + (Vector3.forward + Vector3.left) * 0.2f, Color.yellow);
            Debug.DrawLine(neightborTile.Key.GetPositionIn3Dspace(), neightborTile.Key.GetPositionIn3Dspace() + (Vector3.back + Vector3.right) * 0.2f, Color.yellow);
            Debug.DrawLine(neightborTile.Key.GetPositionIn3Dspace(), neightborTile.Key.GetPositionIn3Dspace() + (Vector3.back + Vector3.left) * 0.2f, Color.yellow);
        }

        Dictionary<Tile, Vector2Int> targetneightborTiles = GetNeightbor(targetindex);

        foreach (KeyValuePair<Tile, Vector2Int> neightborTile in targetneightborTiles)
        {
            Debug.DrawLine(neightborTile.Key.GetPositionIn3Dspace(), neightborTile.Key.GetPositionIn3Dspace() + (Vector3.forward + Vector3.right) * 0.2f, Color.yellow);
            Debug.DrawLine(neightborTile.Key.GetPositionIn3Dspace(), neightborTile.Key.GetPositionIn3Dspace() + (Vector3.forward + Vector3.left) * 0.2f, Color.yellow);
            Debug.DrawLine(neightborTile.Key.GetPositionIn3Dspace(), neightborTile.Key.GetPositionIn3Dspace() + (Vector3.back + Vector3.right) * 0.2f, Color.yellow);
            Debug.DrawLine(neightborTile.Key.GetPositionIn3Dspace(), neightborTile.Key.GetPositionIn3Dspace() + (Vector3.back + Vector3.left) * 0.2f, Color.yellow);
        }
    }

    public float GetTotalSize()
    {
        return tileSize * size;
    }

    public void GenerateTiles(Vector3 _origin)
    {
        origin = _origin;
        tiles = new Tile[size, size];
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

    private Dictionary<Tile, Vector2Int> GetNeightbor(Vector2Int tilePoint)
    {
        Dictionary<Tile, Vector2Int> neightbortiles = new Dictionary<Tile, Vector2Int>();

        if (tiles[tilePoint.x, tilePoint.y].heatCost == int.MaxValue)
            return neightbortiles;
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                    continue;

                int checkX = tilePoint.x + i, checkY = tilePoint.y + j;

                if (checkX >= 0 && checkX < size && checkY >= 0 && checkY < size)
                {
                    Tile neighborTile = tiles[checkX, checkY];

                    if (neighborTile.heatCost == int.MaxValue)
                    {
                        continue;
                    }

                    neightbortiles.Add(neighborTile, new Vector2Int(i, j));
                }
            }
        }

        return neightbortiles;
    }

    public void FindPath(Vector3 target)
    {

    }

    public Vector2Int GetIndexFromGridPosition(Vector3 position)
    {
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

    public Vector3 GetOrigin()
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
    public Vector2 direction = Vector2.zero;
    public int heatCost = 0;

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