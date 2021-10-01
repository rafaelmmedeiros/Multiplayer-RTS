using Mirror;
using RTS.Logic;
using System;
using UnityEngine;

namespace RTS.Combat
{
    public class Targeter : NetworkBehaviour
    {
        private Targetable target;

        public Targetable GetTarget()
        {
            return target;
        }

        #region Server

        public override void OnStartServer()
        {
            GameOverHandler.ServerOnGameOver += HandleServerOnGameOver;
        }

        public override void OnStopServer()
        {
            GameOverHandler.ServerOnGameOver += HandleServerOnGameOver;
        }

        [Command]
        public void CmdSetTarget(GameObject targetGameObject)
        {
            if (!targetGameObject.TryGetComponent<Targetable>(out Targetable newTarget)) return;

            target = newTarget;
        }

        [Server]
        public void ClearTarget()
        {
            target = null;
        }

        [Server]
        private void HandleServerOnGameOver()
        {
            ClearTarget();
        }

        #endregion
    }
}