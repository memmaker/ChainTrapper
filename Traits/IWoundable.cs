namespace ChainTrapper.Traits
{
    public interface IWoundable
    {
        public void TakeDamage(int damage);
        public bool IsDead { get; }
        public int CurrentHealth { get; }
    }
}