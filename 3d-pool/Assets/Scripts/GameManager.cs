using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    enum CurrentPlayer { 
        Player1,
        Player2
    }
    CurrentPlayer currentPlayer;
    bool isWinningshotForPlayer1 = false;
    bool isWinningshotForPlayer2 = false;
    int player1BallsRemaining = 7;
    int player2BallsRemaining = 7;
    bool isWaitingForBallMovementToStop = false;
    bool isGameOver = false;
    bool willSwapPlayers = false;
    bool ballPocketed = false;
    public bool hit = false;
    private float currentTimer;
    [SerializeField] float movementThreshhold;
    [SerializeField] float shotTimer = 3f;
    [SerializeField] TextMeshProUGUI player1BallsText;
    [SerializeField] TextMeshProUGUI player2BallsText;
    [SerializeField] TextMeshProUGUI currentTurnText;
    [SerializeField] TextMeshProUGUI messageText;
    [SerializeField] GameObject restartButton;
    [SerializeField] Transform headPosition;
    [SerializeField] GameObject cueStick;


    [SerializeField] Camera cueStickCamera;
    [SerializeField] Camera overheadCamera;
    Camera currentCamera;

    void Start()
    {
        currentPlayer = CurrentPlayer.Player1;
        currentCamera = cueStickCamera;
        cueStickCamera.enabled = true;
        overheadCamera.enabled = false;
        currentTimer = shotTimer;
    }
    void Update()
    {
        if (currentCamera == cueStickCamera) { 
            cueStick.SetActive(true);
        }
        if (currentCamera == overheadCamera)
        {
            cueStick.SetActive(false);
        }
        if (isWaitingForBallMovementToStop && !isGameOver)
        {
            currentTimer -= Time.deltaTime;
            if (currentTimer <= 0)
            {
                bool allStopped = true;
                foreach (GameObject ball in GameObject.FindGameObjectsWithTag("Ball"))
                {
                    if (ball.GetComponent<Rigidbody>().velocity.magnitude >= movementThreshhold)
                    {
                        allStopped = false;
                        break;
                    }
                }

                if (allStopped)
                {
                    isWaitingForBallMovementToStop = false;
                    SwitchCameras();
                    if (willSwapPlayers || !ballPocketed)
                    {
                        NextPlayerTurn(); 
                    }
                    currentTimer = shotTimer;
                    ballPocketed = false;
                    if (!hit) { 
                        foreach(GameObject ballObj in GameObject.FindGameObjectsWithTag("Ball")){
                            Ball ball = ballObj.GetComponent<Ball>();
                            if (ball != null && ball.isItCueBall())
                            {
                                ballObj.transform.position = headPosition.position;
                                ballObj.GetComponent<Rigidbody>().velocity = Vector3.zero;
                                ballObj.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                                currentCamera.gameObject.GetComponent<CameraController>().ResetCamera();
                                break;
                            }
                        }
                    }
                    hit = false;
                }
            }
        }
    }
    public void SwitchCameras() {
        if (currentCamera == cueStickCamera)
        {
            cueStickCamera.enabled = false;
            cueStick.SetActive(false);
            overheadCamera.enabled = true;
            currentCamera = overheadCamera;
            isWaitingForBallMovementToStop = true;
        }
        else {
            overheadCamera.enabled = false;
            cueStick.SetActive(false);
            cueStickCamera.enabled = true;
            currentCamera = cueStickCamera;
            currentCamera.gameObject.GetComponent<CameraController>().ResetCamera();
        }
    }
    bool Scratch() {
        if (currentPlayer == CurrentPlayer.Player1) {
            if (isWinningshotForPlayer1) { 
                ScratchOnWinningShot("Player 1");
                return true;
            }
        }
        else { if (isWinningshotForPlayer2) {
                ScratchOnWinningShot("Player 2");
                return true;
            } 
        }
        willSwapPlayers = true;
        return false;
    }
    void EarlyEightBall() {
        if (currentPlayer == CurrentPlayer.Player1)
        {
            Lose("Player 1 hit in the eight ball too early and has lost!");
        }
        else {
            Lose("Player 2 hit in the eight ball too early and has lost!");
        }
    }
    void ScratchOnWinningShot(string player) {
        Lose(player + "Scratched on their final shot and has lost!");
    }
    bool CheckBall(Ball ball)
    {
        hit = true;
        if (ball.isItCueBall())
        {
            if (Scratch())
            {
                return true;
            }
            else return false;
        }
        else if (ball.isEightBall())
        {
            if (currentPlayer == CurrentPlayer.Player1)
            {
                if (isWinningshotForPlayer1)
                {
                    if (Scratch())
                    {
                        Lose("Player 1 scratched on the eight ball and has lost!");
                    }
                    else
                    {
                        Win("Player 1 ");
                    }
                    return true;
                }
            }
            else
            {
                if (isWinningshotForPlayer2)
                {
                    if (Scratch())
                    {
                        Lose("Player 2 scratched on the eight ball and has lost!");
                    }
                    else
                    {
                        Win("Player 2 ");
                    }
                    return true;
                }
            }
            EarlyEightBall();
        }
        else
        {
            if (ball.isBallRed())
            {
                player1BallsRemaining--;
                player1BallsText.text = "Player 1 Balls remaining: " + player1BallsRemaining;
                if (player1BallsRemaining <= 0)
                {
                    isWinningshotForPlayer1 = true;
                }
                if (currentPlayer != CurrentPlayer.Player1)
                {
                    //NextPlayerTurn();
                    isWaitingForBallMovementToStop = true;
                }
            }
            else
            {
                player2BallsRemaining--;
                player2BallsText.text = "Player 2 Balls remaining: " + player2BallsRemaining;
                if (player2BallsRemaining <= 0)
                {
                    isWinningshotForPlayer2 = true;
                }
                if (currentPlayer != CurrentPlayer.Player2)
                {
                    //NextPlayerTurn();
                    isWaitingForBallMovementToStop = true;
                }

            }
        }
        return true;
    }
    void Lose(string message) { 
        isGameOver = true;
        messageText.gameObject.SetActive(true);
        messageText.text = message;
        restartButton.SetActive(true);
    }
    void Win(string player)
    {
        isGameOver = true;
        messageText.gameObject.SetActive(true);
        messageText.text = player + "has won!";
        restartButton.SetActive(true);
    }
    void NextPlayerTurn() {
        if (currentPlayer == CurrentPlayer.Player1)
        {
            currentPlayer = CurrentPlayer.Player2;
            currentTurnText.text = "Current Turn : Player 2";
        }
        else {
            currentPlayer = CurrentPlayer.Player1;
            currentTurnText.text = "Current Turn : Player 1";
        }
        willSwapPlayers = false;
        ballPocketed = false;

        if (currentCamera != cueStickCamera)
        {
            SwitchCameras();
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Ball") {
                ballPocketed = true;
            if (CheckBall(other.gameObject.GetComponent<Ball>()))
            {
                Destroy(other.gameObject);
            }
            else {
                other.gameObject.transform.position = headPosition.position;
                other.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                other.gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            }
        }
    }
    public void RestartTheGame() {
        SceneManager.LoadScene(0);
    }
}
