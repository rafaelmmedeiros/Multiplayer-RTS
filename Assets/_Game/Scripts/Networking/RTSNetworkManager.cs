using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS.Networking
{
    public class RTSNetworkManager : NetworkManager
    {
        [SerializeField] private GameObject unitSpawnerPrefab = null;

        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            base.OnServerAddPlayer(conn);

            GameObject unitSpawnerInstance = Instantiate(
                unitSpawnerPrefab, 
                conn.identity.transform.position, 
                conn.identity.transform.rotation);

            NetworkServer.Spawn(unitSpawnerInstance, conn);
        }
    }
}