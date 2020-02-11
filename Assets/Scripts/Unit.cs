using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    // Basic movement stuffs...
    [SerializeField] private Vector2 movement = Vector2.zero;
    [SerializeField] private float speed = 5;
    private Pathfinder pathfinder;

    // Start is called before the first frame update
    void Start()
    {
        pathfinder = GetComponent<Pathfinder>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(new Vector3(movement.x, 0, movement.y) * speed);

        movement = pathfinder.FollowToPath(transform.position);
    }
}
