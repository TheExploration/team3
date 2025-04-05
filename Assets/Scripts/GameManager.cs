using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    
    [SerializeField] private GameObject playerPrefab;
    
    
    private readonly SyncVar<int> connectedPlayerCount = new SyncVar<int>(0);
    private const int MAX_PLAYERS = 4;


    
    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    public override void OnStartClient()
    {
        RpcSpawnPlayer();
    }

    [ServerRpc]
    private void RpcSpawnPlayer(NetworkConnection conn = null)
    {
        if (connectedPlayerCount.Value < MAX_PLAYERS)
        {
            GameObject playerInstance = Instantiate(playerPrefab);
            
            
            
            
            base.Spawn(playerInstance, conn); //networkBehaviour.
            PlayerController pc = playerInstance.GetComponent<PlayerController>();
            if (pc != null)
            {
                // Call the Server method on PlayerController to set the SyncVar Player ID
                pc.SetPlayerId(connectedPlayerCount.Value); // This assumes PlayerController has `public void SetPlayerId(int id)` marked with [Server]
                Debug.Log(connectedPlayerCount.Value);
            }
            connectedPlayerCount.Value++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
