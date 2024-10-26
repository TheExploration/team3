using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;
 

public class PlayerController : NetworkBehaviour
{
    [Header("Base setup")]
    [SerializeField]
    private float moveSpeed = 5f;
 
    [SerializeField]
    private Rigidbody rb;
    
    [HideInInspector]
    public bool canMove = true;
 
    [SerializeField]
    private Camera playerCamera;
 
    
    public Vector3 cameraOffset = new Vector3(0, 10, -10); // Offset for the camera
    public float smoothSpeed = 0.125f;  // Speed for smooth camera follow
    
    private Vector3 movement;
 
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (base.IsOwner)
        {
            playerCamera = Camera.main;
            
        }
        else
        {
            gameObject.GetComponent<PlayerController>().enabled = false;
        }
    }
 
    void Start()
    {
        rb.freezeRotation = true;
        rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
        rb.drag = 5f;
        rb.angularDrag = 10f;
    }
 
    void Update()
    {
        if (base.IsOwner)
        {
            // Get movement input
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.z = Input.GetAxisRaw("Vertical");
            RotateTowardsMouse();
        }
        
        FollowPlayerWithCamera();

    }
    private void RotateTowardsMouse()
    {
        // Step 1: Get the screen center and mouse position
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
        Vector3 mousePosition = Input.mousePosition;

        // Step 2: Calculate direction and angle
        Vector3 direction = mousePosition - screenCenter;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Step 3: Apply the rotation on the Z-axis
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    
    
    void FixedUpdate()
    {
        if (base.IsOwner && canMove)
        {
            // Move the Rigidbody2D based on input
            Vector3 newPosition = rb.position + movement * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(newPosition);
        }
        
    }
    
    private void FollowPlayerWithCamera()
    {
        // Check if the camera is assigned
        if (playerCamera != null)
        {
            // Calculate the target position based on the player position and offset
            Vector3 targetPosition = transform.position + cameraOffset;
            
            // Smoothly move the camera towards the target position
            playerCamera.transform.position = Vector3.Lerp(playerCamera.transform.position, targetPosition, smoothSpeed);
        }
    }
}