using UnityEngine;

namespace Leap.Unity.Interaction.Experimental
{

    public class HeuristicsManager : MonoBehaviour
    {
        [field: SerializeField, Tooltip("This is the original provider that your un-modified hand data should be coming from.")]
        public LeapServiceProvider DataProvider { get; private set; } = null;

        [field: SerializeField, Tooltip("This should be a custom provider such as a Physics Provider." +
            " Leave it as null to use the Data Provider for both sets of calculations.")]
        public PostProcessProvider ModifiedProvider { get; private set; } = null;

        

        public void Awake()
        {
            
        }

        private void OnValidate()
        {
            ValidateHeuristics();
        }

        private void ValidateHeuristics()
        {
            if(DataProvider == null)
            {
                DataProvider = FindObjectOfType<LeapServiceProvider>();
            }
        }
    }

}