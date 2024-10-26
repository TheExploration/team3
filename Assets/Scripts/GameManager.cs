using System.Collections;
using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    
    [SerializeField] private GameObject playerPrefab;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    public override void OnStartClient()
    {
        base.OnStartClient();
        RpcSpawnPlayer();
    }

    [ServerRpc]
    private void RpcSpawnPlayer(NetworkConnection conn = null)
    {
        GameObject go = Instantiate(playerPrefab);
        base.Spawn(go, conn); //networkBehaviour.
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
