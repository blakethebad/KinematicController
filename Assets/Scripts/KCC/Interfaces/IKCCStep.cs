namespace KCC
{
    public interface IKCCStep
    {
        void Execute(KinematicController kcc, KCCData data);
    }
    
    public interface IBeginMoveStep : IKCCStep {}
    public interface ICalculationStep : IKCCStep {} 
    public interface ICollisionStep : IKCCStep {}
    public interface IAfterMoveStep : IKCCStep {}
    public interface IEndMoveStep : IKCCStep {}
}