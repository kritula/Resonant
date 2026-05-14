namespace OmniumLessons
{
	public enum CharacterType
	{
		None = 0,
		DefaultPlayer = 1,
		DefaultEnemy = 2,
		FastEnemy = 3,
		TankEnemy = 4,
        Boss_Null_Core = 100,
    }

    public static class CharacterSpawnHeights
    {
        public static float Get(CharacterType characterType)
        {
            switch (characterType)
            {
                case CharacterType.DefaultPlayer:
                case CharacterType.DefaultEnemy:
                    return 1.7f;

                case CharacterType.FastEnemy:
                    return 1.9f;

                case CharacterType.TankEnemy:
                    return 2.3f;

                case CharacterType.Boss_Null_Core:
                    return 3f;

                default:
                    return 1.7f;
            }
        }

        public static UnityEngine.Vector3 Apply(
            UnityEngine.Vector3 position,
            CharacterType characterType)
        {
            position.y = Get(characterType);
            return position;
        }
    }
}
