using RecallCore.Entities;

namespace RecallCore.Actions
{
    public class AttackAction : IAction
    {
        public string Name => "Attack";
        public int Cost => 1;
        public int ChargeConsumption => 1; // 預設消耗 1 點蓄力等級
        
        public void Execute(Actor self, Actor target)
        {
            int damage = GameConstants.BASE_ATTACK_DAMAGE;
            
            // 重擊機制：使用新的蓄力管理方法
            if (self.HasCharge(ChargeConsumption))
            {
                damage += ChargeConsumption * GameConstants.HEAVY_STRIKE_BONUS;
                self.ConsumeCharge(ChargeConsumption);
            }
            
            target.TakeDamage(damage);
        }
    }
} 