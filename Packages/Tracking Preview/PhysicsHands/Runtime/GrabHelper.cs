using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        public class GrabValues
        {
            public float[] fingerStrength = new float[5];
            public float[] originalFingerStrength = new float[5];
            public Vector3 offset;
            public Quaternion originalHandRotation, rotationOffset;
        }

        public HashSet<SimBone> BoneHash => _boneHash;
        private HashSet<SimBone> _boneHash = new HashSet<SimBone>();
        public int TotalBones { get { return _boneHash.Count; } }

        public Dictionary<SimHand, HashSet<SimBone>[]> Bones => _bones;
        private Dictionary<SimHand, HashSet<SimBone>[]> _bones = new Dictionary<SimHand, HashSet<SimBone>[]>();

        public HashSet<SimHand> GraspingCandidates => _graspingCandidates;
        private HashSet<SimHand> _graspingCandidates = new HashSet<SimHand>();

        public List<SimHand> GraspingHands => _graspingHands;
        private List<SimHand> _graspingHands = new List<SimHand>();

        private Dictionary<SimHand, GrabValues> _grabValues = new Dictionary<SimHand, GrabValues>();

        public bool WasGrasped { get; private set; }
        private bool _justGrasped = false;
        private Rigidbody _rigid;
        public Rigidbody Rigidbody => _rigid;
        private List<Collider> _colliders = new List<Collider>();
        public Pose previousPose;
        public Pose previousPalmPose;

        private Vector3 _newPosition;
        private Quaternion _newRotation;

        private bool _oldKinematic, _oldGravity;

        // This means we can have bones stay "attached" for a small amount of time
        private List<SimBone> _boneCooldownItems = new List<SimBone>();
        private List<float> _boneCooldownTime = new List<float>();


        private GrabManager _manager;
        private GrabConfig _config;

        public GrabHelper(GrabManager grabManager, GrabConfig config, Rigidbody rigidbody) : base(grabManager)
        {
            _manager = grabManager;
            _config = config;
            _rigid = rigidbody;
        }

        private bool Grasped(SimHand hand)
        {
            if(_bones.TryGetValue(hand, out var bones))
            {
                // These values are limited by the EligibleBones
                return (bones[0].Count > 0 || bones[5].Count > 0) && // A thumb or palm bone
                    ((bones[1].Count > 0) || // The intermediate or distal of the index
                    (bones[2].Count > 0) || // The distal of the middle
                    (bones[3].Count > 0 && bones[3].Any(x => x.Joint == 2)) || // The distal of the ring
                    (bones[4].Count > 0 && bones[4].Any(x => x.Joint == 2))); // The distal of the pinky
            }
            return false;
        }

        private bool Grasped(SimHand hand, int finger)
        {
            if (_bones.TryGetValue(hand, out var bones))
            {
                switch (finger)
                {
                    case 0:
                    case 5:
                        return bones[finger].Count > 0;

                    case 1:
                    case 2:
                        return bones[finger].Count > 0 && bones[finger].Any(x => x.Joint != 0);

                    case 3:
                    case 4:
                        return bones[finger].Count > 0 && bones[finger].Any(x => x.Joint == 2);
                }
                return false;
            }
            return false;
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