using Mirror;
using RTS.Networking;
using System;
using TMPro;
using UnityEngine;

namespace RTS.GUI
{
    public class MoneyDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text moneyText = null;

        private RTSPlayer player;

        private void Update()
        {
            if (player == null)
            {
                player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();

                if (player != null)
                {
                    HandleClientOnMoneyUpdated(player.GetMoney());
                    player.ClientOnMoneyUpdated += HandleClientOnMoneyUpdated;
                }
            }
        }

        private void OnDestroy()
        {
            player.ClientOnMoneyUpdated -= HandleClientOnMoneyUpdated;
        }

        private void HandleClientOnMoneyUpdated(int money)
        {
            moneyText.text = $"Money: {money}";
        }
    }
}