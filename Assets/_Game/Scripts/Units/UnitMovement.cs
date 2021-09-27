using Mirror;
using RTS.Combat;
using UnityEngine;
using UnityEngine.AI;

namespace RTS.Units
{
    public class UnitMovement : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private NavMeshAgent agent = null;
        [SerializeField] private Targeter targeter = null;

        [Header("Parameters")]
        [SerializeField] private float chaseRange = 10f;

        #region Server

        [ServerCallback]
        private void Update()
        {
            Targetable target = targeter.GetTarget();
            if (target != null)
            {
                // Performance purposes Vectro3.Distance() is too expansive
                if ((target.transform.position - transform.position).sqrMagnitude > chaseRange * chaseRange)
                {
                    agent.SetDestination(target.transform.position);
                }
                else if (agent.hasPath)
                {
                    agent.ResetPath();
                }

                return;
            }

            if (!agent.hasPath) return;

            if (agent.remainingDistance > agent.stoppingDistance) return;

            agent.ResetPath();
        }

        [Command]
        public void CmdMove(Vector3 position)
        {
            if (targeter != null)
            {
                targeter.ClearTarget();
            }

            if (!NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, NavMesh.AllAreas)) return;

            agent.SetDestination(hit.position);
        }

        #endregion
    }
}