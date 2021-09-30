using RTS.Logic;
using TMPro;
using UnityEngine;
using Mirror;

namespace RTS.GUI
{
    public class GameOverDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text winnerNameText = null;
        [SerializeField] private GameObject gameOverDisplayParent = null;

        private void Start()
        {
            GameOverHandler.ClientOnGameOver += HandleClientOnGameOver;
        }

        private void OnDestroy()
        {
            GameOverHandler.ClientOnGameOver -= HandleClientOnGameOver;
        }

        public void LeaveGame()
        {
            if (NetworkServer.active && NetworkClient.isConnected)
            {
                NetworkManager.singleton.StopHost();
            }
            else
            {
                NetworkManager.singleton.StopClient();
            }
        }

        private void HandleClientOnGameOver(string winner)
        {
            winnerNameText.text = winner;

            gameOverDisplayParent.SetActive(true);
        }
    }
}