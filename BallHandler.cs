using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BallHandler : MonoBehaviour
{
    // ------------- Inspector fields -------------
    // Make a reference to the Prefab where the ball is, such that we can respan it as many times as we like
    [SerializeField] private GameObject ballPrefab;
    // In inspector also add a reference to the pivot
    [SerializeField] private Rigidbody2D pivot;
    [SerializeField] private float detachDelay;
    // Add a respan delay in the inspector
    [SerializeField] private float respawnDelay;

    // Variables
    private Rigidbody2D currentBallRigidbody;
    private SpringJoint2D currentBallSprintJoint;
    private Camera mainCamera;
    private bool isDragging;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        SpawnNewBall();
    }

    // Update is called once per frame
    void Update()
    {
        // If there is no ball, do not run any code below
        if(currentBallRigidbody == null) { return; }

        // If we do not press the screen, just do not execute next lines
        if(!Touchscreen.current.primaryTouch.press.isPressed)
        {
            if(isDragging) {
                LaunchBall();
            }

             isDragging = false;

            return;
        }

        isDragging = true;

        // Take out of physic control
        currentBallRigidbody.isKinematic = true;

        // Record the touch on the screen
        Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();

        // This will record the position in px
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(touchPosition);

        currentBallRigidbody.position = worldPosition;
    }

    private void SpawnNewBall() {
        // We want to spawn the prefab ball to the pivot point every time
        // Last parameter is the rotation type, stored as the data type called quaternion (has x,y,z,w coord)
        // Quaternion.identity is the default rotation, we do not care about it at all
        // This Instancient will return a GameQbject, which is an instance of the ball
        GameObject ballInstance = Instantiate(ballPrefab, pivot.position, Quaternion.identity);

        // Get the rigid body
        currentBallRigidbody = ballInstance.GetComponent<Rigidbody2D>();
        // Get the pivot
        currentBallSprintJoint = ballInstance.GetComponent<SpringJoint2D>();


        // Attach the ball to the pivot manually
        currentBallSprintJoint.connectedBody = pivot;
        
    }

    private void LaunchBall() {

        // Make the ball react to physics again
        currentBallRigidbody.isKinematic = false;

        // Clear the reference entirely
        // We do not want the ball to come back to where we were
        currentBallRigidbody = null;

        // To not have both the physics and detach it from the spring in the same frame
        // Will add a short delay to it
        // Use following "Invoke" method to call the DetachBall() after a certain amount of time
        Invoke(nameof(DetachBall), detachDelay);
    }

    private void DetachBall() {
        // Once we launch the ball, the spring joint is disabled
        // So the ball will not hold up to it anymore
        currentBallSprintJoint.enabled = false;
        currentBallSprintJoint = null;

        Invoke(nameof(SpawnNewBall), respawnDelay);
    }
}
