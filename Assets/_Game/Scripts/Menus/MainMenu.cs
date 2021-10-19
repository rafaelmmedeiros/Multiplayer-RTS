using Mirror;
using RTS.Configs;
using UnityEngine;

namespace RTS.Menus
{
    public class MainMenu : MonoBehaviour
    {
        [Header(Headers.members)]
        [SerializeField] private GameObject landingPagePanel = null;

        public void Host()
        {
            landingPagePanel.SetActive(false);

            NetworkManager.singleton.StartHost();
        }
    }
}