using Mirror;
using RTS.Combat;
using RTS.Configs;
using RTS.Logic;
using RTS.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS.Buildings
{
    public class MoneyGenerator : NetworkBehaviour
    {
        [Header(Headers.members)]
        [SerializeField] private Health health = null;

        [Header(Headers.parameters)]
        [SerializeField] private int resourcePerInterval = 10;
        [SerializeField] private float interval = 1f;

        private float timer;
        private RTSPlayer player;

        public override void OnStartServer()
        {
            timer = interval;
            player = connectionToClient.identity.GetComponent<RTSPlayer>();

            health.ServerOnDie += HandleServerOnDie;
            GameOverHandler.ServerOnGameOver += HandleServerOnGameOver;
        }

        public override void OnStopServer()
        {
            health.ServerOnDie -= HandleServerOnDie;
            GameOverHandler.ServerOnGameOver -= HandleServerOnGameOver;
        }

        [ServerCallback]
        private void Update()
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                player.SetMoney(player.GetMoney() + resourcePerInterval);
                timer = interval;
            }
        }

        private void HandleServerOnDie()
        {
            NetworkServer.Destroy(gameObject);
        }

        private void HandleServerOnGameOver()
        {
            enabled = false;
        }
    }
}