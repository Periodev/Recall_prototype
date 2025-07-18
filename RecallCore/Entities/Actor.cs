namespace RecallCore.Entities
{
    public abstract class Actor
    {
        public string Name { get; }
        public int HP { get; protected set; }
        public int Energy { get; protected set; }

        protected Actor(string name, int hp)
        {
            Name = name;
            HP = hp;
            Energy = 0;
        }

        public void TakeDamage(int dmg) => HP = Math.Max(0, HP - dmg);

        public void AddEnergy(int amount) => Energy += amount;

        public virtual void Block() => Console.WriteLine($"{Name} is blocking!");
    }
}
