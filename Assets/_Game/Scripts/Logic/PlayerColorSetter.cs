using Mirror;
using RTS.Networking;
using UnityEngine;

namespace RTS.Logic
{
    public class PlayerColorSetter : NetworkBehaviour
    {
        [SerializeField] private Renderer[] colorRenderers = new Renderer[0];
        [SerializeField] private SpriteRenderer[] colorSpriteRenderes = new SpriteRenderer[0];

        [SyncVar(hook = nameof(HandleClientColorUpdated))]
        private Color playerColor = new Color();

        #region Server

        public override void OnStartServer()
        {
            RTSPlayer player = connectionToClient.identity.GetComponent<RTSPlayer>();

            playerColor = player.GetColor();
        }

        #endregion

        #region Client

        private void HandleClientColorUpdated(Color oldColor, Color newColor)
        {
            foreach (Renderer renderer in colorRenderers)
            {
                renderer.material.SetColor("_BaseColor", newColor);
            }

            foreach (SpriteRenderer renderer in colorSpriteRenderes)
            {
                renderer.color = newColor;
            }
        }

        #endregion
    }
}