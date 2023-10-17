using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuakeSpell : MonoBehaviour
{
    private bool quakeActive = false;
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
            if (quakeActive) {
                nextActionTime += Random.Range(2,3) * period;
            } else {
                nextActionTime += period;
            }
            quakeActive = !quakeActive;
        }

    }
    public bool isQuakeActive() {
        return quakeActive;
    }
}
