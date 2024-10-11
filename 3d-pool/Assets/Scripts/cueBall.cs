using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cueBall : MonoBehaviour
{
    private GameManager gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ball")
        {
            gameManager.hit = true;
        }
    }
}
