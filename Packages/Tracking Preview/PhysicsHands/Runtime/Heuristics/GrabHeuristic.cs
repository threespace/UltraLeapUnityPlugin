namespace Leap.Unity.Interaction.Experimental
{
    [System.Serializable]
    public class GrabHeuristic
    {
        public float pinchStrength = 0;

        public float fistStrength = 0;
        public float[] fingerStrength = new float[5];

        public void Calculate(Hand hand)
        {
            if (hand == null)
                return;

            fistStrength = hand.GetFistStrength();
            for (int i = 0; i < hand.Fingers.Count; i++)
            {
                fingerStrength[i] = hand.GetFingerStrength(i);
            }
            pinchStrength = hand.PinchDistance / 1000f;
        }
    }
}