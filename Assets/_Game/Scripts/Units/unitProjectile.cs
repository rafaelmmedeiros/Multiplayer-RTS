using Mirror;
using RTS.Configs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS.Units
{
    public class unitProjectile : NetworkBehaviour
    {
        [Header(Headers.members)]
        [SerializeField] private Rigidbody rigidbodyRB = null;

        [Header(Headers.parameters)]
        [SerializeField] private float destroyAfterSeconds = 5f;
        [SerializeField] private float launchForce = 10f;

        private void Start()
        {
            rigidbodyRB.velocity = transform.forward * launchForce;
        }

        public override void OnStartServer()
        {
            Invoke(nameof(DestroySelf), destroyAfterSeconds);
        }

        [Server]
        private void DestroySelf()
        {
            NetworkServer.Destroy(gameObject);
        }
    }
}
