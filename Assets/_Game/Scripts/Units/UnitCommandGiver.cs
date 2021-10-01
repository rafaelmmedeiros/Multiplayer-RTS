using RTS.Combat;
using RTS.Logic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RTS.Units
{
    public class UnitCommandGiver : MonoBehaviour
    {
        [SerializeField] private UnitSelectionHandler unitSelectionHandler = null;
        [SerializeField] private LayerMask layerMask = new LayerMask();

        private Camera mainCamera;

        private void Start()
        {
            mainCamera = Camera.main;

            GameOverHandler.ClientOnGameOver += HandleClientOnGameOver;
        }

        private void OnDestroy()
        {
            GameOverHandler.ClientOnGameOver -= HandleClientOnGameOver;
        }

        private void Update()
        {
            if (!Mouse.current.rightButton.wasPressedThisFrame) return;

            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask)) return;

            if (hit.collider.TryGetComponent<Targetable>(out Targetable target))
            {
                if (target.hasAuthority)
                {
                    TryMove(hit.point);
                    return;
                }

                TryTarget(target);
                return;
            }

            TryMove(hit.point);
        }

        private void HandleClientOnGameOver(string obj)
        {
            enabled = false;
        }

        private void TryMove(Vector3 point)
        {
            foreach (Unit unit in unitSelectionHandler.SelectedUnits)
            {
                unit.GetUnitMovement().CmdMove(point);
            }
        }

        private void TryTarget(Targetable target)
        {
            foreach (Unit unit in unitSelectionHandler.SelectedUnits)
            {
                unit.GetTargeter().CmdSetTarget(target.gameObject);
            }
        }
    }
}