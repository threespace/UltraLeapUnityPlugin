using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PhysExts = Leap.Unity.Interaction.PhysicsHands.PhysExts;

namespace Leap.Unity.Interaction.Experimental
{
    public abstract class SimHand : MonoBehaviour
    {
        protected SimProvider _provider;
        public SimProvider Provider => _provider;

        [SerializeField]
        protected Chirality _handedness;

        protected Hand _dataHand = new Hand(), _modifiedHand = new Hand();

        protected bool _handWasNull = false;

        protected int _layerMask = 0;
        protected int _handLayer = -1, _handResetLayer = -1;

        // For default hands this will only be a single bone but could be more for more complex hands
        [SerializeField, HideInInspector]
        protected SimBone[] _palmBones;
        public SimBone[] PalmBones => _palmBones;

        // This will be every bone from 1+ index as we do not need the metacarpals by default
        [SerializeField, HideInInspector]
        protected SimBone[] _jointBones;
        public SimBone[] JointBones => _jointBones;

        private Collider[] _colliderCache = new Collider[20];


        private List<IgnoreData> _ignoredData = new List<IgnoreData>();
        private class IgnoreData
        {
            public Rigidbody rigid;
            public Collider[] colliders;
            public float timeout = 0;
            public float radius = 0;

            public IgnoreData(Rigidbody rigid, Collider[] colliders)
            {
                this.rigid = rigid;
                this.colliders = colliders;
            }
        }

        public void SetLayers(int handLayer, int handResetLayer, int layerMask)
        {
            _handLayer = handLayer;
            _handResetLayer = handResetLayer;
            _layerMask = layerMask;
        }

        public void UpdateDataHand(Hand hand)
        {
            _dataHand.CopyFrom(hand);
            if (Time.inFixedTimeStep)
            {
                // Simulation hands need to be within the physics timestep
                UpdateHand(_dataHand, ref _modifiedHand);
            }
        }

        public abstract bool IsHandReady();

        public Hand GetHand()
        {
            if (IsHandReady())
            {
                return _modifiedHand;
            }
            return null;
        }

        public void SetEnvironment(SimProvider provider, Chirality handedness)
        {
            _provider = provider;
            _handedness = handedness;
        }

        public abstract void GenerateHand();

        /// <summary>
        /// Within this function you should both receive and process the new data hand, and then return the modified hand
        /// </summary>
        /// <param name="dataHand">The raw input from the original Leap Provider</param>
        /// <param name="dataHand">The modified output from your </param>
        /// <returns></returns>
        protected abstract void UpdateHand(Hand dataHand, ref Hand modifiedHand);

        public bool HasHandGenerated()
        {
            if(_palmBones == null || _palmBones.Length == 0)
            {
                return false;
            }
            if(_jointBones == null || _jointBones.Length == 0)
            {
                return false;
            }
            return true;
        }

        #region Hand Radius Calculations

