using Mirror;
using UnityEngine;
using UnityEngine.AI;

namespace RTS.Units
{
    public class UnitMovement : NetworkBehaviour
    {
        [SerializeField] private NavMeshAgent navMeshAgent = null;

        #region Server

        [Command]
        public void CmdMove(Vector3 target)
        {
            if (!NavMesh.SamplePosition(target, out NavMeshHit hit, 1f, NavMesh.AllAreas)) return;

            navMeshAgent.SetDestination(hit.position);
        }

        #endregion

    }
}