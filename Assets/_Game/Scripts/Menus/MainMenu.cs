using Mirror;
using RTS.Configs;
using System.Collections;
using System.Collections.Generic;
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