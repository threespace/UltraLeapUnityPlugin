using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Leap.Unity.Interaction.Experimental
{
    public class PhysicsProvider : SimProvider
    {
        public PhysicsHand LeftPhysicsHand { get { return (PhysicsHand)LeftHand; } }
        public PhysicsHand RightPhysicsHand { get { return (PhysicsHand)RightHand; } }

        // Physics Hand Settings
        public float Strength => _strength;
        [SerializeField, Range(0.1f, 2f)]
        private float _strength = 2f;

        private float _forceLimit = 1000f;
        private float _stiffness = 100f;

        public float PerBoneMass => _perBoneMass;
        [SerializeField, Tooltip("The mass of each finger bone; the palm will be 3x this.")]
        private float _perBoneMass = 0.6f;

        public float HandTeleportDistance => _handTeleportDistance;
        [SerializeField, Tooltip("The distance between the physics and original data hand can reach before it snaps back to the original hand position."), Range(0.01f, 0.5f)]
        private float _handTeleportDistance = 0.1f;

        public float HandGraspTeleportDistance => _handGraspTeleportDistance;
        [SerializeField, Tooltip("The distance between the physics and original data hand can reach before it snaps back to the original hand position. This is used when a hand is reported as grasping."), Range(0.01f, 0.5f)]
        private float _handGraspTeleportDistance = 0.2f;

        [SerializeField, HideInInspector]
        private PhysicMaterial _handMaterial;
        public PhysicMaterial HandMaterial => _handMaterial;

        public int solverIterations = 50;
        public int solverVelocityIterations = 20;
        public float angularDamping = 50f, maxAngularVelocity = 1.75f, maxDepenetrationVelocity = 3f;

        public override SimHand GenerateHand(Chirality handedness)
        {
            GameObject go = new GameObject($"{(handedness == Chirality.Left ? "Left" : "Right")} Hand", typeof(PhysicsHand));
            go.transform.parent = transform;

            PhysicsHand hand = go.GetComponent<PhysicsHand>();
            hand.SetEnvironment(this, Chirality.Left);
            hand.GenerateHand();
            return hand;
        }

        public override void PreHandGeneration()
        {
            _handMaterial = CreateHandPhysicsMaterial();
        }

        private PhysicMaterial CreateHandPhysicsMaterial()
        {
            PhysicMaterial material = new PhysicMaterial("HandPhysics");

            material.dynamicFriction = 0.1f;
            material.staticFriction = 0.15f;
            material.frictionCombine = PhysicMaterialCombine.Average;
            material.bounceCombine = PhysicMaterialCombine.Minimum;

            return material;
        }

        public override void PostHandGeneration()
        {
            
        }
    }
}