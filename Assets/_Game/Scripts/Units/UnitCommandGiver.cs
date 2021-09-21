using UnityEngine;
using UnityEngine.InputSystem;

namespace RTS.Units
{
    public class UnitCommandGiver : MonoBehaviour
    {
        [SerializeField] private UnitSelectorHandler unitSelector = null;
        [SerializeField] private LayerMask layerMask = new LayerMask();

        private Camera mainCamera;

        private void Start()
        {
            mainCamera = Camera.main;
        }

        private void Update()
        {
            if (!Mouse.current.rightButton.wasPressedThisFrame) return;

            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask)) return;

            TryMove(hit.point);
        }

        private void TryMove(Vector3 point)
        {
            foreach (Unit unit in unitSelector.SelectedUnits)
            {
                unit.GetUnitMovement().CmdMove(point);
            }
        }
    }
}