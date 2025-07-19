using RecallCore.Entities;

namespace RecallCore.Actions
{
    public class HeavyStrikeAction : IAction
    {
        public string Name => "HeavyStrike";
        public int Cost => 2; // 更高成本
        public int ChargeConsumption => 3; // 消耗 3 點蓄力等級
        
        public void Execute(Actor self, Actor target)
        {
            int damage = GameConstants.BASE_ATTACK_DAMAGE;
            
            // 強力攻擊：消耗多層蓄力來造成更高傷害
            if (self.ChargeLevel > 0)
            {
                int consumed = Math.Min(self.ChargeLevel, ChargeConsumption);
                damage += consumed * GameConstants.HEAVY_STRIKE_BONUS;
                self.ChargeLevel -= consumed;
                self.IsCharged = self.ChargeLevel > 0;
            }
            
            target.TakeDamage(damage);
        }
    }
} 