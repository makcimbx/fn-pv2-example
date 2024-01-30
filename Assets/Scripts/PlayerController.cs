using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

namespace Game.Views.GamePlay
{
    [RequireComponent(typeof(RigidbodySync))]
    public class PlayerController : NetworkBehaviour
    {
        private RigidbodySync rigidbodySync;

        private RigidbodySync RigidbodySync => rigidbodySync ??= GetComponent<RigidbodySync>();

        public void ForceInDirection(Vector3 direction)
        {
            if (!IsOwner) return;

            RigidbodySync.ForceInDirection(direction, ForceMode.Impulse);
        }
    }
}
