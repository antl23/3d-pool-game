using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class GameSetUp : MonoBehaviour
{
    int redBallsRemaining = 7;
    int blueBallsRemaining = 7;
    float radius;
    float diameter;
    [SerializeField] GameObject ballPrefab;
    [SerializeField] Transform cueBallPosition;
    [SerializeField] Transform headBallPosition;

    private void Awake()
    {
        radius = ballPrefab.GetComponent<SphereCollider>().radius;
        diameter = radius * 2;
        placeAllBalls();
    }
    void placeAllBalls() {
        placeCueBall();
        placeRandomBalls();
    }
    void placeCueBall() {
        GameObject ball = Instantiate(ballPrefab, cueBallPosition.position, Quaternion.identity);
        ball.GetComponent<Ball>().makeCueBall();
    }
    void placeEightBall(Vector3 position) {
        GameObject ball = Instantiate(ballPrefab, position, Quaternion.identity);
        ball.GetComponent<Ball>().makeEightBall();
    }
    void placeRandomBalls() {
        int NumInThisRow = 1;
        int rand;
        Vector3 firstInRowPosition = headBallPosition.position;
        Vector3 currPosition = firstInRowPosition;

        void placeRedBall(Vector3 position) {
            GameObject ball = Instantiate(ballPrefab, position, Quaternion.identity);
            ball.GetComponent<Ball>().ballSetUp(true);
            redBallsRemaining--;
        }
        void placeBlueBall(Vector3 position)
        {
            GameObject ball = Instantiate(ballPrefab, position, Quaternion.identity);
            ball.GetComponent<Ball>().ballSetUp(false);
            blueBallsRemaining--;
        }

        for (int i = 0; i < 5; i++) {
            for (int j = 0; j < NumInThisRow; j++) {
                if (i == 2 && j == 1)
                {
                    placeEightBall(currPosition);
                }
                else if (0 < redBallsRemaining && 0 < blueBallsRemaining)
                {
                    rand = Random.Range(0, 1);
                    if (rand == 0)
                    {
                        placeRedBall(currPosition);
                    }
                    else
                    {
                        placeBlueBall(currPosition);
                    }
                }
                else if (0 < redBallsRemaining)
                {
                    placeRedBall(currPosition);
                }
                else if (0 < blueBallsRemaining) 
                {
                    placeBlueBall(currPosition);
                }
                currPosition += new Vector3(1, 0, 0).normalized * diameter;
            }
            firstInRowPosition += Vector3.back * (Mathf.Sqrt(3) * radius * 1.05f) + Vector3.left * radius * 1.05f;
            currPosition = firstInRowPosition;
            NumInThisRow++;
        }
    }
}
