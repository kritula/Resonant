using System;
using UnityEngine;

namespace OmniumLessons
{
    [Serializable]
    public class WaveEnemyEntry
    {
        [SerializeField] private CharacterType _characterType;
        [SerializeField] private int _weight = 1;

        public CharacterType CharacterType => _characterType;
        public int Weight => Mathf.Max(1, _weight);
    }
}