        /// <summary>
        /// Used to make sure that the hand is not going to make contact with any object within the scene. Adjusting radius will inflate the joints. This prevents hands from attacking objects when they swap back to contacting.
        /// </summary>
        public bool IsAnyObjectInHandRadius(float radius = 0.005f)
        {
            foreach (var bone in _palmBones)
            {
                if (IsAnyObjectInBoneRadius(bone, radius))
                {
                    return true;
                }
            }

            foreach (var bone in _jointBones)
            {
                if (IsAnyObjectInBoneRadius(bone, radius))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Used to make sure that a bone is not going to make contact with any object within the scene. Adjusting radius will inflate the joints.
        /// </summary>
        public bool IsAnyObjectInBoneRadius(SimBone bone, float radius = 0.005f)
        {
            int overlappingColliders = 0;
            if (bone.Collider.GetType() == typeof(BoxCollider))
            {
                overlappingColliders = PhysExts.OverlapBoxNonAllocOffset((BoxCollider)bone.Collider, Vector3.zero, _colliderCache, _layerMask, QueryTriggerInteraction.Ignore, radius);
            }
            else if (bone.Collider.GetType() == typeof(CapsuleCollider))
            {
                overlappingColliders = PhysExts.OverlapCapsuleNonAllocOffset((CapsuleCollider)bone.Collider, Vector3.zero, _colliderCache, _layerMask, QueryTriggerInteraction.Ignore, radius);
            }

            for (int i = 0; i < overlappingColliders; i++)
            {
                if (WillColliderAffectHand(_colliderCache[i]))
                {
                    return true;
                }
            }

            return false;
        }

        protected virtual bool WillColliderAffectHand(Collider collider)
        {
            if (collider.attachedRigidbody != null)
            {
                return false;
            }
            if (gameObject != collider.gameObject)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks to see whether the hand will be in contact with a specified object. Radius can be used to inflate the bones.
        /// </summary>
        public bool IsObjectInHandRadius(Rigidbody rigid, float radius = 0f)
        {
            if (rigid == null)
                return false;

            foreach (var bone in _palmBones)
            {
                if (IsObjectInBoneRadius(rigid, bone, radius))
                {
                    return true;
                }
            }

            foreach (var bone in _jointBones)
            {
                if (IsObjectInBoneRadius(rigid, bone, radius))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks to see whether a specific bone will be in contact with a specified object. Radius can be used to inflate the bone.
        /// </summary>
        public bool IsObjectInBoneRadius(Rigidbody rigid, SimBone bone, float radius = 0f)
        {
            int overlappingColliders = 0;
            if (bone.Collider.GetType() == typeof(BoxCollider))
            {
                overlappingColliders = PhysExts.OverlapBoxNonAllocOffset((BoxCollider)bone.Collider, Vector3.zero, _colliderCache, _layerMask, QueryTriggerInteraction.Ignore, radius);
            }
            else if (bone.Collider.GetType() == typeof(CapsuleCollider))
            {
                overlappingColliders = PhysExts.OverlapCapsuleNonAllocOffset((CapsuleCollider)bone.Collider, Vector3.zero, _colliderCache, _layerMask, QueryTriggerInteraction.Ignore, radius);
            }
            for (int i = 0; i < overlappingColliders; i++)
            {
                if (_colliderCache[i].attachedRigidbody == rigid)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Disables all collisions between a rigidbody and hand. Will automatically handle all colliders on the rigidbody. Timeout lets you specify a minimum time to ignore collisions for.
        /// </summary>
        public void IgnoreCollision(Rigidbody rigid, float timeout = 0, float radius = 0)
        {
            TogglePhysicsIgnore(rigid, true, timeout, radius);
        }

        private void TogglePhysicsIgnore(Rigidbody rigid, bool ignore, float timeout = 0, float radius = 0)
        {
            Collider[] colliders = rigid.GetComponentsInChildren<Collider>(true);
            foreach (var collider in colliders)
            {
                foreach (var palm in _palmBones)
                {
                    Physics.IgnoreCollision(collider, palm.Collider, ignore);
                }
                foreach (var joint in _jointBones)
                {
                    Physics.IgnoreCollision(collider, joint.Collider, ignore);
                }
            }
            int ind = _ignoredData.FindIndex(x => x.rigid == rigid);
            if (ignore)
            {
                if (ind == -1)
                {
                    _ignoredData.Add(new IgnoreData(rigid, colliders) { timeout = timeout, radius = radius });
                }
                else
                {
                    _ignoredData[ind].timeout = timeout;
                    _ignoredData[ind].radius = radius;
                }
            }
            else
            {
                if (ind != -1)
                {
                    _ignoredData.RemoveAt(ind);
                }
            }
        }

        protected void HandleIgnoredObjects()
        {
            if (_ignoredData.Count > 0)
            {
                for (int i = 0; i < _ignoredData.Count; i++)
                {
                    if (_ignoredData[i].timeout >= 0)
                    {
                        _ignoredData[i].timeout -= Time.fixedDeltaTime;
                    }

                    if (_ignoredData[i].timeout <= 0 && !IsObjectInHandRadius(_ignoredData[i].rigid, _ignoredData[i].radius))
                    {
                        TogglePhysicsIgnore(_ignoredData[i].rigid, false);
                        i--;
                    }
                }
            }
        }
        #endregion
    }
}