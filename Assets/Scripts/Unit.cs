using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Unit : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private Vector2 movement = Vector2.zero;
    [SerializeField] private float speed = 5;
    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.rotation = Quaternion.Euler(new Vector3(0, rb.rotation.eulerAngles.y, 0));
        transform.Translate(new Vector3(movement.x, 0, movement.y) * speed);
    }
}
