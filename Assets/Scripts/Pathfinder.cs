using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    public PathfindingGrid grid;
    public Vector2Int index;
    public Transform target;

    // Start is called before the first frame update
    void Start()
    {
        grid.GenerateTiles(grid.GetOrigin());
    }

    // Update is called once per frame
    void Update()
    {
        grid.FindPath(target.position);
        index = grid.GetIndexFromGridPosition(transform.position);
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            grid.DebugTiles(transform.position, target.position);
        }
    }
}
