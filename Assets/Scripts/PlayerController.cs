using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;
 

public class PlayerController : NetworkBehaviour
{
    [Header("Base setup")]
    [SerializeField]
    private float moveSpeed = 2f;
 
    [SerializeField]
    private Rigidbody rb;
    
    [HideInInspector]
    public bool canMove = true;
 
    [SerializeField]
    private Camera targetCamera;
    
    public float pixelsPerUnit = 32f; // Match this to your game's PPU
    
    [SerializeField]
    private float bufferZone = 1f;      // Maximum pixel distance from the player
    
    public float smoothSpeed = 15f; // Adjust as needed

    private Vector3 smoothPosition;
    private Vector3 snappedPosition;
    private Vector3 offset;

    
    
    private Vector3 movement;
 
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (base.IsOwner)
        {
            targetCamera = Camera.main;
            
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
        Vector3 targetPosition = transform.position;

// Maintain the camera's current y position
        targetPosition.y = targetCamera.transform.position.y;

        // Move the camera towards the player's position
        Vector3 movedPosition = Vector3.MoveTowards(
            targetCamera.transform.position,
            targetPosition,
            10f * Time.deltaTime
        );

        // Debug: Log positions before snapping
        Debug.Log($"Before Snapping - Camera Pos: {targetCamera.transform.position}, Moved Pos: {movedPosition}");

        // Snap the moved camera position to the pixel grid
        movedPosition.x = Mathf.Round(movedPosition.x * pixelsPerUnit) / pixelsPerUnit;
        movedPosition.z = Mathf.Round(movedPosition.z * pixelsPerUnit) / pixelsPerUnit;

        // Debug: Log positions after snapping
        Debug.Log($"After Snapping - Moved Pos: {movedPosition}");
        movedPosition.y = targetCamera.transform.position.y;
// Update the camera's position
        targetCamera.transform.position = movedPosition;
        
    }
}