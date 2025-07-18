namespace RecallCore.Entities
{
    public abstract class Actor
    {
        public string Name { get; }
        public int HP { get; set; }
        public int ActionPoints { get; set; }
        public bool IsBlocking { get; set; } = false;

        protected Actor(string name, int hp)
        {
            Name = name;
            HP = hp;
            ActionPoints = 2; // 預設初始 AP
        }

        public void ResetAP()
        {
            ActionPoints = 2;
        }

        public void TakeDamage(int dmg)
        {
            HP = Math.Max(0, HP - dmg);
        }

        public void AddEnergy(int amount)
        {
            ActionPoints += amount;
        }

        public virtual void Block()
        {
            IsBlocking = true;
        }
    }
} 