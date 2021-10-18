using RTS.Buildings;
using RTS.Configs;
using RTS.Networking;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace RTS.GUI
{
    public class BuildingButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [Header(Headers.members)]
        [SerializeField] private Image iconImage = null;
        [SerializeField] private TMP_Text priceText = null;

        [Header(Headers.prefabs)]
        [SerializeField] private Building buidlingPrefab = null;

        [Header(Headers.parameters)]
        [SerializeField] private LayerMask floorMask = new LayerMask();

        private Camera mainCamera;
        private RTSPlayer player;

        private GameObject buildingPreviewInstance;
        private Renderer buildingRendererInstance;
        private BoxCollider buildingCollider;

        private void Start()
        {
            mainCamera = Camera.main;
            iconImage.sprite = buidlingPrefab.GetIcon();
            priceText.text = buidlingPrefab.GetPrice().ToString();

            player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
            buildingCollider = buidlingPrefab.GetComponent<BoxCollider>();
        }

        private void Update()
        {
            if (buildingPreviewInstance == null) return;

            UpdateBuildingPreview();
        }

        private void UpdateBuildingPreview()
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, floorMask)) { return; }

            buildingPreviewInstance.transform.position = hit.point;

            if (!buildingPreviewInstance.activeSelf)
            {
                buildingPreviewInstance.SetActive(true);
            }

            Color color = (player.CanPlaceBuilding(buildingCollider, hit.point)) ? Color.green : Color.red;

            buildingRendererInstance.material.SetColor("_BaseColor", color);
        }


        //  INTERFACES
        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;

            if (player.GetMoney() < buidlingPrefab.GetPrice()) return;

            buildingPreviewInstance = Instantiate(buidlingPrefab.GetBuildingPreview());
            buildingRendererInstance = buildingPreviewInstance.GetComponentInChildren<Renderer>();

            buildingPreviewInstance.SetActive(false);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (buildingPreviewInstance == null) return;

            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, floorMask))
            {
                player.CmdTryPlaceBuilding(buidlingPrefab.GetId(), hit.point);
            }

            Destroy(buildingPreviewInstance);
        }

    }
}