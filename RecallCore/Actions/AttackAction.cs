using RecallCore.Entities;

namespace RecallCore.Actions
{
    public class AttackAction : IAction
    {
        public string Name => "Attack";
        public int Cost => 1;

        public void Execute(Actor self, Actor target)
        {
            int damage = GameConstants.BASE_ATTACK_DAMAGE;
            
            // 重擊機制：如果有蓄力等級，消耗蓄力等級來增加傷害
            if (self.ChargeLevel > 0)
            {
                damage += self.ChargeLevel * GameConstants.HEAVY_STRIKE_BONUS;
                self.ChargeLevel = 0; // 消耗所有蓄力等級
                self.IsCharged = false; // 保持向後相容性
            }
            
            target.TakeDamage(damage);
        }
    }
} 