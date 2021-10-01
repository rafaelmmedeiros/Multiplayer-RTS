using Mirror;
using UnityEngine;

namespace RTS.Combat
{
    public class Targetable : NetworkBehaviour
    {
        [SerializeField] private Transform aimAtPoint = null;

        public Transform GetAimAtPoint() => aimAtPoint;
    }
}