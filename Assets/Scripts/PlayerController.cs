using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Component.Animating;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine.UIElements;

public class PlayerController : NetworkBehaviour
{
    [Header("Base setup")]
    [SerializeField]
    private float moveSpeed = 100f;
 
    [SerializeField]
    private Rigidbody2D rb2D;

    [SerializeField]
    private BoxCollider2D boxCollider2D;

    [SerializeField]
    private GameObject[] playerVisuals = new GameObject[4];
    
    [HideInInspector]
    public bool canMove = true;


    private bool moving = false;
    
    [SerializeField]
    private Camera targetCamera;
    
    // Reference to your material
    public Material pixelOffsetMaterial;

    private String currentState = "IDLE_DOWNLEFT";

    private NetworkAnimator activeAnimator;

    
    public float pixelsPerUnit = 32f; // Match this to your game's PPU

    public float horizontalCameraLimit = 1;
    public float verticalCameraLimit = 1;
    
    private Vector3 snappedPosition;  // The camera's snapped position
    private Vector2 subpixelOffset;   // The offset to apply in the shader
    
    private Vector2 movement;
    
    

    [SerializeField]
    private float verticalOffset = 3f;
 
    
    private readonly SyncVar<int> playerId = new SyncVar<int>(-1, new SyncTypeSettings(1f));
    
    private void Awake()
    {
        playerId.OnChange += on_playerID;
    }

    private void on_playerID(int prev, int next, bool asServer)
    {
        /* Each callback for SyncVars must contain a parameter
         * for the previous value, the next value, and asServer.
         * The previous value will contain the value before the
         * change, while next contains the value after the change.
         * By the time the callback occurs the next value had
         * already been set to the field, eg: _health.
         * asServer indicates if the callback is occurring on the
         * server or on the client. Sometimes you may want to run
         * logic only on the server, or client. The asServer
         * allows you to make this distinction. */
        UpdateVisuals(next);
        
    }
    public override void OnStartClient()
    {
        base.OnStartClient();
        
        UpdateVisuals(playerId.Value);
        if (base.IsOwner)
        {
            targetCamera = Camera.main;
        }
    }
 
