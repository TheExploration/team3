using System.Collections;
using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEditor;
using UnityEngine;

public class InventoryManager : NetworkBehaviour
{

     // SyncVar for the currently selected weapon index
    private readonly SyncVar<int> selectedWeapon = new SyncVar<int>(-1); // Default to -1 (no weapon selected)

    // SyncLists to store networked weapons and items
    private readonly SyncList<NetworkObject> weapons = new SyncList<NetworkObject>();
    private readonly SyncList<NetworkObject> items = new SyncList<NetworkObject>();

    // Maximum number of weapons and items allowed
    //private const int MAX_WEAPONS = 3;
    //private const int MAX_ITEMS = 5;

    public GameObject startingWeaponPrefab;

    public NetworkObject rightHand;
    public NetworkObject leftHand;

    private bool hasFlipped = false;
    
    
    public override void OnStartServer()
    {
        base.OnStartServer();
        if (startingWeaponPrefab != null)
        {
            // Instantiate and spawn the starting weapon
            GameObject weaponInstance = Instantiate(startingWeaponPrefab);
            base.Spawn(weaponInstance);
            NetworkObject weaponNetObj = weaponInstance.GetComponent<NetworkObject>();
            
            // Add to inventory and parent to player
            weapons.Add(weaponNetObj);
            selectedWeapon.Value = 0; // Select the starting weapon
            weaponNetObj.SetParent(rightHand);
        }
    }
    
    // Called when the client starts, including late joiners
    public override void OnStartClient()
    {
        base.OnStartClient();
        
        
        // Register callback for weapon list changes
        weapons.OnChange += OnWeaponsChanged;
        selectedWeapon.OnChange += OnSelectedWeaponChanged;
        // Set initial visibility state
        CmdSetSelectedWeapon(0);
        UpdateWeaponVisibility();

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

    private void Update()
    {
        if (!base.IsOwner) return;
        
        // Handle weapon switching input
        HandleWeaponSwitching();
        
        NetworkObject networkObject = weapons[selectedWeapon.Value];

        float angle = RotateTowardsMouse();
        
        networkObject.transform.eulerAngles = new Vector3(0, 0, -angle);
        
        
        if (!hasFlipped)
        {
            if (angle < 265 && angle > 95)
            {
                hasFlipped = true;
                Vector3 newScale = networkObject.transform.localScale;
                newScale.y *= -1; // Ensure X scale is flipped negatively
                networkObject.transform.localScale = newScale;
                networkObject.SetParent(leftHand);
            }
        }
        else
        {
            if (angle > 275 || angle < 85)
            {
                hasFlipped = false;
                Vector3 newScale = networkObject.transform.localScale;
                newScale.y *= -1; // Ensure X scale is flipped negatively
                networkObject.transform.localScale = newScale;
                networkObject.SetParent(rightHand);

            }
        }
        
        
        if (angle > 180)
        {
            networkObject.transform.localPosition = new Vector3(0, -0.25f, -0.25f);
        }
        else
        {
            networkObject.transform.localPosition = new Vector3(0, 0.25f, 0.25f);

        }
            
            
        
    }

    // Handle weapon switching via number keys
    private void HandleWeaponSwitching()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && weapons.Count >= 1)
            CmdSetSelectedWeapon(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2) && weapons.Count >= 2)
            CmdSetSelectedWeapon(1);
        else if (Input.GetKeyDown(KeyCode.Alpha3) && weapons.Count >= 3)
            CmdSetSelectedWeapon(2);
    }

    // Server RPC to set the selected weapon
    [ServerRpc(RequireOwnership = false)]
    private void CmdSetSelectedWeapon(int index)
    {
        if (index >= 0 && index < weapons.Count)
            selectedWeapon.Value = index;
    }

    // Callback when selectedWeapon SyncVar changes
    private void OnSelectedWeaponChanged(int oldValue, int newValue, bool asServer)
    {
        UpdateWeaponVisibility();
    }

    // Callback when weapons SyncList changes
    private void OnWeaponsChanged(SyncListOperation op, int index, NetworkObject oldItem, NetworkObject newItem, bool asServer)
    {
        
        UpdateWeaponVisibility();
        
    }

    // Update weapon visibility based on selected index
    private void UpdateWeaponVisibility()
    {
        for (int i = 0; i < weapons.Count; i++)
        {
            if (weapons[i] != null)
                weapons[i].gameObject.SetActive(i == selectedWeapon.Value);
        }
    }

    // Public method to pick up a weapon
    public void PickupWeapon(NetworkObject weaponNetObj)
    {
        if (base.IsOwner)
        {
            CmdPickupWeapon(weaponNetObj);
        }
    }

    // Server RPC to handle weapon pickup
    [ServerRpc]
    private void CmdPickupWeapon(NetworkObject weaponNetObj)
    {
        
        
        weapons.Add(weaponNetObj);
        weaponNetObj.SetParent(rightHand);
        weaponNetObj.transform.localPosition = Vector3.zero; // Adjust as needed
        if (weapons.Count == 1)
        {
            selectedWeapon.Value = 0; // Auto-select first weapon
        }
        
    }

    // Public method to pick up an item
    public void PickupItem(NetworkObject itemNetObj)
    {
        if (base.IsOwner)
        {
            CmdPickupItem(itemNetObj);
        }
    }

    // Server RPC to handle item pickup
    [ServerRpc]
    private void CmdPickupItem(NetworkObject itemNetObj)
    {
       
        items.Add(itemNetObj);
        itemNetObj.transform.SetParent(transform);
        itemNetObj.transform.localPosition = Vector3.zero;
        itemNetObj.gameObject.SetActive(false); // Items are stored, not displayed
        
    }



}
