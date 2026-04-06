namespace OmniumLessons {
    public interface IAttackComponent {
      

        public void Initialize(CharacterData characterData);

        public void MakeDamage(Character damageTarget);

        public void OnUpdate();
    }
}