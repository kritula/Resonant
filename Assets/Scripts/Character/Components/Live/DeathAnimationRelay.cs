using UnityEngine;

namespace OmniumLessons
{
    public class DeathAnimationRelay : MonoBehaviour
    {
        [SerializeField] private PlayerCharacter _playerCharacter;
        [SerializeField] private EnemyCharacter _enemyCharacter;

        private void Reset()
        {
            if (_playerCharacter == null)
                _playerCharacter = GetComponentInParent<PlayerCharacter>();

            if (_enemyCharacter == null)
                _enemyCharacter = GetComponentInParent<EnemyCharacter>();
        }

        public void OnDeathAnimationFinished()
        {
            if (_playerCharacter != null)
            {
                _playerCharacter.OnDeathAnimationFinished();
                return;
            }

            if (_enemyCharacter != null)
            {
                _enemyCharacter.OnDeathAnimationFinished();
            }
        }
    }
}