using Mirror;
using RTS.Combat;
using RTS.Configs;
using System;
using UnityEngine;

namespace RTS.Buildings
{
    public class UnitBase : NetworkBehaviour
    {
        [Header(Headers.members)]
        [SerializeField] private Health health = null;

        public static event Action<int> ServerOnPlayerDie;

        public static event Action<UnitBase> ServerOnUnitSpawned;
        public static event Action<UnitBase> ServerOnUnitDespawned;

        #region Server

        public override void OnStartServer()
        {
            health.ServerOnDie += HandleServerOnDie;

            ServerOnUnitSpawned?.Invoke(this);
        }

        public override void OnStopServer()
        {
            health.ServerOnDie -= HandleServerOnDie;

            ServerOnUnitDespawned?.Invoke(this);
        }

        [Server]
        private void HandleServerOnDie()
        {
            ServerOnPlayerDie?.Invoke(connectionToClient.connectionId);

            NetworkServer.Destroy(gameObject);
        }

        #endregion

        #region Client

        #endregion
    }
}