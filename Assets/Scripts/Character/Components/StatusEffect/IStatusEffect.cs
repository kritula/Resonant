namespace OmniumLessons
{
    public interface IStatusEffect
    {
        bool IsFinished { get; }

        void Initialize(Character owner);
        void OnUpdate();
    }
}