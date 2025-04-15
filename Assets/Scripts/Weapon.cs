using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class Weapon : NetworkBehaviour
{

        /// <summary>
    /// Projectile to spawn.
    /// </summary>
    [Tooltip("Projectile to spawn.")]
    [SerializeField]
    private Bullet _bullet;
    /// <summary>
    /// Maximum amount of passed time a projectile may have.
    /// This ensures really laggy players won't be able to disrupt
    /// other players by having the projectile speed up beyond
    /// reason on their screens.
    /// </summary>
    private const float MAX_PASSED_TIME = 0.3f;

    /// <summary>
    /// Local client fires weapon.
    /// </summary>
    public void ClientFire()
    {
        Vector3 position = transform.position;
        Vector3 direction = transform.forward;

        /* Spawn locally with 0f passed time.
        * Since this is the firing client
        * they do not need to accelerate/catch up
        * the projectile. */
        SpawnProjectile(position, direction, 0f);
        //Ask server to also fire passing in current Tick.
        ServerFire(position, direction, base.TimeManager.Tick);
    }

    /// <summary>
    /// Spawns a projectile locally.
    /// </summary>
    private void SpawnProjectile(Vector3 position, Vector3 direction, float passedTime)
    {
        Bullet pp = Instantiate(_bullet, position, Quaternion.identity);
        pp.Initialize(direction, passedTime);
    }

    /// <summary>
    /// Fires on the server.
    /// </summary>
    /// <param name="position">Position to spawn projectile.</param>
    /// <param name="direction">Direction to move projectile.</param>
    /// <param name="tick">Tick when projectile was spawned on client.</param>
    [ServerRpc]
    private void ServerFire(Vector3 position, Vector3 direction, uint tick)
    {
        /* You may want to validate position and direction here.
        * How this is done depends largely upon your game so it
        * won't be covered in this guide. */

        //Get passed time. Note the false for allow negative values.
        float passedTime = (float)base.TimeManager.TimePassed(tick, false);
        /* Cap passed time at half of constant value for the server.
        * In our example max passed time is 300ms, so server value
        * would be max 150ms. This means if it took a client longer
        * than 150ms to send the rpc to the server, the time would
        * be capped to 150ms. This might sound restrictive, but that would
        * mean the client would have roughly a 300ms ping; we do not want
        * to punish other players because a laggy client is firing. */
        passedTime = Mathf.Min(MAX_PASSED_TIME / 2f, passedTime);

        //Spawn on the server.
        SpawnProjectile(position, direction, passedTime);
        //Tell other clients to spawn the projectile.
        ObserversFire(position, direction, tick);
    }

    /// <summary>
    /// Fires on all clients but owner.
    /// </summary>
    [ObserversRpc(ExcludeOwner = true)]
    private void ObserversFire(Vector3 position, Vector3 direction, uint tick)
    {
        //Like on server get the time passed and cap it. Note the false for allow negative values.
        float passedTime = (float)base.TimeManager.TimePassed(tick, false);
        passedTime = Mathf.Min(MAX_PASSED_TIME, passedTime);

        //Spawn the projectile locally.
        SpawnProjectile(position, direction, passedTime);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
