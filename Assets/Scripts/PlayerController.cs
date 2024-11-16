using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;
 

public class PlayerController : NetworkBehaviour
{
    [Header("Base setup")]
    [SerializeField]
    private float moveSpeed = 100f;
 
    [SerializeField]
    private Rigidbody rb;
    
    [HideInInspector]
    public bool canMove = true;
 
    [SerializeField]
    private Camera targetCamera;
    
    public float pixelsPerUnit = 32f; // Match this to your game's PPU
    
    [SerializeField]
    private float bufferZone = 1f;      // Maximum pixel distance from the player
    
    private Vector3 snappedPosition;  // The camera's snapped position
    private Vector2 subpixelOffset;   // The offset to apply in the shader

    
    
    private Vector3 movement;
 
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (base.IsOwner)
        {
            targetCamera = Camera.main;
            // Find the RawImageOffsetController in the scene
            RawImageOffsetController offsetController = FindObjectOfType<RawImageOffsetController>();

            if (offsetController != null)
            {
                // Pass the player's transform to the RawImageOffsetController
                offsetController.SetPlayerTransform(this.transform);
            }
            else
            {
                Debug.LogError("RawImageOffsetController not found in the scene.");
            }
            
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
        
        

    }

    void LateUpdate()
    {
        FollowPlayerWithCamera();
    }
    private void RotateTowardsMouse()
    {
        
        
        // Get the screen positions of the object and the mouse
        Vector3 objectScreenPosition = Camera.main.WorldToScreenPoint(transform.position);
        Vector3 mouseScreenPosition = Input.mousePosition;
        
        
        // Calculate the direction vector from the object to the mouse
        Vector2 direction = (mouseScreenPosition - objectScreenPosition).normalized;
        
        // Use the right direction as the reference
        Vector2 referenceDirection = Vector2.right;
        
        // Calculate the angle between the reference direction and the direction to the mouse
        float angle = Vector2.SignedAngle(referenceDirection, direction);

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
        if (targetCamera == null) return; // Ensure the camera is assigned

        // Get the player's position
        Vector3 playerPosition = transform.position;

        // Maintain the camera's current y position
        playerPosition.y = targetCamera.transform.position.y;

        
        // Calculate the snap value (size of one pixel in world units)
        float snapValue = 1f / pixelsPerUnit;

        // Snap the camera's position to the pixel grid
        snappedPosition.x = Mathf.Round(playerPosition.x / snapValue) * snapValue;
        snappedPosition.z = Mathf.Round(playerPosition.z / snapValue) * snapValue;
        snappedPosition.y = targetCamera.transform.position.y; // Keep Y (depth) constant
        
        
        // Update the camera's position
        targetCamera.transform.position = snappedPosition;

        
        
        
    }
}