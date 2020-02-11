using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    public PathfindingGrid grid;
    public Vector2Int index;
    public Transform target;
    public bool pathFind = true;
    public GameObject[] obstacles;

    // Start is called before the first frame update
    void Start()
    {
        // Just generating grid at start.
        grid.origin = Vector3.zero;
        grid.GenerateTiles();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var o in obstacles)
        {
            Vector2Int index = grid.GetIndexFromGridPosition(o.transform.position);
            var neighbors = grid.GetNeightborAStar(index);

            foreach (var n in neighbors)
            {
                if (n.distance == -2)
                    continue;
                n.distance = -1;
            }

            grid.tiles[index.x, index.y].distance = -2;
        }

        grid.UpdateFlowField(target.position);
        index = grid.GetIndexFromGridPosition(transform.position);

        if (pathFind == true)
        {
            pathFind = false;
        }
    }

    public Vector3 FollowToPath(Vector3 pos)
    {
        Vector2Int unitIndex = grid.GetIndexFromGridPosition(pos);
        //Vector2Int targetIndex = grid.GetIndexFromGridPosition(target.position);

        return grid.tiles[unitIndex.x, unitIndex.y].direction;
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            // Drawing the grid. Using the different function because Unity have some specific functions for specific events. Can't be called in OnUpdate method.
            grid.DebugTiles(transform.position, target.position);
        }
    }
}
