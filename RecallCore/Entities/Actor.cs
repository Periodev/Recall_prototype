namespace RecallCore.Entities
{
    public abstract class Actor
    {
        public string Name { get; }
        public int HP { get; set; }
        public int ActionPoints { get; set; }
        public bool IsBlocking { get; set; } = false;
        public bool IsCharged { get; set; } = false;
        protected int InitialAP { get; }

        protected Actor(string name, int hp, int initialAP)
        {
            Name = name;
            HP = hp;
            InitialAP = initialAP;
            ActionPoints = initialAP;
        }

        public void ResetAP()
        {
            ActionPoints = InitialAP;
            // IsCharged 不重置，可以跨回合保留
        }

        public void TakeDamage(int dmg)
        {
            // 直接扣除傷害（Block 減傷已在 AttackAction 中處理）
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