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
    private Rigidbody2D rb2D;

    [SerializeField]
    private BoxCollider2D boxCollider2D;

    
    [HideInInspector]
    public bool canMove = true;
 
    [SerializeField]
    private Camera targetCamera;
    
    public float pixelsPerUnit = 32f; // Match this to your game's PPU
    
    
    private Vector3 snappedPosition;  // The camera's snapped position
    private Vector2 subpixelOffset;   // The offset to apply in the shader

    // Reference to your material
    public Material pixelOffsetMaterial;
    
    private Vector2 movement;

    [SerializeField]
    private float verticalOffset = 3f;
 
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
        // Configure Rigidbody2D properties
        rb2D.freezeRotation = true;
        rb2D.drag = 5f;
        rb2D.angularDrag = 10f;
        rb2D.gravityScale = 0f; // Disable gravity for top-down or side-scrolling games
    }
 
    void Update()
    {
        if (base.IsOwner)
        {
            // Get movement input
            // Movement is now along the X and Y axes
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = -Input.GetAxisRaw("Vertical");
            //RotateTowardsMouse(); // Uncomment if rotation towards mouse is needed
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

        // Apply the rotation on the Z-axis
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    
    void FixedUpdate()
    {
        if (base.IsOwner && canMove)
        {
            // Move the Rigidbody based on input
            Vector2 newPosition = rb2D.position + movement * moveSpeed * Time.fixedDeltaTime;
            rb2D.MovePosition(newPosition);
        }
    }
    
    private void FollowPlayerWithCamera()
    {
        if (targetCamera == null) return; // Ensure the camera is assigned

        // Get the player's position
        Vector3 playerPosition = transform.position;

        // Maintain the camera's current Z position for depth
        playerPosition.z = transform.position.z;

        // Calculate the snap value (size of one pixel in world units)
        float snapValue = 1f / pixelsPerUnit;

        // Snap the camera's position to the pixel grid
        snappedPosition.x = Mathf.Round(playerPosition.x / snapValue) * snapValue;
        snappedPosition.y = Mathf.Round((playerPosition.y + verticalOffset) / snapValue) * snapValue;
        snappedPosition.z = targetCamera.transform.position.z; // Keep Z (depth) constant
        
        // Update the camera's position
        targetCamera.transform.position = snappedPosition;

        // Calculate the subpixel offset
        Vector2 subpixelOffset = new Vector2(
            playerPosition.x - snappedPosition.x,
            snappedPosition.y - (playerPosition.y + verticalOffset)
        );
        
        // Snap the camera position to the pixel grid
        float orthographicSize = targetCamera.orthographicSize;
        
        // Convert to pixels
        float pixelsPerUnitX = 180f / (targetCamera.aspect * orthographicSize * 2f);
        float pixelsPerUnitY = 180f / (orthographicSize * 2f);
        
        Vector2 subpixelOffsetPixels = new Vector2(
            subpixelOffset.x * pixelsPerUnitX,
            subpixelOffset.y * pixelsPerUnitY
        );
        
        Vector2 subpixelOffsetUV = new Vector2(
            subpixelOffsetPixels.x / 180f,
            subpixelOffsetPixels.y / 180f
        );

        // Apply the subpixel offset to the material
        pixelOffsetMaterial.SetVector("_SubpixelOffset", subpixelOffsetUV);
    }
}