    private void UpdateVisuals(int id)
    {
        if (id < 0 || id >= playerVisuals.Length)
        {
            Debug.LogError($"Invalid Player ID {id} received for visuals.", this.gameObject);
            // Optionally disable all visuals if ID is invalid
            for (int i = 0; i < playerVisuals.Length; i++)
            {
                if (playerVisuals[i] != null) playerVisuals[i].SetActive(false);
            }
            activeAnimator = null;
            return;
        }

        Debug.Log($"Setting visuals for Player ID: {id}");

        // Loop through all visual GameObjects
        for (int i = 0; i < playerVisuals.Length; i++)
        {
            if (playerVisuals[i] != null)
            {
                // Activate the visual GameObject matching the playerId, deactivate others
                bool isActive = (i == id);
                playerVisuals[i].SetActive(isActive);

                // If this is the active visual, get its Animator component
                if (isActive)
                {
                    activeAnimator = playerVisuals[i].GetComponent<NetworkAnimator>();
                    if (activeAnimator == null)
                    {
                        Debug.LogError($"Active visual for Player ID {id} is missing Animator component!", playerVisuals[i]);
                    }
                }
            }
            else
            {
                Debug.LogWarning($"Player Visuals array index {i} is not assigned in the Inspector.", this.gameObject);
            }
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

    void ChangeAnimationState(String newState)
    {
        if (currentState == newState) return;
        
        activeAnimator.Play(newState);
        
        currentState = newState;
    }
    
    // Returns values from -1 to 1 where 0 is center of screen
    public Vector2 GetMousePosition()
    {
        // Get the current mouse position in screen coordinates
        Vector2 mousePos = Input.mousePosition;
        
        // Convert to normalized position where (0,0) is the center of the screen
        // and (-1,-1) to (1,1) are the edges
        float normalizedX = (mousePos.x / Screen.width) * 2 - 1;
        float normalizedY = (mousePos.y / Screen.height) * 2 - 1;
        
        // Return the normalized position
        return new Vector2(normalizedX, normalizedY);
    }


    void animateMoving(String idle, String walk)
    {
        if (moving)
        {
            ChangeAnimationState(walk);
            
        }
        else
        {
            ChangeAnimationState(idle);
        }
    }
 
    void Update()
    {
        if (!base.IsOwner)
            return;
   
        // Get movement input
        // Movement is now along the X and Y axes
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = -Input.GetAxisRaw("Vertical");

        if (movement.x != 0 || movement.y != 0)
        {
            moving = true;
        }
        else
        {
            moving = false;
        }

        float angle = RotateTowardsMouse();
        Debug.Log($"Angle: {angle}");
        if (angle < 30) {
            animateMoving("IDLE_RIGHT", "WALK_RIGHT");
        } else if (angle < 60) {
            animateMoving("IDLE_RIGHTUP", "WALK_RIGHTUP");
        } else if (angle < 90) {
            animateMoving("IDLE_UP", "WALK_UPRIGHT");
        } else if (angle < 120) {
            animateMoving("IDLE_UP", "WALK_UPLEFT");
        } else if (angle < 150) {
            animateMoving("IDLE_LEFTUP", "WALK_LEFTUP");
        } else if (angle < 180) {
            animateMoving("IDLE_LEFT", "WALK_LEFT");
        } else if (angle < 225) {
            animateMoving("IDLE_LEFT", "WALK_LEFT");
        } else if (angle < 270)
        {
            animateMoving("IDLE_DOWNLEFT","WALK_DOWNLEFT");
        } else if (angle < 315)
        {
            animateMoving("IDLE_DOWNRIGHT", "WALK_DOWNRIGHT");
        }
        else
        {
            animateMoving("IDLE_RIGHT", "WALK_RIGHT");
        }
                
    }
    
    [Server] // Ensures this can only be called on the server
    public void SetPlayerId(int id)
    {
        Debug.Log("set iD!!!!" + id);
        playerId.Value = id;
        // The SyncVar automatically syncs this change to clients, triggering OnPlayerIdChanged
    }

    void LateUpdate()
    {
        FollowPlayerWithCamera();
    }

    private float RotateTowardsMouse()
    {
        // Get the center of the screen in pixel coordinates
        Vector3 centerScreenPosition = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);

        // Get the current mouse position on the screen
        Vector3 mouseScreenPosition = Input.mousePosition;

        // Calculate the direction vector from the center of the screen to the mouse
        // We only need the X and Y components for a 2D angle
        Vector2 direction = (Vector2)mouseScreenPosition - (Vector2)centerScreenPosition;

        // --- No change below this line ---

        // Use the 'right' direction (positive X-axis) as the reference (0 degrees)
        // If your sprite faces upwards by default, you might use Vector2.up here instead.
        Vector2 referenceDirection = Vector2.right;

        // Calculate the angle between the reference direction and the direction to the mouse
        // SignedAngle gives the angle in degrees (-180 to 180)
        float angle = Vector2.SignedAngle(referenceDirection, direction);

        if (angle < 0)
        {
            angle += 360f;
        }

        return angle;
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
        Vector3 cameraPosition = transform.position;

        // Maintain the camera's current Z position for depth
        cameraPosition.z = transform.position.z;
        cameraPosition.y = cameraPosition.y + verticalOffset;

        
        cameraPosition.x = cameraPosition.x + (horizontalCameraLimit * GetMousePosition().x);
        cameraPosition.y = cameraPosition.y - (verticalCameraLimit * GetMousePosition().y);
        
        // Calculate the snap value (size of one pixel in world units)
        float snapValue = 1f / pixelsPerUnit;

        // Snap the camera's position to the pixel grid
        snappedPosition.x = Mathf.Round(cameraPosition.x / snapValue) * snapValue;
        snappedPosition.y = Mathf.Round((cameraPosition.y) / snapValue) * snapValue;
        snappedPosition.z = targetCamera.transform.position.z; // Keep Z (depth) constant
        
        // Update the camera's position
        targetCamera.transform.position = snappedPosition;

        // Calculate the subpixel offset
        Vector2 subpixelOffsetWorld  = new Vector2(
            cameraPosition.x - snappedPosition.x,
            snappedPosition.y - (cameraPosition.y)
        );
        
        // Simplified calculation for UV offset
        float cameraViewHeightWorld = targetCamera.orthographicSize * 2f;
        float cameraViewWidthWorld = cameraViewHeightWorld * targetCamera.aspect; // aspect = width/height

        
        Vector2 subpixelOffsetUV = new Vector2(
            subpixelOffsetWorld.x / cameraViewWidthWorld,
            subpixelOffsetWorld.y / cameraViewHeightWorld
        );

        // Apply the subpixel offset to the material
        pixelOffsetMaterial.SetVector("_SubpixelOffset", subpixelOffsetUV);
    }
}