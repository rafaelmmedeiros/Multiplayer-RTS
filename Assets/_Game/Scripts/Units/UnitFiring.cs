using Mirror;
using RTS.Combat;
using RTS.Configs;
using UnityEngine;

namespace RTS.Units
{
    public class UnitFiring : NetworkBehaviour
    {
        [Header(Headers.scripts)]
        [SerializeField] private Targeter targeter = null;

        [Header(Headers.preFabs)]
        [SerializeField] private GameObject projectilePrefab = null;

        [Header(Headers.others)]
        [SerializeField] private Transform projectileSpawnPoint = null;

        [Header(Headers.parameters)]
        [SerializeField] private float fireRange = 5f;
        [SerializeField] private float fireRate = 1f;
        [SerializeField] private float rotationSpeed = 20f;

        private float lastFireTimer;

        [ServerCallback]
        private void Update()
        {
            Targetable target = targeter.GetTarget();

            if (target == null) { return; }

            if (!CanFireAtTarget()) { return; }

            Quaternion targetRotation =
                Quaternion.LookRotation(target.transform.position - transform.position);

            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            if (Time.time > (1 / fireRate) + lastFireTimer)
            {
                Quaternion projectileRotation = Quaternion.LookRotation(
                    target.GetAimAtPoint().position - projectileSpawnPoint.position);

                GameObject projectileInstance = Instantiate(
                    projectilePrefab, projectileSpawnPoint.position, projectileRotation);

                NetworkServer.Spawn(projectileInstance, connectionToClient);

                lastFireTimer = Time.time;
            }
        }

        [Server]
        private bool CanFireAtTarget()
        {
            return (targeter.GetTarget().transform.position - transform.position).sqrMagnitude
                <= fireRange * fireRange;
        }

    }
}