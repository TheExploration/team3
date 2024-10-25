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
        
    }
 
    void Update()
    {
        if (base.IsOwner)
        {
            // Get movement input
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.z = Input.GetAxisRaw("Vertical");
        }
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
}