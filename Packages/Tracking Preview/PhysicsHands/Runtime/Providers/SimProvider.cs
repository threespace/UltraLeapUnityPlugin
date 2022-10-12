using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Leap.Unity.Interaction.Experimental
{
    public abstract class SimProvider : PostProcessProvider
    {
        [field: SerializeField]
        public SimHand LeftHand { get; protected set; } = null;
        [field: SerializeField]
        public SimHand RightHand { get; protected set; } = null;

        private int _leftIndex = -1, _rightIndex = -1;
        protected int leftIndex => _leftIndex;

        #region Sim Hand Generation

        public void GenerateHands()
        {
            if(LeftHand != null)
            {
                Destroy(LeftHand.gameObject);
            }
            if(RightHand != null)
            {
                Destroy(RightHand.gameObject);
            }
            PreHandGeneration();
            LeftHand = GenerateHand(Chirality.Left);
            RightHand = GenerateHand(Chirality.Right);
            PostHandGeneration();
        }

        /// <summary>
        /// This happens for the provider as a whole, just before generating the individual hands
        /// </summary>
        public abstract void PreHandGeneration();

        /// <summary>
        /// This happens for the provider as a whole, just after generating the individual hands
        /// </summary>
        public abstract void PostHandGeneration();
        
        /// <summary>
        /// This happens per hand.
        /// </summary>
        public abstract SimHand GenerateHand(Chirality handedness);

        #endregion

        public override void ProcessFrame(ref Frame inputFrame)
        {
            if (!Application.isPlaying)
            {
                return;
            }

            _leftIndex = inputFrame.Hands.FindIndex(x => x.IsLeft);
            if (_leftIndex != -1)
            {
                if(LeftHand == null)
                {
                    LeftHand = GenerateHand(Chirality.Left);
                }

                if (LeftHand != null)
                {
                    LeftHand.UpdateDataHand(inputFrame.Hands[_leftIndex]);
                    ApplyHand(_leftIndex, ref inputFrame, LeftHand);
                }
            }

            _rightIndex = inputFrame.Hands.FindIndex(x => x.IsLeft);
            if (_rightIndex != -1)
            {
                if (RightHand == null)
                {
                    RightHand = GenerateHand(Chirality.Left);
                }

                if (RightHand != null)
                {
                    RightHand.UpdateDataHand(inputFrame.Hands[_rightIndex]);
                    ApplyHand(_rightIndex, ref inputFrame, RightHand);
                }
            }
        }

        private void ApplyHand(int index, ref Frame inputFrame, SimHand hand)
        {
            if (index != -1)
            {
                if (hand.IsHandReady())
                {
                    inputFrame.Hands[index].CopyFrom(hand.GetHand());
                }
                else
                {
                    inputFrame.Hands.RemoveAt(index);
                }
            }
        }


        protected override void OnValidate()
        {
            base.OnValidate();
            dataUpdateMode = DataUpdateMode.FixedUpdateOnly;
        }

    }
}