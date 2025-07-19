using RecallCore.Entities;

namespace RecallCore.Actions
{
    public class HeavyStrikeAction : IAction
    {
        public string Name => "HeavyStrike";
        public int Cost => 2; // 更高成本
        public int ChargeConsumption => 1; // 消耗 1 點蓄力等級，讓它更實用
        
        public void Execute(Actor self, Actor target)
        {
            int damage = GameConstants.BASE_ATTACK_DAMAGE;
            
            // 強力攻擊：使用新的蓄力管理方法
            if (self.HasCharge(ChargeConsumption))
            {
                damage += ChargeConsumption * GameConstants.HEAVY_STRIKE_BONUS;
                self.ConsumeCharge(ChargeConsumption);
            }
            
            target.TakeDamage(damage);
        }
    }
} 