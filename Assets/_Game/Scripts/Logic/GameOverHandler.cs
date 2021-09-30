using Mirror;
using RTS.Buildings;
using System.Collections.Generic;
using UnityEngine;

namespace RTS.Logic
{
    public class GameOverHandler : NetworkBehaviour
    {
        private List<UnitBase> bases = new List<UnitBase>();

        #region Server

        public override void OnStartServer()
        {
            UnitBase.ServerOnUnitSpawned += HandleServerBaseSpawned;
            UnitBase.ServerOnUnitDespawned += HandleServerBaseDespawned;
        }

        public override void OnStopServer()
        {
            UnitBase.ServerOnUnitSpawned -= HandleServerBaseSpawned;
            UnitBase.ServerOnUnitDespawned -= HandleServerBaseDespawned;
        }

        [Server]
        private void HandleServerBaseSpawned(UnitBase unitbase)
        {
            bases.Add(unitbase);
        }

        [Server]
        private void HandleServerBaseDespawned(UnitBase unitbase)
        {
            bases.Remove(unitbase);

            if (bases.Count != 1) return;

            Debug.Log("Game over!");
        }

        #endregion

        #region Client

        #endregion
    }
}