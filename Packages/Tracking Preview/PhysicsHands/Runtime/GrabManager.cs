using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Leap.Unity.Interaction.Experimental
{
    public class GrabManager : MonoBehaviour
    {
        [field: SerializeField]
        public LeapInteractionManager InteractionManager { get; private set; } = null;

        [SerializeField]
        private GrabConfig _grabConfig = new GrabConfig();
        
        public void Awake()
        {
            if(InteractionManager.ModifiedProvider == null)
            {

            }
        }

        private void OnValidate()
        {
            ValidateProvider();
        }

        private void ValidateProvider()
        {
            if (InteractionManager == null)
            {
                InteractionManager = FindObjectOfType<LeapInteractionManager>();
            }
        }

    }
}