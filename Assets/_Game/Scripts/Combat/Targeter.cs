using Mirror;
using UnityEngine;

namespace RTS.Combat
{
    public class Targeter : NetworkBehaviour
    {
        [SerializeField] private Targetable target;

        #region Server

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

        #endregion

        #region Client

        #endregion

    }
}