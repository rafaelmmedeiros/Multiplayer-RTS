using Mirror;
using RTS.Configs;
using System;
using UnityEngine;

namespace RTS.Buildings
{
    public class Building : NetworkBehaviour
    {
        [SerializeField] private int id = -1;

        [Header(Headers.members)]
        [SerializeField] private Sprite icon = null;

        [Header(Headers.parameters)]
        [SerializeField] private int price = 100;

        public static event Action<Building> ServerOnBuildingSpawned;
        public static event Action<Building> ServerOnBuildingDespawned;

        public int GetId()
        {
            return id;
        }

        public Sprite GetIcon()
        {
            return icon;
        }

        public int GetPrice()
        {
            return price;
        }

        #region Server

        public override void OnStartServer()
        {
            ServerOnBuildingSpawned?.Invoke(this);
        }

        public override void OnStopServer()
        {
            ServerOnBuildingDespawned?.Invoke(this);
        }

        #endregion

        #region Client
        #endregion
    }
}