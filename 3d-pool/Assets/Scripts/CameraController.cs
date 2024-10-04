using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] float rotationSpeed;
    [SerializeField] Vector3 offset;
    [SerializeField] float downAngle;
    [SerializeField] float power;
    private float horizontalInput;
    private bool isTakingShot = false;
    [SerializeField] float maxDrawDistance;
    private float savedMousePosition;
    Transform cueBall;
    GameManager gameManager;
    [SerializeField] TextMeshProUGUI powerText;

    void Start()
    {   
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        foreach (GameObject ball in GameObject.FindGameObjectsWithTag("Ball")) {
            if (ball.GetComponent<Ball>().isItCueBall()) {
                cueBall = ball.transform; break;
            }
        }
        ResetCamera();
    }

    
    void Update()
    {
        if (cueBall != null && !isTakingShot) {
            horizontalInput = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            transform.RotateAround(cueBall.position, Vector3.up, horizontalInput);
        }
        Shoot();
    }
    public void ResetCamera() {
        transform.position = cueBall.position + offset;
        transform.LookAt(cueBall.position);
        transform.localEulerAngles = new Vector3 (downAngle, transform.localEulerAngles.y, 0);
    }
    void Shoot()
    {
        if (gameObject.GetComponent<Camera>().enabled)
        {
            if (Input.GetButtonDown("Fire1") && !isTakingShot)
            {
                isTakingShot = true;
                savedMousePosition = 0f; 
            }
            else if (isTakingShot)
            {
                float mouseY = Input.GetAxis("Mouse Y");
                savedMousePosition += mouseY;

                savedMousePosition = Mathf.Clamp(savedMousePosition, 0, maxDrawDistance);

                float powerValue = (savedMousePosition / maxDrawDistance) * 100f;
                int powerValueInt = Mathf.RoundToInt(powerValue); 

                powerText.text = "Power : " + powerValueInt.ToString() + "%";
                if (Input.GetButtonUp("Fire1"))
                {
                    Vector3 hitDirection = transform.forward;
                    hitDirection = new Vector3(hitDirection.x, 0, hitDirection.z).normalized;
                    float normalizedPower = powerValue / 10f;

                    cueBall.gameObject.GetComponent<Rigidbody>().AddForce(hitDirection * normalizedPower * power, ForceMode.Impulse);

                    gameManager.SwitchCameras();
                    isTakingShot = false;
                }
            }
        }
    }
}
