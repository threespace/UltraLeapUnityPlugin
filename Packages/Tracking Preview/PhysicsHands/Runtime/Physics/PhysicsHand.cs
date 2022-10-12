using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Leap.Unity.Interaction.Experimental
{
    public class PhysicsHand : SimHand
    {
        public const int FINGERS = 5, BONES = 3;

        private int _lastFrameTeleport = 0;
        private bool _ghosted = false, _hasReset = false;

        private float _graspMass = 0;

        private PhysicsBone _palmBone { get { return (PhysicsBone)_palmBones[0]; } set { _palmBones[0] = value; } }

        public override bool IsHandReady()
        {
            return _hasReset;
        }

        public override Hand GetHand()
        {
            if (_hasReset)
            {
                return _modifiedHand;
            }
            return null;
        }

        #region Hand Generation

        public override void GenerateHand()
        {
            Leap.Hand leapHand = TestHandFactory.MakeTestHand(_handedness == Chirality.Left ? true : false, pose: TestHandFactory.TestHandPose.HeadMountedB);

            _palmBones = new PhysicsBone[1];

            GameObject palmGo = new GameObject($"{(_handedness == Chirality.Left ? "Left" : "Right")} Palm", typeof(PhysicsBone));
            PhysicsBone palmBone = palmGo.GetComponent<PhysicsBone>();
            Transform palmTransform = palmGo.transform;

            palmTransform.parent = transform;
            palmTransform.position = leapHand.PalmPosition;
            palmTransform.rotation = leapHand.Rotation;

            _palmBone = palmBone;

            _palmBone.SetEnvironment(this, 5, 0);
            _palmBone.GeneratePalmBone(leapHand);

            _jointBones = new PhysicsBone[FINGERS * BONES];

            Transform lastTransform;
            Bone knuckleBone, prevBone, bone;
            int boneArrayIndex;

            for (int fingerIndex = 0; fingerIndex < FINGERS; fingerIndex++)
            {
                lastTransform = palmTransform;
                knuckleBone = leapHand.Fingers[fingerIndex].Bone(0);
                for (int jointIndex = 0; jointIndex < BONES; jointIndex++)
                {
                    prevBone = leapHand.Fingers[fingerIndex].Bone((Bone.BoneType)jointIndex);
                    bone = leapHand.Fingers[fingerIndex].Bone((Bone.BoneType)(jointIndex + 1)); // +1 to skip first bone.
                    boneArrayIndex = fingerIndex * BONES + jointIndex;

                    GameObject boneGo = new GameObject($"{(_handedness == Chirality.Left ? "Left" : "Right")} {fingerIndex} {jointIndex}", typeof(PhysicsBone));
                    PhysicsBone bonePhys = boneGo.GetComponent<PhysicsBone>();
                    Transform boneTransform = boneGo.transform;

                    boneTransform.parent = lastTransform;

                    bonePhys.SetEnvironment(this, fingerIndex, jointIndex);
                    bonePhys.GenerateBone(bone, jointIndex > 0 ? _jointBones[boneArrayIndex] : null);


                    _jointBones[boneArrayIndex] = bonePhys;
                }
            }
        }

        #endregion

        #region Hand Updating

        protected override void UpdateHand()
        {

        }

        #endregion
    }
}