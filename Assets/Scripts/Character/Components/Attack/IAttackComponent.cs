namespace OmniumLessons {
    public interface IAttackComponent {
      

        public void Initialize(CharacterData characterData);

        public bool MakeDamage(Character damageTarget);

        public void OnUpdate();
    }
}