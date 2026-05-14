using System.Collections.Generic;
using UnityEngine;

namespace OmniumLessons
{
    public static class CharacterCollisionPassThrough
    {
        public static void ConfigureForNewCharacter(
            Character character,
            IReadOnlyList<Character> activeCharacters)
        {
            if (character == null)
                return;

            if (activeCharacters == null)
                return;

            for (int i = 0; i < activeCharacters.Count; i++)
            {
                Character other = activeCharacters[i];

                if (other == null || other == character)
                    continue;

                IgnoreCollisions(character, other);
            }
        }

        private static void IgnoreCollisions(
            Character first,
            Character second)
        {
            Collider[] firstColliders =
                first.GetComponentsInChildren<Collider>(true);

            Collider[] secondColliders =
                second.GetComponentsInChildren<Collider>(true);

            for (int i = 0; i < firstColliders.Length; i++)
            {
                Collider firstCollider = firstColliders[i];

                if (firstCollider == null)
                    continue;

                for (int j = 0; j < secondColliders.Length; j++)
                {
                    Collider secondCollider = secondColliders[j];

                    if (secondCollider == null)
                        continue;

                    Physics.IgnoreCollision(
                        firstCollider,
                        secondCollider,
                        true);
                }
            }
        }
    }
}
