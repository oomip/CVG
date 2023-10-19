using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Direction : MonoBehaviour
{
    public PlayerControls playerControls;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = playerControls.getPos();

    }

    // Update is called once per frame
    void Update()
    {
        Vector2 facing = playerControls.getFacingDirection();
        Vector3 toFace = new Vector3(facing[0],facing[1], 0);
        transform.rotation = Quaternion.LookRotation(toFace);

        transform.position = playerControls.getPos();
    }
}
