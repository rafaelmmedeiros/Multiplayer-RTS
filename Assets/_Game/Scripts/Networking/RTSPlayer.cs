using Mirror;
using RTS.Buildings;
using RTS.Units;
using System.Collections.Generic;
using UnityEngine;

namespace RTS.Networking
{
    public class RTSPlayer : NetworkBehaviour
    {
        private List<Unit> playerUnits = new List<Unit>();
        private List<Building> playerBuildings = new List<Building>();

        public List<Unit> GetPlayerUnits()
        {
            return playerUnits;
        }

        public List<Building> GetPlayerBuildings()
        {
            return playerBuildings;
        }

        #region Server

        public override void OnStartServer()
        {
            Unit.ServerOnUnitSpawned += HandleServerUnitSpawned;
            Unit.ServerOnUnitDespawned += HandleServerUnitDespawned;

            Building.ServerOnBuildingSpawned += HandleServerBuildingSpawned;
            Building.ServerOnBuildingDespawned += HandleServerBuildingDespawned;
        }

        public override void OnStopServer()
        {
            Unit.ServerOnUnitSpawned -= HandleServerUnitSpawned;
            Unit.ServerOnUnitDespawned -= HandleServerUnitDespawned;

            Building.ServerOnBuildingSpawned -= HandleServerBuildingSpawned;
            Building.ServerOnBuildingDespawned -= HandleServerBuildingDespawned;

        }

        private void HandleServerUnitSpawned(Unit unit)
        {
            if (unit.connectionToClient.connectionId != connectionToClient.connectionId) return;

            playerUnits.Add(unit);
        }

        private void HandleServerUnitDespawned(Unit unit)
        {
            if (unit.connectionToClient.connectionId != connectionToClient.connectionId) return;

            playerUnits.Remove(unit);
        }

        private void HandleServerBuildingSpawned(Building building)
        {
            if (building.connectionToClient.connectionId != connectionToClient.connectionId) return;

            playerBuildings.Add(building);
        }

        private void HandleServerBuildingDespawned(Building building)
        {
            if (building.connectionToClient.connectionId != connectionToClient.connectionId) return;

            playerBuildings.Remove(building);
        }

        #endregion

        #region Client

        public override void OnStartAuthority()
        {
            if (NetworkServer.active) return;

            Unit.AuthorityOnUnitSpawned += AuthorityHandleUnitSpawned;
            Unit.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;
        }

        public override void OnStopClient()
        {
            if (!isClientOnly || !hasAuthority) return;

            Unit.AuthorityOnUnitSpawned -= AuthorityHandleUnitSpawned;
            Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;
        }

        private void AuthorityHandleUnitSpawned(Unit unit)
        {
            playerUnits.Add(unit);
        }

        private void AuthorityHandleUnitDespawned(Unit unit)
        {
            playerUnits.Remove(unit);
        }

        #endregion
    }
}