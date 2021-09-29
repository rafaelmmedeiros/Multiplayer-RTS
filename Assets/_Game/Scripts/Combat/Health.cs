using Mirror;
using RTS.Configs;
using System;
using UnityEngine;

namespace RTS.Combat
{
    public class Health : NetworkBehaviour
    {
        [Header(Headers.parameters)]
        [SerializeField] private int maxHealth = 100;

        [SyncVar(hook = nameof(HandleHealthUpdated))]
        private int currentHealth;

        public event Action ServerOnDie;
        public event Action<int, int> ClientOnHealthUpdated;

        #region Server

        public override void OnStartServer()
        {
            currentHealth = maxHealth;
        }

        [Server]
        public void DealDamage(int damageAmount)
        {
            if (currentHealth == 0) return;

            currentHealth = Mathf.Max(0, currentHealth - damageAmount);

            if (currentHealth != 0) return;

            ServerOnDie?.Invoke(); // ? Stop errors if someone is not lintening

            Debug.Log("Die");
        }

        #endregion

        #region Cliente

        private void HandleHealthUpdated(int oldHealth, int newHealth)
        {
            ClientOnHealthUpdated?.Invoke(newHealth, maxHealth);
        }

        #endregion
    }
}