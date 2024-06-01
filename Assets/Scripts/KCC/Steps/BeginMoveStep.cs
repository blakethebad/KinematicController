namespace KCC
{
    public class BeginMoveStep : IBeginMoveStep
    {
        public void Execute(KinematicController kcc, KCCData data)
        {
            data.JumpFrames = default;
        }
    }
}