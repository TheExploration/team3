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
    
    [SerializeField]
    private float bufferZone = 1000f;      // Maximum pixel distance from the player
    
    [SerializeField]
    private float smoothSpeed = 100f;      // Speed at which the camera follows

 
    
    public Vector3 cameraOffset = new Vector3(0, 10, -10); // Offset for the camera
    
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
        if (playerCamera != null) 
        {
            // Get the player's position in screen space
            Vector3 playerScreenPosition = playerCamera.WorldToScreenPoint(transform.position);

            // Calculate the mouse offset from the player's screen position
            Vector2 mouseOffset = (Vector2)Input.mousePosition - (Vector2)playerScreenPosition;

            // Clamp the offset to the buffer zone in screen space
            mouseOffset = Vector2.ClampMagnitude(mouseOffset, bufferZone);

            // Convert the clamped screen-space offset to a world-space offset
            Vector3 offsetWorldSpace = playerCamera.ScreenToWorldPoint(new Vector3(playerScreenPosition.x + mouseOffset.x, playerScreenPosition.y + mouseOffset.y, transform.position.y));
        
            // Calculate the target position by adjusting only the X and Z coordinates
            Vector3 targetPosition = new Vector3(offsetWorldSpace.x, playerCamera.transform.position.y, offsetWorldSpace.z);

            // Smoothly move the camera towards the target position
            playerCamera.transform.position = Vector3.Lerp(playerCamera.transform.position, targetPosition, smoothSpeed);
        }
    }
}