using Mirror;
using RTS.Configs;
using UnityEngine;

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
    }
}