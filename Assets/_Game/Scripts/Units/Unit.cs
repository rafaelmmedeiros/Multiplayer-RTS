using Mirror;
using RTS.Combat;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace RTS.Units
{
    public class Unit : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private Targeter targeter = null;
        [SerializeField] private UnitMovement unitMovement = null;

        [Header("Unity Events")]
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
            ServerOnUnitSpawned?.Invoke(this);
        }

        public override void OnStopServer()
        {
            ServerOnUnitDespawned?.Invoke(this);
        }

        #endregion

        #region Client

        public override void OnStartClient()
        {
            if (!isClientOnly || !hasAuthority) return;

            AuthorityOnUnitSpawned?.Invoke(this);
        }

        public override void OnStopClient()
        {
            if (!isClientOnly || !hasAuthority) return;

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