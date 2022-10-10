using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Leap.Unity.Interaction.Experimental
{
    public class GrabManager : MonoBehaviour
    {
        [field: SerializeField]
        public HeuristicsManager Heuristics { get; private set; } = null;


        public void Awake()
        {

        }

        private void OnValidate()
        {
            ValidateProvider();
        }

        private void ValidateProvider()
        {
            if (Heuristics == null)
            {
                Heuristics = FindObjectOfType<HeuristicsManager>();
            }
        }

    }
}