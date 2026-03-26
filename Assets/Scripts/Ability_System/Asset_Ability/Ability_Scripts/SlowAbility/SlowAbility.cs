using System.Collections.Generic;
using UnityEngine;

namespace OmniumLessons
{
    public class SlowAbility : AbilityBehaviour
    {
        [Header("Visual")]
        [SerializeField] private Transform _visualZone;

        private SlowAbilityData _slowAbilityData;
        private readonly Dictionary<Character, float> _slowedEnemies = new Dictionary<Character, float>();

        public override void Initialize(Character owner, AbilityData data)
        {
            base.Initialize(owner, data);

            _slowAbilityData = data as SlowAbilityData;

            if (_slowAbilityData == null)
            {
                Debug.LogError("SlowAbility: data is not SlowAbilityData");
                return;
            }

            transform.position = _owner.transform.position;
            UpdateVisualScale();
        }

        public override void OnUpdate()
        {
            if (_owner == null || _slowAbilityData == null)
            {
                return;
            }

            transform.position = _owner.transform.position;

            ApplySlowToEnemiesInRadius();
            RemoveSlowFromEnemiesOutsideRadius();
        }

        private void ApplySlowToEnemiesInRadius()
        {
            Collider[] colliders = Physics.OverlapSphere(_owner.transform.position, _slowAbilityData.Radius);

            for (int i = 0; i < colliders.Length; i++)
            {
                Character target = colliders[i].GetComponentInParent<Character>();

                if (target == null)
                {
                    continue;
                }

                if (target == _owner)
                {
                    continue;
                }

                if (target.CharacterType == CharacterType.DefaultPlayer)
                {
                    continue;
                }

                if (target.LiveComponent == null || target.LiveComponent.IsAlive == false)
                {
                    continue;
                }

                if (target.MovableComponent == null)
                {
                    continue;
                }

                if (_slowedEnemies.ContainsKey(target))
                {
                    continue;
                }

                float originalSpeed = target.MovableComponent.Speed;
                float slowedSpeed = Mathf.Max(0.1f, originalSpeed * (1f - _slowAbilityData.SlowPercent));

                target.MovableComponent.Speed = slowedSpeed;
                _slowedEnemies.Add(target, originalSpeed);
            }
        }

        private void RemoveSlowFromEnemiesOutsideRadius()
        {
            List<Character> enemiesToRestore = new List<Character>();

            foreach (KeyValuePair<Character, float> pair in _slowedEnemies)
            {
                Character enemy = pair.Key;

                if (enemy == null)
                {
                    enemiesToRestore.Add(enemy);
                    continue;
                }

                if (enemy.MovableComponent == null)
                {
                    enemiesToRestore.Add(enemy);
                    continue;
                }

                if (enemy.LiveComponent == null || enemy.LiveComponent.IsAlive == false)
                {
                    enemy.MovableComponent.Speed = pair.Value;
                    enemiesToRestore.Add(enemy);
                    continue;
                }

                float distance = Vector3.Distance(_owner.transform.position, enemy.transform.position);

                if (distance > _slowAbilityData.Radius)
                {
                    enemy.MovableComponent.Speed = pair.Value;
                    enemiesToRestore.Add(enemy);
                }
            }

            for (int i = 0; i < enemiesToRestore.Count; i++)
            {
                _slowedEnemies.Remove(enemiesToRestore[i]);
            }
        }

        private void UpdateVisualScale()
        {
            if (_visualZone == null)
            {
                return;
            }

            float diameter = _slowAbilityData.Radius * 2f;
            _visualZone.localScale = new Vector3(diameter, 0.05f, diameter);
        }

        private void OnDestroy()
        {
            RestoreAllSpeeds();
        }

        private void RestoreAllSpeeds()
        {
            foreach (KeyValuePair<Character, float> pair in _slowedEnemies)
            {
                Character enemy = pair.Key;

                if (enemy == null)
                {
                    continue;
                }

                if (enemy.MovableComponent == null)
                {
                    continue;
                }

                enemy.MovableComponent.Speed = pair.Value;
            }

            _slowedEnemies.Clear();
        }
    }
}