using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private bool isRed;
    private bool is8Ball = false;
    private bool isCueBall = false;
    Rigidbody rb;

    void Start() { 
        rb = GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        if (rb.velocity.y > 0) { 
            Vector3 newVelocity = rb.velocity;
            newVelocity.y = 0f;
            rb.velocity = newVelocity;
        }
    }

    public bool isBallRed() {
        return isRed;
    }
    public bool isItCueBall() {
        return isCueBall;
    }
    public bool isEightBall() {
        return is8Ball;
    }

    public void ballSetUp(bool red)
    {
        isRed = red;
        if (isRed)
        {
            GetComponent<Renderer>().material.color = Color.red;
        }
        else {
            GetComponent<Renderer>().material.color = Color.blue;
        }
    }
    public void makeCueBall() { 
        isCueBall = true;
    } 
    public void makeEightBall() { 
        is8Ball = true;
        GetComponent<Renderer>().material.color = Color.black;
    }
}

