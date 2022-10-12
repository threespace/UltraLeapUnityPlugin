using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Leap.Unity.Interaction.Experimental
{
    [System.Serializable]
    public class GrabConfig
    {
        [field: SerializeField]
        public float TriggerDistance { get; private set; } = 0.004f;
        [field: SerializeField]
        public float MinimumStrength { get; private set; } = 0.25f;
        [field: SerializeField]
        public float MinimumThumbStrength { get; private set; } = 0.2f;
        [field: SerializeField]
        public float RequiredEntryPercent { get; private set; } = 0.15f;
        [field: SerializeField]
        public float RequiredExitPercent { get; private set; } = 0.05f;
        [field: SerializeField]
        public float RequiredThumbExitPercent { get; private set; } = 0.1f;
        [field: SerializeField]
        public float RequiredPinchDistance { get; private set; } = 0.018f;
    }
}