namespace OmniumLessons
{
    public interface IAttackStatsDecorator
    {
        AttackStats Decorate(AttackStats stats);
    }
}