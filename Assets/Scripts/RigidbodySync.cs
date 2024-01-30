using System;
using FishNet.Object;
using FishNet.Object.Prediction;
using FishNet.Transporting;
using UnityEngine;

namespace Game.Views.GamePlay
{
    public class RigidbodySync : NetworkBehaviour
    {
        public struct MoveData : IReplicateData
        {
            private uint _tick;

            public bool addForce;
            public Vector3 force;
            public ForceMode mode;

            public MoveData(bool addForce, Vector3 force, ForceMode mode)
            {
                this.addForce = addForce;
                this.force = force;
                this.mode = mode;

                _tick = 0;
            }

            public void Dispose()
            {
            }

            public uint GetTick() => _tick;
            public void SetTick(uint value) => _tick = value;
        }

        public struct ReconcileData : IReconcileData
        {
            public Vector3 Position;
            public Quaternion Rotation;
            public Vector3 Velocity;
            public Vector3 AngularVelocity;

            public ReconcileData(
                Vector3 position,
                Quaternion rotation,
                Vector3 velocity,
                Vector3 angularVelocity)
            {
                Position = position;
                Rotation = rotation;
                Velocity = velocity;
                AngularVelocity = angularVelocity;

                _tick = 0;
            }

            private uint _tick;

            public void Dispose()
            {
            }

            public uint GetTick() => _tick;
            public void SetTick(uint value) => _tick = value;
        }

        private bool addForce;
        private ForceMode forceMode;
        private Vector3 force;

        private Rigidbody _rigidbody;

        public override void OnStartNetwork()
        {
            base.OnStartNetwork();
            _rigidbody = GetComponent<Rigidbody>();
            base.TimeManager.OnTick += TimeManager_OnTick;
            base.TimeManager.OnPostTick += TimeManager_OnPostTick;
        }

        public override void OnStopNetwork()
        {
            base.OnStopNetwork();
            base.TimeManager.OnTick -= TimeManager_OnTick;
            base.TimeManager.OnPostTick -= TimeManager_OnPostTick;
        }

        public void ForceInDirection(Vector3 direction, ForceMode mode)
        {
            addForce = true;
            forceMode = mode;
            force = direction;
        }

        private void TimeManager_OnTick()
        {
            Move(BuildMoveData());
        }

        private MoveData BuildMoveData()
        {
            if (!base.IsOwner)
                return default;

            MoveData md = new MoveData(addForce, force, forceMode);

            addForce = false;
            force = Vector3.zero;

            return md;
        }

        private void TimeManager_OnPostTick()
        {
            //Send reconcile per usual.
            if (IsServer)
            {
                var rd = new ReconcileData(
                    transform.position,
                    transform.rotation,
                    _rigidbody.velocity,
                    _rigidbody.angularVelocity);

                Reconciliation(rd);
            }
        }


        [Replicate]
        private void Move(
            MoveData md,
            ReplicateState state = ReplicateState.Invalid,
            Channel channel = Channel.Unreliable)
        {
            if (md.addForce)
                _rigidbody.AddForce(md.force, md.mode);
        }

        [Reconcile]
        private void Reconciliation(ReconcileData rd, Channel channel = Channel.Unreliable)
        {
            var transformCache = transform;

            transformCache.position = rd.Position;
            transformCache.rotation = rd.Rotation;

            _rigidbody.velocity = rd.Velocity;
            _rigidbody.angularVelocity = rd.AngularVelocity;
        }
    }
}