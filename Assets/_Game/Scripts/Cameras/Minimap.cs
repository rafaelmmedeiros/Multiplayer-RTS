using Mirror;
using RTS.Networking;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace RTS.Minimap
{
    public class Minimap : MonoBehaviour, IPointerDownHandler, IDragHandler
    {
        [SerializeField] private RectTransform minimapRect = null;
        [SerializeField] private float mapScale = 10f;
        [SerializeField] private float offset = -6f;

        private Transform playerCameraTransform;

        private void Update()
        {
            if (playerCameraTransform != null) return;

            if (NetworkClient.connection.identity == null) return;

            playerCameraTransform = NetworkClient.connection.identity
                .GetComponent<RTSPlayer>().GetCameraTransform();
        }

        private void MoveCamera()
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                minimapRect,
                mousePosition,
                null,
                out Vector2 localPoint)) return;

            //  Lerp is interpolation in english language
            Vector2 lerp = new Vector2(
                (localPoint.x - minimapRect.rect.x) / minimapRect.rect.width,
                (localPoint.y - minimapRect.rect.y) / minimapRect.rect.height);


            Vector3 cameraPositionToMove = new Vector3(
                Mathf.Lerp(-mapScale, mapScale, lerp.x),
                playerCameraTransform.position.y,
                Mathf.Lerp(-mapScale, mapScale, lerp.y));

            playerCameraTransform.position = cameraPositionToMove + new Vector3(0f, 0f, offset);

        }

        //  INTERFACES
        public void OnPointerDown(PointerEventData eventData)
        {
            MoveCamera();
        }

        public void OnDrag(PointerEventData eventData)
        {
            MoveCamera();
        }

    }

}