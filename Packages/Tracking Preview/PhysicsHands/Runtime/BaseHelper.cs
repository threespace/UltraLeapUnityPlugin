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

        protected GrabManager _grabManager;

        public BaseHelper(GrabManager grabManager)
        {
            _grabManager = grabManager;
        }

        public abstract void OnCreate();

        public abstract HelperResult OnUpdate(Frame dataFrame, Frame modifiedFrame);

        public abstract HelperResult OnFixedUpdate(Frame dataFixedFrame, Frame modifiedFixedFrame);

        public abstract void OnDestroy();
    }
}