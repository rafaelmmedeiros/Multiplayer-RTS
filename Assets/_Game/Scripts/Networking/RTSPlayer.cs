using Mirror;
using RTS.Buildings;
using RTS.Configs;
using RTS.Units;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RTS.Networking
{
    public class RTSPlayer : NetworkBehaviour
    {
        [Header(Headers.members)]
        [SerializeField] private Building[] buildings = new Building[0];

        [Header(Headers.parameters)]
        [SerializeField] private LayerMask buildingBlockLayer = new LayerMask();
        [SerializeField] private float builgingRangeLimit = 10f;

        [SyncVar(hook = nameof(HandleClientMineralsUpdated))]
        private int money = 500;

        private List<Unit> playerUnits = new List<Unit>();
        private List<Building> playerBuildings = new List<Building>();

        public int GetMoney() => money;
        public List<Unit> GetPlayerUnits() => playerUnits;
        public List<Building> GetPlayerBuildings() => playerBuildings;

        [Server]
        public void SetMoney(int money) => this.money = money;

        public event Action<int> ClientOnMoneyUpdated;

        public bool CanPlaceBuilding(BoxCollider buildingCollider, Vector3 point)
        {
            if (Physics.CheckBox(
                point + buildingCollider.center,
                buildingCollider.size / 2,
                Quaternion.identity,
                buildingBlockLayer)) return false;

            foreach (Building building in buildings)
            {
                // .magnitude is more expensive
                if ((point - building.transform.position).sqrMagnitude <=
                    builgingRangeLimit * builgingRangeLimit)
                {
                    return true;
                }
            }

            return false;
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

        [Command]
        public void CmdTryPlaceBuilding(int buildingId, Vector3 point)
        {
            //TODO: Make methods here, a lot os responsability to the same method

            Building buildingToPlace = null;

            foreach (Building building in buildings)
            {
                if (building.GetId() == buildingId)
                {
                    buildingToPlace = building;
                    break;
                }
            }

            if (buildingToPlace == null) return;

            BoxCollider buildingCollider = buildingToPlace.GetComponent<BoxCollider>();

            if (!CanPlaceBuilding(buildingCollider, point)) return;

            GameObject buildingInstance = Instantiate(buildingToPlace.gameObject, point, buildingToPlace.transform.rotation);

            NetworkServer.Spawn(buildingInstance, connectionToClient);

            SetMoney(money - buildingToPlace.GetPrice());
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

            Building.AuthorityOnBuildingSpawned += AuthorityHandleBuildingSpawned;
            Building.AuthorityOnBuildingDespawned += AuthorityHandleBuildingDespawned;
        }

        public override void OnStopClient()
        {
            if (!isClientOnly || !hasAuthority) return;

            Unit.AuthorityOnUnitSpawned -= AuthorityHandleUnitSpawned;
            Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;

            Building.AuthorityOnBuildingSpawned -= AuthorityHandleBuildingSpawned;
            Building.AuthorityOnBuildingDespawned -= AuthorityHandleBuildingDespawned;
        }

        private void HandleClientMineralsUpdated(int oldResource, int newResource)
        {
            ClientOnMoneyUpdated?.Invoke(newResource);
        }

        private void AuthorityHandleUnitSpawned(Unit unit)
        {
            playerUnits.Add(unit);
        }

        private void AuthorityHandleUnitDespawned(Unit unit)
        {
            playerUnits.Remove(unit);
        }

        private void AuthorityHandleBuildingDespawned(Building building)
        {
            playerBuildings.Add(building);
        }

        private void AuthorityHandleBuildingSpawned(Building building)
        {
            playerBuildings.Remove(building);
        }

        #endregion
    }
}