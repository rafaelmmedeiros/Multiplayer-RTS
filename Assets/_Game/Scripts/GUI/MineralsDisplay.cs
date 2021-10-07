using Mirror;
using RTS.Networking;
using System;
using TMPro;
using UnityEngine;

namespace RTS.GUI
{
    public class MineralsDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text mineralsText = null;

        private RTSPlayer player;

        private void Update()
        {
            if (player == null)
            {
                player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();

                if (player != null)
                {
                    HandleClientOnMineralsUpdated(player.GetMinerals());
                    player.ClientOnMineralsUpdated += HandleClientOnMineralsUpdated;
                }
            }
        }

        private void OnDestroy()
        {
            player.ClientOnMineralsUpdated -= HandleClientOnMineralsUpdated;
        }

        private void HandleClientOnMineralsUpdated(int minerals)
        {
            mineralsText.text = $"Minerals: {minerals}";
        }
    }
}