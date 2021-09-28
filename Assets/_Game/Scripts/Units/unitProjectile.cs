using Mirror;
using RTS.Combat;
using RTS.Configs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS.Units
{
    public class unitProjectile : NetworkBehaviour
    {
        [Header(Headers.members)]
        [SerializeField] private Rigidbody rigidBody = null;

        [Header(Headers.parameters)]
        [SerializeField] private float destroyAfterSeconds = 5f;
        [SerializeField] private float launchForce = 10f;
        [SerializeField] private int damage = 20;

        private void Start()
        {
            rigidBody.velocity = transform.forward * launchForce;
        }

        #region Server

        public override void OnStartServer()
        {
            Invoke(nameof(DestroySelf), destroyAfterSeconds);
        }

        [ServerCallback]
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<NetworkIdentity>(out NetworkIdentity networkIdentity))
            {
                if (networkIdentity.connectionToClient == connectionToClient) return;
            }

            if (other.TryGetComponent<Health>(out Health health))
            {
                health.DealDamage(damage);
            }

            DestroySelf();
        }

        [Server]
        private void DestroySelf()
        {
            NetworkServer.Destroy(gameObject);
        }

        #endregion
    }
}
