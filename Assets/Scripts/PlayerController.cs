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

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100))
                {
                    var vector = hit.point - transform.position;
                    var normalizedVector = vector.normalized;
                    ForceInDirection(normalizedVector * 5f);
                }
            }
        }
    }
}
