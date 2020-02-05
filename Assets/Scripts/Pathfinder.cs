using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    public PathfindingGrid grid;
    public Vector2Int index;
    public Transform target;
    public bool pathFind = true;

    // Start is called before the first frame update
    void Start()
    {
        // Just generating grid at start.
        grid.GenerateTiles(grid.GetOrigin());
    }

    // Update is called once per frame
    void Update()
    {
        if (pathFind == true)
        {
            grid.UpdateFlowField(target.position);
            index = grid.GetIndexFromGridPosition(transform.position);
            pathFind = false;
        }
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
