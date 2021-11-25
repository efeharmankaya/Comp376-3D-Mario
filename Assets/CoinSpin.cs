// ------------------------------------------------------------------------------
// Quiz
// Written by: Efe Harmankaya - 40077277
// For COMP 376 – Fall 2021
// Controls the spin and floating of coins scattered around the world
// -----------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// https://www.youtube.com/watch?v=oxwMsCFKzPM
public class CoinSpin : MonoBehaviour
{
    public float spinSpeed;
    private float floatDelta = 0.25f;
    private Vector3 startPosition;
    private bool goingUp = true;
    void Start(){
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, spinSpeed, 0, Space.World);

        if(goingUp && transform.position.y >= startPosition.y + floatDelta)
            goingUp = false;
        if(!goingUp && transform.position.y <= startPosition.y)
            goingUp = true;

        if(goingUp) // float up
            transform.Translate(Vector3.up * Time.deltaTime / 5, Space.World);
        else // float down
            transform.Translate(-Vector3.up * Time.deltaTime / 5, Space.World);
    }
}
