using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Leap.Unity.Interaction.Experimental
{
    // Data store and conversion when using colliders
    // When no collider is being used, the bones simply act as a data store and will not process information
    public abstract class SimBone : MonoBehaviour
    {
        public SimHand Hand => _hand;
        [SerializeField, HideInInspector]
        protected SimHand _hand;

        public bool IsPalm => _isPalm;
        [SerializeField, HideInInspector]
        protected bool _isPalm = false;

        public int Finger => _finger;
        [SerializeField, HideInInspector]
        private int _finger;

        public int Joint => _joint;
        [SerializeField, HideInInspector]
        private int _joint;

        private Bone _dataBone = new Bone(), _modifiedBone = new Bone();
        private Vector3 _computeCacheA, _computeCacheB;
        private float _computeRadius;

        public SimBone NextBone => _nextBone;
        public SimBone PreviousBone => _previousBone;
        [SerializeField, HideInInspector]
        internal SimBone _nextBone = null, _previousBone = null;

        public Collider Collider => _collider;
        [SerializeField, HideInInspector]
        protected Collider _collider;

        public SimCollider SimCollider => _simCollider;
        protected SimCollider _simCollider;

        public Vector3 oldPosition;

        public HashSet<Rigidbody> ContactingObjects => _contactObjects;
        protected HashSet<Rigidbody> _contactObjects = new HashSet<Rigidbody>();
        public HashSet<Rigidbody> GrabbingObjects => _grabbingObjects;
        protected HashSet<Rigidbody> _grabbingObjects = new HashSet<Rigidbody>();

        public bool IsContacting
        {
            get
            {
                return _contactObjects.Count > 0;
            }
        }

        public bool IsGrabbing
        {
            get
            {
                return _grabbingObjects.Count > 0;
            }
        }

        public Bone Bone
        {
            get
            {
                if (_hand.IsHandReady())
                {
                    return _collider == null ? _dataBone : _modifiedBone;
                }
                return null;
            }
        }

        protected virtual void Awake()
        {
            _collider = GetComponent<Collider>();
        }

        public void SetEnvironment(SimHand hand, int finger, int joint)
        {
            _hand = hand;
            _finger = finger;
            _joint = joint;
        }

        public virtual void AddContacting(Rigidbody rigid)
        {
            _contactObjects.Add(rigid);
        }

        public virtual void RemoveContacting(Rigidbody rigid)
        {
            _contactObjects.Remove(rigid);
        }

        public virtual void AddGrasping(Rigidbody rigid)
        {
            _grabbingObjects.Add(rigid);
        }

        public virtual void RemoveGrasping(Rigidbody rigid)
        {
            _grabbingObjects.Remove(rigid);
        }

        // Bone may need to be changed to something more agnostic
        public abstract void GenerateBone(Bone bone, SimBone previous);

        public abstract void GeneratePalmBone(Hand hand);

        internal void SetBones(SimBone previous, SimBone next)
        {
            _previousBone = previous;
            _nextBone = next;
        }

        public virtual void UpdateBone(Bone bone)
        {
            _dataBone = bone;
            
        }

        public void ComputeBone()
        {
            if (_dataBone != null)
            {
                if (_collider == null)
                {
                    transform.position = _dataBone.Center;
                    transform.rotation = _dataBone.Rotation;
                    if (_isPalm)
                    {
                        
                    }
                    else
                    {
                        _simCollider.ManualLeapBone(_dataBone);
                    }
                }
                else
                {
                    if (_collider.GetType() == typeof(CapsuleCollider))
                    {
                        PhysicsHands.PhysExts.ToWorldSpaceCapsule((CapsuleCollider)_collider, out _computeCacheA, out _computeCacheB, out _computeRadius);
                        _modifiedBone.PrevJoint = _computeCacheB;
                        _modifiedBone.NextJoint = _computeCacheA;
                        _modifiedBone.Width = _computeRadius;
                        _modifiedBone.Center = (_modifiedBone.PrevJoint + _modifiedBone.NextJoint) / 2f;
                        _modifiedBone.Direction = _modifiedBone.PrevJoint - _modifiedBone.NextJoint;
                        _modifiedBone.Length = Vector3.Distance(_computeCacheA, _computeCacheB);
                        _modifiedBone.Rotation = transform.rotation;
                    }
                    _simCollider.UpdateCollider(_collider);
                }
            }
            
        }
    }
}