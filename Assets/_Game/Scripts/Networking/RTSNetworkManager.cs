using Mirror;
using RTS.Configs;
using RTS.Logic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RTS.Networking
{
    public class RTSNetworkManager : NetworkManager
    {
        [Header(Headers.prefabs)]
        [SerializeField] private GameObject unitSpawnerPrefab = null;
        [SerializeField] private GameOverHandler gameOverHandlerPrefab = null;

        public static event Action ClientOnConnected;
        public static event Action ClientOnDisconnected;

        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);

            ClientOnConnected?.Invoke();
        }

        public override void OnClientDisconnect(NetworkConnection conn)
        {
            base.OnClientDisconnect(conn);

            ClientOnDisconnected.Invoke();
        }

        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            base.OnServerAddPlayer(conn);

            RTSPlayer player = conn.identity.GetComponent<RTSPlayer>();

            player.SetColor(new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f)));
        }

        public override void OnServerSceneChanged(string sceneName)
        {
            if (SceneManager.GetActiveScene().name.StartsWith("Scene_Map"))
            {
                GameOverHandler gameOverHandlerInstance = Instantiate(gameOverHandlerPrefab);

                NetworkServer.Spawn(gameOverHandlerInstance.gameObject);
            }
        }
    }
}