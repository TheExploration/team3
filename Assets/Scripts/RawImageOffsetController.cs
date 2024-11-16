using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class RawImageOffsetController : MonoBehaviour
{
    public float pixelsPerUnit = 32f;   // Match this to your game's settings
    public Camera targetCamera;         // Reference to the main camera

    private RawImage rawImage;
    private Transform playerTransform;
    
    private Vector2 previousSubpixelOffset = Vector2.zero;
    private float smoothingFactor = 0.5f; // Adjust as needed
    void Start()
    {
        rawImage = GetComponent<RawImage>();

        if (targetCamera == null)
        {
            Debug.LogError("Target Camera not assigned.");
        }
    }

    // New method to set the player's transform
    public void SetPlayerTransform(Transform player)
    {
        playerTransform = player;
    }


    void LateUpdate()
    {
        SubpixelOffset();
    }

 
    void SubpixelOffset()
    {
        if (playerTransform == null)
            return; // Player not yet assigned

        // Get the player's position relative to the camera
        Vector3 playerPosition = playerTransform.position;
        Vector3 cameraPosition = targetCamera.transform.position;

        // Calculate the offset between the player and camera in world units
        Vector3 offset = playerPosition - cameraPosition;
       

        // Convert offset to pixel units
        float offsetX = offset.x % 1f;
        float offsetY = offset.z % 1f; // Use offset.z if depth is along the y-axis
        
        
        // Adjust the range from [0,1) to [-0.5, 0.5]
        if (offsetX > 0.5f) offsetX -= 1f;
        if (offsetY > 0.5f) offsetY -= 1f;
        



        


        
        // Determine the size of the camera's view in world units
        float cameraHeight = targetCamera.orthographicSize * 2f;
        float cameraWidth = cameraHeight * targetCamera.aspect;
        
        // Map the fractional world units to UV space
        float uvOffsetX = offsetX / cameraWidth;
        float uvOffsetY = offsetY / cameraHeight;

        // Get the fractional part to find the subpixel offset
        float subpixelX = uvOffsetX;
        float subpixelY = uvOffsetY;

        // Apply the subpixel offset to the Raw Image UV Rect
        Rect uvRect = rawImage.uvRect;
        uvRect.x = subpixelX;
        uvRect.y = subpixelY;
        rawImage.uvRect = uvRect;
    }
}