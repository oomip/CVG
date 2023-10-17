using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuakeSpell : MonoBehaviour
{
    public static bool quakeActive = false;
    // Scheduling
    private float nextActionTime = 0.0f;
    public float period = 3f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (Time.time > nextActionTime ) {
            nextActionTime += period;
            quakeActive = !quakeActive;
        }

    }
}
