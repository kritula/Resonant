using System.Collections.Generic;
using UnityEngine;

namespace OmniumLessons
{
    public class CharacterFactory : MonoBehaviour
    {
        [Header("Prefabs")]
        [SerializeField] private Character _playerCharacterPrefab;
        [SerializeField] private Character _enemyCharacterPrefab;
        [SerializeField] private Character _fastEnemyCharacterPrefab;
        [SerializeField] private Character _tankEnemyCharacterPrefab;
        [SerializeField] private Character _bossNullCorePrefab;

        private readonly Dictionary<CharacterType, Queue<Character>> _disabledCharactersPool =
            new Dictionary<CharacterType, Queue<Character>>();

        private readonly List<Character> _activeCharactersPool =
            new List<Character>(16);

        public Character Player { get; private set; }
        public List<Character> ActiveCharacters => _activeCharactersPool;

        public Character CreateCharacter(CharacterType characterType)
        {
            Character character = GetFromPool(characterType);

            if (character == null)
            {
                character = InstantiateCharacter(characterType);
            }

            if (character == null)
            {
                Debug.LogError($"CharacterFactory: failed to create character of type {characterType}.");
                return null;
            }

            if (!_activeCharactersPool.Contains(character))
            {
                _activeCharactersPool.Add(character);
            }

            character.gameObject.SetActive(true);
            character.Initialize();

            if (characterType == CharacterType.DefaultPlayer)
            {
                Player = character;
            }

            return character;
        }

        public void ReturnCharacterToPool(Character character)
        {
            if (character == null)
                return;

            CleanupCharacterBeforePooling(character);

            _activeCharactersPool.Remove(character);

            CharacterType characterType = character.CharacterType;

            if (!_disabledCharactersPool.ContainsKey(characterType))
            {
                _disabledCharactersPool.Add(characterType, new Queue<Character>());
            }

            character.gameObject.SetActive(false);
            _disabledCharactersPool[characterType].Enqueue(character);

            if (Player == character)
            {
                Player = null;
            }
        }

        public void ClearAll()
        {
            for (int i = _activeCharactersPool.Count - 1; i >= 0; i--)
            {
                Character character = _activeCharactersPool[i];

                if (character == null)
                {
                    _activeCharactersPool.RemoveAt(i);
                    continue;
                }

                CleanupCharacterBeforePooling(character);

                CharacterType characterType = character.CharacterType;

                if (!_disabledCharactersPool.ContainsKey(characterType))
                {
                    _disabledCharactersPool.Add(characterType, new Queue<Character>());
                }

                _activeCharactersPool.RemoveAt(i);
                character.gameObject.SetActive(false);
                _disabledCharactersPool[characterType].Enqueue(character);
            }

            Player = null;
        }

        private Character GetFromPool(CharacterType characterType)
        {
            if (!_disabledCharactersPool.ContainsKey(characterType))
            {
                _disabledCharactersPool.Add(characterType, new Queue<Character>());
                return null;
            }

            Queue<Character> pool = _disabledCharactersPool[characterType];

            while (pool.Count > 0)
            {
                Character character = pool.Dequeue();

                if (character != null)
                    return character;
            }

            return null;
        }

        private Character InstantiateCharacter(CharacterType characterType)
        {
            Character prefab = GetPrefabByType(characterType);

            if (prefab == null)
            {
                Debug.LogError($"CharacterFactory: prefab is not assigned for type {characterType}.");
                return null;
            }

            Character characterObject = Instantiate(prefab, null);
            return characterObject;
        }

        private Character GetPrefabByType(CharacterType characterType)
        {
            switch (characterType)
            {
                case CharacterType.DefaultPlayer:
                    return _playerCharacterPrefab;

                case CharacterType.DefaultEnemy:
                    return _enemyCharacterPrefab;

                case CharacterType.FastEnemy:
                    return _fastEnemyCharacterPrefab;

                case CharacterType.TankEnemy:
                    return _tankEnemyCharacterPrefab;

                case CharacterType.Boss_Null_Core:
                    return _bossNullCorePrefab;
            }

            Debug.LogError($"CharacterFactory: unknown character type {characterType}.");
            return null;
        }

        private void CleanupCharacterBeforePooling(Character character)
        {
            if (character == null)
                return;

            if (character is PlayerCharacter playerCharacter)
            {
                playerCharacter.ClearAbilities();
            }
        }
    }
}