using Leap.Unity.Interaction.PhysicsHands;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Leap.Unity.Interaction.Experimental
{
    public class GrabHelper : BaseHelper
    {
        private const float GRAB_COOLDOWNTIME = 0.025f;
        private const float MINIMUM_STRENGTH = 0.25f, MINIMUM_THUMB_STRENGTH = 0.2f;
        private const float REQUIRED_ENTRY_STRENGTH = 0.15f, REQUIRED_EXIT_STRENGTH = 0.05f, REQUIRED_THUMB_EXIT_STRENGTH = 0.1f, REQUIRED_PINCH_DISTANCE = 0.018f;

        public enum GrabState
        {
            Idle,
            Contact,
            Grab
        }

        public GrabState CurrentGrabState { get; private set; } = GrabState.Idle;

        public HashSet<PhysicsBone> BoneHash => _boneHash;
        private HashSet<PhysicsBone> _boneHash = new HashSet<PhysicsBone>();
        public int TotalBones { get { return _boneHash.Count; } }

        public Dictionary<PhysicsHand, HashSet<PhysicsBone>[]> Bones => _bones;
        private Dictionary<PhysicsHand, HashSet<PhysicsBone>[]> _bones = new Dictionary<PhysicsHand, HashSet<PhysicsBone>[]>();

        public HashSet<PhysicsHand> GraspingCandidates => _graspingCandidates;
        private HashSet<PhysicsHand> _graspingCandidates = new HashSet<PhysicsHand>();

        public List<PhysicsHand> GraspingHands => _graspingHands;
        private List<PhysicsHand> _graspingHands = new List<PhysicsHand>();

        private Dictionary<PhysicsHand, GrabValues> _grabValues = new Dictionary<PhysicsHand, GrabValues>();

        public class GrabValues
        {
            public float[] fingerStrength = new float[5];
            public float[] originalFingerStrength = new float[5];
            public Vector3[] tipPositions = new Vector3[5];
            public Matrix4x4 mat;
            public Vector3 offset;
            public Quaternion originalHandRotation, rotationOffset;
        }

        public GrabHelper(HeuristicsManager heuristicsManager) : base(heuristicsManager)
        {

        }

        public override void OnCreate()
        {
        }

        public override void OnDestroy()
        {
        }

        public override HelperResult OnFixedUpdate(Frame dataFrame, Frame modifiedFrame)
        {
            return HelperResult.Continue;
        }

        public override HelperResult OnUpdate(Frame dataFixedFrame, Frame modifiedFixedFrame)
        {
            return HelperResult.Continue;
        }
    }
}