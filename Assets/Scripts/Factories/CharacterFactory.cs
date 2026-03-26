using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OmniumLessons
{
    public class CharacterFactory : MonoBehaviour
    {
        [SerializeField] private Character _playerCharacterPrefab;
        [SerializeField] private Character _enemyCharacterPrefab;
        [SerializeField] private Character _fastEnemyCharacterPrefab;
        [SerializeField] private Character _tankEnemyCharacterPrefab;
        
        private Dictionary<CharacterType, Queue<Character>> _disabledCharactersPool = new Dictionary<CharacterType, Queue<Character>>();
        
        private List<Character> _activeCharactersPool = new List<Character>(10);
        
        public Character Player { get; private set; }
        public List<Character> ActiveCharacters => _activeCharactersPool;
        
        public Character CreateCharacter(CharacterType characterType)
        {
            Character character = GetFromPool(characterType);

            if (character == null) 
            {
                character = InstantiateCharacter(characterType);
            }
        
            _activeCharactersPool.Add(character);
            
            character.Initialize();
            
            if (characterType == CharacterType.DefaultPlayer)
                Player = character;
        
            return character;
        }

        public void ReturnCharacterToPool(Character character)
        {
            if (character == null)
                return;

            if (character is PlayerCharacter playerCharacter)
            {
                playerCharacter.ClearAbilities();
            }

            _activeCharactersPool.Remove(character);
            var characterType = character.CharacterType;
            _disabledCharactersPool[characterType].Enqueue(character);
        }

        private Character GetFromPool(CharacterType characterType)
        {
            if (!_disabledCharactersPool.ContainsKey(characterType))
            {
                _disabledCharactersPool.Add(characterType, new Queue<Character>());
                return null;
            }
        
            if (_disabledCharactersPool[characterType].Count > 0)
            {
                return _disabledCharactersPool[characterType].Dequeue();
            }
            
            return null;
        }
        
        private Character InstantiateCharacter(CharacterType characterType)
        {
            Character characterObject = null;

            switch (characterType)
            {
                case CharacterType.DefaultPlayer:
                    characterObject = GameObject.Instantiate(_playerCharacterPrefab, null);
                    break;
                case CharacterType.DefaultEnemy:
                    characterObject = GameObject.Instantiate(_enemyCharacterPrefab, null);
                    break;
                case CharacterType.FastEnemy:
                    characterObject = GameObject.Instantiate(_fastEnemyCharacterPrefab, null);
                    break;
                case CharacterType.TankEnemy:
                    characterObject = GameObject.Instantiate(_tankEnemyCharacterPrefab, null);
                    break;
                default:
                    Debug.LogError("Unknown character type: " + characterType);
                    break;
            }

            return characterObject;
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

                if (character is PlayerCharacter playerCharacter)
                {
                    playerCharacter.ClearAbilities();
                }

                var type = character.CharacterType;

                if (!_disabledCharactersPool.ContainsKey(type))
                    _disabledCharactersPool.Add(type, new Queue<Character>());

                _activeCharactersPool.RemoveAt(i);
                _disabledCharactersPool[type].Enqueue(character);
                character.gameObject.SetActive(false);
            }

            Player = null;
        }

    }
}