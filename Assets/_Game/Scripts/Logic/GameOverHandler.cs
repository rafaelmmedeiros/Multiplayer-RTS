using Mirror;
using RTS.Buildings;
using RTS.Networking;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RTS.Logic
{
    public class GameOverHandler : NetworkBehaviour
    {
        public static event Action<string> ClientOnGameOver;

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

            int winnerPlayerId = bases[0].connectionToClient.connectionId;

            RpcGameOVer($"Player: {winnerPlayerId} has won!");
        }

        #endregion

        #region Client

        [ClientRpc]
        private void RpcGameOVer(string winner)
        {
            ClientOnGameOver?.Invoke(winner);
        }

        #endregion
    }
}