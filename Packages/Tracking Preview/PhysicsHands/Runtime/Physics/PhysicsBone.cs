using LeapInternal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Leap.Unity.Interaction.Experimental
{
    public class PhysicsBone : SimBone
    {
        [SerializeField, HideInInspector]
        private ArticulationBody _body;

        public ArticulationBody ArticulationBody
        {
            get
            {
                if (_body == null)
                {
                    _body = GetComponent<ArticulationBody>();
                }
                return _body;
            }
        }

        private PhysicsProvider _physicsProvider { get { return (PhysicsProvider)_hand.Provider; } }

        [SerializeField, HideInInspector]
        private float _origXDriveLimit = float.MaxValue, _currentXDriveLimit = float.MaxValue;

        public float OriginalXDriveLimit => _origXDriveLimit;
        public float XDriveLimit => _currentXDriveLimit;

        public override void AddGrasping(Rigidbody rigid)
        {
            if (_grabbingObjects.Count == 0 && _body != null && _body.jointPosition.dofCount > 0)
            {
                _currentXDriveLimit = _body.jointPosition[0] * Mathf.Rad2Deg;
            }
            base.AddGrasping(rigid);
        }

        public override void GenerateBone(Bone bone, SimBone previous)
        {
            _body = gameObject.AddComponent<ArticulationBody>();
            
            GenerateJointBone();

            if (previous != null)
            {
                previous._nextBone = this;
                _previousBone = previous;
            }

            _collider.material = ((PhysicsProvider)_hand.Provider).HandMaterial;
            
        }

        public override void GeneratePalmBone(Hand hand)
        {
            _isPalm = true;
            SetupPalmCollider(hand);
            SetupPalmBody();

        }

        private void SetupPalmCollider(Hand hand)
        {
            if (_collider == null)
            {
                _collider = gameObject.AddComponent<BoxCollider>();
            }
        }

        private void SetupJointCollider(Bone bone)
        {

        }

        private void SetupPalmBody()
        {
            if(_body == null)
            {
                _body = gameObject.AddComponent<ArticulationBody>();
            }
            _body.immovable = false;
#if UNITY_2021_2_OR_NEWER
            _body.matchAnchors = false;
#endif
            _body.mass = _physicsProvider.PerBoneMass * 3f;
            _body.solverIterations = _physicsProvider.solverIterations;
            _body.solverVelocityIterations = _physicsProvider.solverVelocityIterations;
            _body.angularDamping = _physicsProvider.angularDamping;
            _body.useGravity = false;
            _body.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        }

        private void SetupJointBody()
        {
            if (_body == null)
            {
                _body = gameObject.AddComponent<ArticulationBody>();
            }
            _body.anchorPosition = new Vector3(0f, 0f, 0f);
            _body.anchorRotation = Quaternion.identity;
#if UNITY_2021_2_OR_NEWER
            _body.matchAnchors = false;
#endif
            _body.mass = _physicsProvider.PerBoneMass;
            _body.solverIterations = _physicsProvider.solverIterations;
            _body.solverVelocityIterations = _physicsProvider.solverVelocityIterations;
            _body.maxAngularVelocity = _physicsProvider.maxAngularVelocity;
            _body.maxDepenetrationVelocity = _physicsProvider.maxDepenetrationVelocity;
            _body.useGravity = false;
            _body.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        }

        private void GenerateJointBone()
        {

        }
    }
}