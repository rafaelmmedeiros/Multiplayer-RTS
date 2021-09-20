using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RTS.Buildings
{
    public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
    {
        [SerializeField] private GameObject unitPrefab = null;
        [SerializeField] private Transform unitSpawnPoint = null;

        #region Server

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

        // THE GREAT DIVIDE

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