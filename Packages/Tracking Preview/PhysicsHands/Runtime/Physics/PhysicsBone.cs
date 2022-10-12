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
            _body = gameObject.AddComponent<ArticulationBody>();
            _collider = gameObject.AddComponent<BoxCollider>();

        }

        private void SetupPalmCollider(Hand hand)
        {

        }

        private void GenerateJointBone()
        {

        }
    }
}