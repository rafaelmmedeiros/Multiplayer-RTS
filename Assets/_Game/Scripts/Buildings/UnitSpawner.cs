using Mirror;
using RTS.Combat;
using RTS.Configs;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RTS.Buildings
{
    public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
    {
        [Header(Headers.members)]
        [SerializeField] private Health health = null;
        [SerializeField] private Transform unitSpawnPoint = null;

        [Header(Headers.prefabs)]
        [SerializeField] private GameObject unitPrefab = null;

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
            //NetworkServer.Destroy(gameObject);
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