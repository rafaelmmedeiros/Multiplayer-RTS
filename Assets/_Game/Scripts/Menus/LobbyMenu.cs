using Mirror;
using RTS.Networking;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RTS.Menus
{
    public class LobbyMenu : MonoBehaviour
    {
        [SerializeField] private GameObject lobbyUI = null;
        [SerializeField] private Button startGameButton = null;
        [SerializeField] private TMP_Text[] playerNameTexts = new TMP_Text[4];

        private void Start()
        {
            RTSNetworkManager.ClientOnConnected += HandleClientConnected;
            RTSPlayer.AuthorityOnPartyOwnerStateUpdated += HandleAuthorityOnPartyOwnerStateUpdated;
            RTSPlayer.ClientOnInfoUpdated += HandleClientInfoUpdated;
        }

        private void OnDestroy()
        {
            RTSNetworkManager.ClientOnConnected -= HandleClientConnected;
            RTSPlayer.AuthorityOnPartyOwnerStateUpdated -= HandleAuthorityOnPartyOwnerStateUpdated;
            RTSPlayer.ClientOnInfoUpdated -= HandleClientInfoUpdated;
        }

        private void HandleClientInfoUpdated()
        {
            List<RTSPlayer> players = ((RTSNetworkManager)NetworkManager.singleton).Players;

            for (int i = 0; i < players.Count; i++)
            {
                playerNameTexts[i].text = players[i].GetDisplayName();
            }

            for (int i = players.Count; i < playerNameTexts.Length; i++)
            {
                playerNameTexts[i].text = "Waiting For Player...";
            }

            startGameButton.interactable = players.Count >= 2;

        }

        private void HandleClientConnected()
        {
            lobbyUI.SetActive(true);
        }

        private void HandleAuthorityOnPartyOwnerStateUpdated(bool state)
        {
            startGameButton.gameObject.SetActive(state);
        }

        public void StartGame()
        {
            NetworkClient.connection.identity.GetComponent<RTSPlayer>().CmdStartGame();
        }

        public void LeaveLobby()
        {
            if (NetworkServer.active && NetworkClient.isConnected)
            {
                NetworkManager.singleton.StopHost();
            }
            else
            {
                NetworkManager.singleton.StopClient();

                SceneManager.LoadScene(0);
            }
        }

    }
}