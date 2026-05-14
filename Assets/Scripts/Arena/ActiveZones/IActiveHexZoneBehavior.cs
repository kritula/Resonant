namespace OmniumLessons
{
    public interface IActiveHexZoneBehavior
    {
        void Activate(
            ActiveHexZoneContext context,
            ActiveHexZoneData data);

        void Deactivate();
        void Tick(float deltaTime);
        void OnPlayerEnter(PlayerCharacter player);
        void OnPlayerExit(PlayerCharacter player);
    }
}
