using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PhysExts = Leap.Unity.Interaction.PhysicsHands.PhysExts;

namespace Leap.Unity.Interaction.Experimental
{
    public enum ColliderType
    {
        Box,
        Sphere,
        Capsule
    }

    /// <summary>
    /// Stores information from colliders so they can be re-used later on. It tries to calculate as much information from the collider so that it doesn't need to re-calculate it later.
    /// </summary>
    public class SimCollider
    {
        public ColliderType colliderType;

        public Transform transform;

        public Vector3 center = Vector3.zero;

        /// <summary>
        /// Used for box
        /// </summary>
        public Vector3 halfExtents;

        /// <summary>
        /// Used for capsule
        /// </summary>
        public Vector3 direction;
        /// <summary>
        /// Used for capsule
        /// </summary>
        public float height;

        public Vector3 lossyScale;

        public float radius = -1;

        public SimCollider(ColliderType type, Transform transform)
        {
            this.colliderType = type;
            this.transform = transform;
        }

        /// <summary>
        /// Automatically update the information by passing a collider
        /// </summary>
        /// <param name="collider"></param>
        public void UpdateCollider(Collider collider)
        {
            if (ValidateCollider(collider))
            {
                lossyScale = PhysExts.AbsVec3(transform.lossyScale);
                switch (colliderType)
                {
                    case ColliderType.Box:
                        UpdateBoxCollider((BoxCollider)collider);
                        break;
                    case ColliderType.Sphere:
                        UpdateSphereCollider((SphereCollider)collider);
                        break;
                    case ColliderType.Capsule:
                        UpdateCapsuleCollider((CapsuleCollider)collider);
                        break;
                }
            }
        }

        public bool ValidateCollider(Collider collider)
        {
            switch (colliderType)
            {
                case ColliderType.Box:
                    if (collider.GetType() == typeof(BoxCollider))
                    {
                        return true;
                    }
                    break;
                case ColliderType.Sphere:
                    if (collider.GetType() == typeof(SphereCollider))
                    {
                        return true;
                    }
                    break;
                case ColliderType.Capsule:
                    if (collider.GetType() == typeof(CapsuleCollider))
                    {
                        return true;
                    }
                    break;
            }
            return false;
        }

        private void UpdateBoxCollider(BoxCollider collider)
        {
            center = collider.center;
            halfExtents = Vector3.Scale(lossyScale, collider.size) * 0.5f;
        }

        private void UpdateSphereCollider(SphereCollider collider)
        {
            center = collider.center;
            radius = collider.radius * PhysExts.MaxVec3(PhysExts.AbsVec3(lossyScale));
        }

        private void UpdateCapsuleCollider(CapsuleCollider collider)
        {
            center = collider.center;

            switch (collider.direction)
            {
                case 0: // x
                    radius = Mathf.Max(lossyScale.y, lossyScale.z) * collider.radius;
                    height = lossyScale.x * collider.height;
                    direction = transform.TransformDirection(Vector3.right);
                    break;
                case 1: // y
                    radius = Mathf.Max(lossyScale.x, lossyScale.z) * collider.radius;
                    height = lossyScale.y * collider.height;
                    direction = transform.TransformDirection(Vector3.up);
                    break;
                case 2: // z
                    radius = Mathf.Max(lossyScale.x, lossyScale.y) * collider.radius;
                    height = lossyScale.z * collider.height;
                    direction = transform.TransformDirection(Vector3.forward);
                    break;
            }

            if (height < radius * 2f)
            {
                direction = Vector3.zero;
            }
        }

        /// <summary>
        /// Converts a basic hand into collider usable values
        /// </summary>
        public void ManualLeapPalm(Hand hand)
        {
            if(hand == null)
            {
                return;
            }
            center = Vector3.zero;
            halfExtents = PhysicsHands.PhysicsHandsUtils.CalculatePalmSize(hand) * 0.5f;
        }

        /// <summary>
        /// Converts the leap bone into collider usable values
        /// </summary>
        public void ManualLeapBone(Bone bone)
        {
            if(bone == null)
            {
                return;
            }
            center = bone.Center;
            direction = bone.Direction;
            height = bone.Length;
            radius = bone.Width;
        }

        public int CalculateNonAlloc(Collider[] results, int layerMask = Physics.DefaultRaycastLayers, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
        {
            return CalculateNonAlloc(results, Vector3.zero, layerMask: layerMask, queryTriggerInteraction: queryTriggerInteraction);
        }

        public int CalculateNonAlloc(Collider[] results, Vector3 offset, float extraRadius = 0, int layerMask = Physics.DefaultRaycastLayers, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
        {
            switch (colliderType)
            {
                case ColliderType.Box:
                    return CalculateBox(results, offset, extraRadius, layerMask, queryTriggerInteraction);
                case ColliderType.Sphere:
                    return CalculateSphere(results, offset, extraRadius, layerMask, queryTriggerInteraction);
                case ColliderType.Capsule:
                    return CalculateCapsule(results, offset, extraRadius, layerMask, queryTriggerInteraction);
            }
            return 0;
        }

        private int CalculateBox(Collider[] results, Vector3 offset, float extraRadius, int layerMask = Physics.DefaultRaycastLayers, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
        {
            return Physics.OverlapBoxNonAlloc(
                transform.TransformPoint(center + offset),
                halfExtents + (extraRadius == 0 ? Vector3.zero : new Vector3(radius, radius, radius)),
                results,
                transform.rotation,
                layerMask,
                queryTriggerInteraction);
        }

        private int CalculateSphere(Collider[] results, Vector3 offset, float extraRadius, int layerMask = Physics.DefaultRaycastLayers, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
        {
            return Physics.OverlapSphereNonAlloc(
                transform.TransformPoint(center + offset),
                radius + extraRadius,
                results,
                layerMask,
                queryTriggerInteraction);
        }

        private int CalculateCapsule(Collider[] results, Vector3 offset, float extraRadius, int layerMask = Physics.DefaultRaycastLayers, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
        {
            Vector3 centroid = transform.TransformPoint(center + offset);
            return Physics.OverlapCapsuleNonAlloc(
                centroid + direction * (height * 0.5f - (radius + extraRadius)),
                centroid - direction * (height * 0.5f - (radius + extraRadius)),
                radius + extraRadius,
                results,
                layerMask,
                queryTriggerInteraction);
        }
    }
}