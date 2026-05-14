namespace OmniumLessons
{
    public static class ActiveHexZoneBehaviorFactory
    {
        public static IActiveHexZoneBehavior Create(
            HexActiveZoneType zoneType)
        {
            switch (zoneType)
            {
                case HexActiveZoneType.ResonanceZone:
                    return new ResonanceZoneBehavior();

                case HexActiveZoneType.CollapseTile:
                    return new CollapseTileBehavior();

                case HexActiveZoneType.PullNodeTile:
                    return new PullNodeTileBehavior();

                default:
                    return null;
            }
        }
    }
}
