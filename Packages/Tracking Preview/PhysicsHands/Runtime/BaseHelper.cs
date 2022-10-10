using UnityEngine;
using Leap;

namespace Leap.Unity.Interaction.Experimental
{
    public abstract class BaseHelper
    {
        public enum HelperResult
        {
            Continue,
            Succeed,
            Exit
        }

        protected HeuristicsManager _heuristicsManager;

        public BaseHelper(HeuristicsManager heuristicsManager)
        {
            _heuristicsManager = heuristicsManager;
        }

        public abstract void OnCreate();

        public abstract HelperResult OnUpdate(Frame dataFrame, Frame modifiedFrame);

        public abstract HelperResult OnFixedUpdate(Frame dataFixedFrame, Frame modifiedFixedFrame);

        public abstract void OnDestroy();
    }
}