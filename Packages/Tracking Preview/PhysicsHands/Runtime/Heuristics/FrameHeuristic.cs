namespace Leap.Unity.Interaction.Experimental
{
    [System.Serializable]
    public class FrameHeuristic
    {
        public long frameID = 0;

        public HandHeuristic leftHand = new HandHeuristic(), rightHand = new HandHeuristic();
    }
}