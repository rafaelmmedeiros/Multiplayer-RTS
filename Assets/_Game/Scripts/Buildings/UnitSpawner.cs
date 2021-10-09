using Mirror;
using RTS.Combat;
using RTS.Configs;
using RTS.Networking;
using RTS.Units;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RTS.Buildings
{
    public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
    {
        [Header(Headers.members)]
        [SerializeField] private Health health = null;
        [SerializeField] private Transform unitSpawnPoint = null;
        [SerializeField] private TMP_Text unitsToSpawnText = null;
        [SerializeField] private Image timerImage = null;

        [Header(Headers.parameters)]
        [SerializeField] private int maxUnitQueue = 5;
        [SerializeField] private float spawnMoveRange = 7f;
        [SerializeField] private float timeToSpawnUnit = 5f;

        [Header(Headers.prefabs)]
        [SerializeField] private Unit unitPrefab = null;

        [SyncVar(hook = nameof(HandleClientUnitQueueUnitUpdated))]
        private int queueUnits;
        [SyncVar]
        private float unitTimer;

        private float progressImageVelocity;

        private void Start()
        {
            timerImage.fillAmount = 0;
            unitsToSpawnText.text = "0";
            unitTimer = 0;
            queueUnits = 0;
        }

        private void Update()
        {
            if (isServer)
            {
                ProduceUmits();
            }

            if (isClient)
            {
                UpdateTimerDisplay();
            }
        }

        #region Server

        public override void OnStartServer()
        {
            health.ServerOnDie += ServerHandleOnDie;
        }

        public override void OnStopServer()
        {
            health.ServerOnDie -= ServerHandleOnDie;
        }

        [Server]
        private void ProduceUmits()
        {
            if (queueUnits == 0) return;

            unitTimer += Time.deltaTime;

            if (unitTimer < timeToSpawnUnit) return;

            GameObject unitInstance = Instantiate(
                unitPrefab.gameObject,
                unitSpawnPoint.position,
                unitSpawnPoint.rotation);

            NetworkServer.Spawn(unitInstance, connectionToClient);

            Vector3 spawnOffset = Random.insideUnitSphere * spawnMoveRange;
            spawnOffset.y = unitSpawnPoint.position.y;

            UnitMovement unitMovement = unitInstance.GetComponent<UnitMovement>();
            unitMovement.ServerMove(unitSpawnPoint.position + spawnOffset);

            queueUnits--;
            unitTimer = 0f;
        }

        [Server]
        private void ServerHandleOnDie()
        {
            NetworkServer.Destroy(gameObject);
        }

        [Command]
        private void CmdSpawnUnit()
        {
            if (queueUnits == maxUnitQueue) return;

            RTSPlayer player = connectionToClient.identity.GetComponent<RTSPlayer>();

            if (player.GetMoney() < unitPrefab.GetMoneyCost()) return;

            queueUnits++;

            player.SetMoney(player.GetMoney() - unitPrefab.GetMoneyCost());
        }

        #endregion

        #region Client

        private void UpdateTimerDisplay()
        {
            float progress = unitTimer / timeToSpawnUnit;

            if (progress < timerImage.fillAmount)
            {
                timerImage.fillAmount = progress;
            }
            else
            {
                timerImage.fillAmount = Mathf.SmoothDamp(
                    timerImage.fillAmount,
                    progress,
                    ref progressImageVelocity,
                    0.1f);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;

            if (!hasAuthority) return;

            CmdSpawnUnit();
        }

        public void HandleClientUnitQueueUnitUpdated(int oldQueueUnits, int newQueueUnits)
        {
            unitsToSpawnText.text = newQueueUnits.ToString();
        }

        #endregion
    }

}