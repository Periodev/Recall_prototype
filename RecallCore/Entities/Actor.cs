namespace RecallCore.Entities
{
    public abstract class Actor
    {
        public string Name { get; }
        public int HP { get; set; }
        public int ActionPoints { get; set; }
        public bool IsBlocking { get; set; } = false;
        public bool IsCharged { get; set; } = false;
        public int CurrentShield { get; set; } = 0; // 新增護盾屬性
        public int ChargeLevel { get; set; } = 0; // 新增蓄力等級屬性
        protected int InitialAP { get; }

        protected Actor(string name, int hp, int initialAP)
        {
            Name = name;
            HP = hp;
            InitialAP = initialAP;
            ActionPoints = initialAP;
            CurrentShield = 0; // 初始化護盾為 0
            ChargeLevel = 0; // 初始化蓄力等級為 0
        }

        public void ResetAP()
        {
            ActionPoints = InitialAP;
            // IsCharged 不重置，可以跨回合保留
        }

        public void TakeDamage(int dmg)
        {
            // 邊界檢查：防止負數或零傷害
            if (dmg <= 0) return;
            
            // 護盾機制：先從護盾扣除傷害
            if (CurrentShield > 0)
            {
                if (CurrentShield >= dmg)
                {
                    // 護盾足夠抵擋全部傷害
                    CurrentShield -= dmg;
                    dmg = 0;
                }
                else
                {
                    // 護盾不足，扣除護盾後剩餘傷害扣 HP
                    dmg -= CurrentShield;
                    CurrentShield = 0;
                }
            }
            
            // 剩餘傷害扣 HP
            if (dmg > 0)
            {
                HP = Math.Max(0, HP - dmg);
            }
        }

        public void AddEnergy(int amount)
        {
            ActionPoints += amount;
        }

        public virtual void Block()
        {
            AddShield(GameConstants.BLOCK_SHIELD_VALUE);
            IsBlocking = true;
        }

        public void AddShield(int amount)
        {
            // 邊界檢查：防止負數護盾
            if (amount < 0) return;
            CurrentShield += amount;
        }

        public void ClearShield()
        {
            CurrentShield = 0;
        }

        public virtual void EndTurn()
        {
            ResetAP();
            ClearShield(); // 回合結束時清空護盾
            // ChargeLevel 不重置，可以跨回合保留
        }

        public abstract bool CanAct();
        public abstract int GetMaxHP();
    }
} 