using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Mirror;
using RTS.Networking;

namespace RTS.Menus
{
    public class JoinLobbyMenu : MonoBehaviour
    {
        [SerializeField] private GameObject landingPagePanel = null;
        [SerializeField] private TMP_InputField adressInput = null;
        [SerializeField] private Button joinButton = null;

        private void OnEnable()
        {
            RTSNetworkManager.ClientOnConnected += HandleClientOnConnected;
            RTSNetworkManager.ClientOnDisconnected += HandleClientOnDisconnected;
        }

        private void OnDisable()
        {
            RTSNetworkManager.ClientOnConnected -= HandleClientOnConnected;
            RTSNetworkManager.ClientOnDisconnected -= HandleClientOnDisconnected;
        }

        public void Join()
        {
            string address = adressInput.text;

            NetworkManager.singleton.networkAddress = address;
            NetworkManager.singleton.StartClient();

            joinButton.interactable = false;
        }

        private void HandleClientOnConnected()
        {
            joinButton.interactable = true;

            gameObject.SetActive(false);
            landingPagePanel.SetActive(false);
        }

        private void HandleClientOnDisconnected()
        {
            joinButton.interactable = true;
        }
    }
}