using Mirror;
using RTS.Combat;
using RTS.Configs;
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

        [SyncVar]
        private int queueUnits;
        [SyncVar]
        private float unitTimer;

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
        private void ServerHandleOnDie()
        {
            NetworkServer.Destroy(gameObject);
        }

        [Command]
        private void CmdSpawnUnit()
        {
            GameObject unitInstance = Instantiate(
                unitPrefab,
                unitSpawnPoint.position,
                unitSpawnPoint.rotation);

            NetworkServer.Spawn(unitInstance, connectionToClient);
        }

        #endregion

        #region Client

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;

            if (!hasAuthority) return;

            CmdSpawnUnit();
        }

        #endregion
    }

}