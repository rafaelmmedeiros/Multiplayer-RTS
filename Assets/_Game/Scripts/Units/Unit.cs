using Mirror;
using RTS.Combat;
using RTS.Configs;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace RTS.Units
{
    public class Unit : NetworkBehaviour
    {
        [Header(Headers.members)]
        [SerializeField] private Targeter targeter = null;
        [SerializeField] private UnitMovement unitMovement = null;
        [SerializeField] private Health health = null;

        [Header(Headers.unityEvents)]
        [SerializeField] private UnityEvent onSelected = null;
        [SerializeField] private UnityEvent onDeselected = null;

        public static event Action<Unit> ServerOnUnitSpawned;
        public static event Action<Unit> ServerOnUnitDespawned;

        public static event Action<Unit> AuthorityOnUnitSpawned;
        public static event Action<Unit> AuthorityOnUnitDespawned;


        public UnitMovement GetUnitMovement()
        {
            return unitMovement;
        }

        public Targeter GetTargeter()
        {
            return targeter;
        }

        #region Server

        public override void OnStartServer()
        {
            health.ServerOnDie += ServerHandleOnDie;
            ServerOnUnitSpawned?.Invoke(this);
        }

        public override void OnStopServer()
        {
            health.ServerOnDie -= ServerHandleOnDie;
            ServerOnUnitDespawned?.Invoke(this);
        }

        [Server]
        private void ServerHandleOnDie()
        {
            NetworkServer.Destroy(gameObject);
        }

        #endregion

        #region Client

        public override void OnStartAuthority()
        {
            AuthorityOnUnitSpawned?.Invoke(this);
        }

        public override void OnStopClient()
        {
            if (!hasAuthority) return;

            AuthorityOnUnitDespawned?.Invoke(this);
        }

        [Client]
        public void Select()
        {
            if (!hasAuthority) return;

            onSelected?.Invoke();
        }

        [Client]
        public void Deselect()
        {
            if (!hasAuthority) return;

            onDeselected?.Invoke();
        }

        #endregion
    }
}