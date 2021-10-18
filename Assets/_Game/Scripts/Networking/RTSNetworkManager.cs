using Mirror;
using RTS.Configs;
using RTS.Logic;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RTS.Networking
{
    public class RTSNetworkManager : NetworkManager
    {
        [Header(Headers.prefabs)]
        [SerializeField] private GameObject playerBasePrefab = null;
        [SerializeField] private GameOverHandler gameOverHandlerPrefab = null;

        private bool isGameInProgress = false;

        public List<RTSPlayer> Players { get; } = new List<RTSPlayer>();

        public static event Action ClientOnConnected;
        public static event Action ClientOnDisconnected;

        #region server

        public override void OnServerConnect(NetworkConnection conn)
        {
            if (!isGameInProgress) return;

            conn.Disconnect();
        }

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            RTSPlayer player = conn.identity.GetComponent<RTSPlayer>();

            Players.Remove(player);

            base.OnServerDisconnect(conn);
        }

        public override void OnStopServer()
        {
            Players.Clear();

            isGameInProgress = false;
        }

        public void StartGame()
        {
            if (Players.Count < 2) return;

            isGameInProgress = true;

            ServerChangeScene("Scene_Map_01");
        }

        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            base.OnServerAddPlayer(conn);

            RTSPlayer player = conn.identity.GetComponent<RTSPlayer>();

            Players.Add(player);

            player.SetColor(new Color(
                UnityEngine.Random.Range(0f, 1f),
                UnityEngine.Random.Range(0f, 1f),
                UnityEngine.Random.Range(0f, 1f)));

            player.SetIsPartyOwner(Players.Count == 1);
        }

        public override void OnServerSceneChanged(string sceneName)
        {
            if (SceneManager.GetActiveScene().name.StartsWith("Scene_Map"))
            {
                GameOverHandler gameOverHandlerInstance = Instantiate(gameOverHandlerPrefab);

                NetworkServer.Spawn(gameOverHandlerInstance.gameObject);

                foreach (RTSPlayer player in Players)
                {
                    GameObject playerBaseInstance = Instantiate(
                        playerBasePrefab,
                        GetStartPosition().position,
                        Quaternion.identity);

                    NetworkServer.Spawn(playerBaseInstance, player.connectionToClient);
                }
            }
        }

        #endregion


        #region client

        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);

            ClientOnConnected?.Invoke();
        }

        public override void OnClientDisconnect(NetworkConnection conn)
        {
            base.OnClientDisconnect(conn);

            ClientOnDisconnected?.Invoke();
        }

        public override void OnStopClient()
        {
            Players.Clear();
        }

        #endregion
    }
}