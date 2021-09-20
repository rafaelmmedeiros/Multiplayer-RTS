using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

namespace RTS.Movement
{
    public class Mover : NetworkBehaviour
    {
        [SerializeField] private NavMeshAgent navMeshAgent = null;

        private Camera mainCamera;

        #region Server

        [Command]
        private void CmdMove(Vector3 target)
        {
            if (!NavMesh.SamplePosition(target, out NavMeshHit hit, 1f, NavMesh.AllAreas)) return;

            navMeshAgent.SetDestination(hit.position);
        }

        #endregion

        #region Client

        public override void OnStartAuthority()
        {
            mainCamera = Camera.main;
        }

        [ClientCallback]
        private void Update()
        {
            if (!hasAuthority) return;

            if (!Mouse.current.rightButton.wasPressedThisFrame) return;

            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity)) return;

            CmdMove(hit.point);
        }

        #endregion
    }
}