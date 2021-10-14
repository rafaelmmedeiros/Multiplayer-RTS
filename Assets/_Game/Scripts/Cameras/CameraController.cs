using Mirror;
using RTS.Configs;
using RTS.Inputs;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RTS.Cameras
{
    public class CameraController : NetworkBehaviour
    {
        [Header(Headers.members)]
        [SerializeField] private Transform playerCameraTransform = null;

        [Header(Headers.parameters)]
        [SerializeField] private float speed = 20f;
        [SerializeField] private float screenBorderThickness = 10f;
        [SerializeField] private Vector2 screenXLimits = Vector2.zero;
        [SerializeField] private Vector2 screenYLimits = Vector2.zero;

        private Vector2 previousInput;

        private Controls controls;

        public override void OnStartAuthority()
        {
            playerCameraTransform.gameObject.SetActive(true);

            controls = new Controls();

            controls.Player.MoveCamera.performed += SetPreviousInput;
            controls.Player.MoveCamera.canceled += SetPreviousInput;

            controls.Enable();
        }

        private void SetPreviousInput(InputAction.CallbackContext context)
        {
            previousInput = context.ReadValue<Vector2>();
        }
    }
}