using Mirror;
using RTS.Networking;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RTS.Units
{
    public class UnitSelectionHandler : MonoBehaviour
    {
        [SerializeField] private RectTransform unitSelectedArea = null;
        [SerializeField] private LayerMask layerMask = new LayerMask();

        private RTSPlayer player;
        private Camera mainCamera;

        private Vector2 startPosition;

        public List<Unit> SelectedUnits { get; } = new List<Unit>();

        private void Start()
        {
            mainCamera = Camera.main;
        }

        private void Update()
        {
            if (player == null)
            {
                player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
            }

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                StartSelectionArea();
            }
            else if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                ClearSelectionArea();
            }
            else if (Mouse.current.leftButton.isPressed)
            {
                UpdateSelectionArea();
            }

        }

        private void StartSelectionArea()
        {
            if (!Keyboard.current.leftShiftKey.isPressed)
            {
                foreach (Unit selectedUnit in SelectedUnits)
                {
                    selectedUnit.Deselect();
                }

                SelectedUnits.Clear();
            }

            unitSelectedArea.gameObject.SetActive(true);

            startPosition = Mouse.current.position.ReadValue();

            UpdateSelectionArea();

        }

        private void ClearSelectionArea() // When clear the selected area, make selection
        {
            unitSelectedArea.gameObject.SetActive(false);

            if (unitSelectedArea.sizeDelta.magnitude == 0)
            {
                Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

                if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask)) return;

                if (!hit.collider.TryGetComponent<Unit>(out Unit unit)) return;

                if (!unit.hasAuthority) return;

                SelectedUnits.Add(unit);

                foreach (Unit selectedUnit in SelectedUnits)
                {
                    selectedUnit.Select();
                }

                return;
            }

            Vector2 min = unitSelectedArea.anchoredPosition - (unitSelectedArea.sizeDelta / 2);
            Vector2 max = unitSelectedArea.anchoredPosition + (unitSelectedArea.sizeDelta / 2);

            foreach (Unit unit in player.GetMyUnits())
            {
                if (SelectedUnits.Contains(unit)) continue;

                Vector3 screenPosition = mainCamera.WorldToScreenPoint(unit.transform.position);

                if (screenPosition.x > min.x &&
                    screenPosition.x < max.x &&
                    screenPosition.y > min.y &&
                    screenPosition.y < max.y)
                {
                    SelectedUnits.Add(unit);
                    unit.Select();
                }
            }


        }

        private void UpdateSelectionArea()
        {
            Vector2 mouseCurrentPostion = Mouse.current.position.ReadValue();

            float areaWidth = mouseCurrentPostion.x - startPosition.x;  
            float areaHeight = mouseCurrentPostion.y - startPosition.y;

            unitSelectedArea.sizeDelta = new Vector2(Mathf.Abs(areaWidth), Mathf.Abs(areaHeight));
            unitSelectedArea.anchoredPosition = startPosition + new Vector2(areaWidth / 2, areaHeight / 2);
        }

    }
}