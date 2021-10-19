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
        [SerializeField] private Transform cameraTransform = null;

        [Header(Headers.parameters)]
        [SerializeField] private LayerMask buildingBlockLayer = new LayerMask();
        [SerializeField] private float builgingRangeLimit = 5f;

        [SyncVar(hook = nameof(HandleClientMineralsUpdated))]
        private int money = 500;
        [SyncVar(hook = nameof(HandleAuthorityPartyOwnerStateUpdated))]
        private bool isPartyOwner = false;
        [SyncVar(hook = nameof(HandleClientDisplayNameUpdated))]
        private string displayName;

        private Color playerColor = new Color();

        private List<Unit> playerUnits = new List<Unit>();
        private List<Building> playerBuildings = new List<Building>();

        public event Action<int> ClientOnMoneyUpdated;
        public static event Action<bool> AuthorityOnPartyOwnerStateUpdated;
        public static event Action ClientOnInfoUpdated;


        public Transform GetCameraTransform() => cameraTransform;
        public int GetMoney() => money;
        public bool GetIsPartyOwner => isPartyOwner;
        public Color GetColor() => playerColor;
        public List<Unit> GetPlayerUnits() => playerUnits;
        public List<Building> GetPlayerBuildings() => playerBuildings;
        public string GetDisplayName() => displayName;

        [Server]
        public void SetMoney(int money) => this.money = money;
        [Server]
        public void SetIsPartyOwner(bool isPartyOwner) => this.isPartyOwner = isPartyOwner;
        [Server]
        public void SetColor(Color playerColor) => this.playerColor = playerColor;
        [Server]
        public void SetDisplayName(string displayName) => this.displayName = displayName;

        public bool CanPlaceBuilding(BoxCollider buildingCollider, Vector3 point)
        {
            //  TODO: Problem is possivel on terrain and building Z AXIS
            return true;
            //if (Physics.CheckBox(
            //    point + buildingCollider.center,
            //    buildingCollider.size / 2,
            //    Quaternion.identity,
            //    buildingBlockLayer)) return false;

            //foreach (Building building in buildings)
            //{
            //    // .magnitude is more expensive
            //    if ((point - building.transform.position).sqrMagnitude <=
            //        builgingRangeLimit * builgingRangeLimit)
            //    {
            //        return true;
            //    }
            //}
            //return false;
        }

        #region Server

        public override void OnStartServer()
        {
            Unit.ServerOnUnitSpawned += HandleServerUnitSpawned;
            Unit.ServerOnUnitDespawned += HandleServerUnitDespawned;

            Building.ServerOnBuildingSpawned += HandleServerBuildingSpawned;
            Building.ServerOnBuildingDespawned += HandleServerBuildingDespawned;

            DontDestroyOnLoad(gameObject);
        }

        public override void OnStopServer()
        {
            Unit.ServerOnUnitSpawned -= HandleServerUnitSpawned;
            Unit.ServerOnUnitDespawned -= HandleServerUnitDespawned;

            Building.ServerOnBuildingSpawned -= HandleServerBuildingSpawned;
            Building.ServerOnBuildingDespawned -= HandleServerBuildingDespawned;
        }

        [Command]
        public void CmdStartGame()
        {
            if (!isPartyOwner) return;

            ((RTSNetworkManager)NetworkManager.singleton).StartGame();
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

        public override void OnStartClient()
        {
            if (NetworkServer.active) return;

            DontDestroyOnLoad(gameObject);

            ((RTSNetworkManager)NetworkManager.singleton).Players.Add(this);
        }

        public override void OnStopClient()
        {
            ClientOnInfoUpdated?.Invoke();

            if (!isClientOnly) return;

            ((RTSNetworkManager)NetworkManager.singleton).Players.Remove(this);

            if (!hasAuthority) return;

            Unit.AuthorityOnUnitSpawned -= AuthorityHandleUnitSpawned;
            Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;

            Building.AuthorityOnBuildingSpawned -= AuthorityHandleBuildingSpawned;
            Building.AuthorityOnBuildingDespawned -= AuthorityHandleBuildingDespawned;
        }

        private void HandleClientMineralsUpdated(int oldResource, int newResource)
        {
            ClientOnMoneyUpdated?.Invoke(newResource);
        }

        private void HandleAuthorityPartyOwnerStateUpdated(bool oldState, bool newState)
        {
            if (!hasAuthority) return;

            AuthorityOnPartyOwnerStateUpdated?.Invoke(newState);
        }

        private void HandleClientDisplayNameUpdated(string oldDisplayName, string newDisplayName)
        {
            ClientOnInfoUpdated?.Invoke();
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