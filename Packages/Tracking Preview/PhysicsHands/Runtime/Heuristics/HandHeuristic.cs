namespace Leap.Unity.Interaction.Experimental
{
    [System.Serializable]
    public class HandHeuristic
    {
        public bool tracked = false;
        public GrabHeuristic grab = new GrabHeuristic();

        public void Calculate(Hand hand)
        {
            tracked = hand != null;
            if (!tracked)
                return;

            grab.Calculate(hand);
        }
    }
}