using RecallCore.Entities;

namespace RecallCore.Actions
{
    public class ChargeAction : IAction
    {
        public string Name => "Charge";
        public int Cost => 1;
        public int ChargeConsumption => 0; // Charge 不消耗蓄力等級，而是增加蓄力等級
        
        public void Execute(Actor self, Actor target)
        {
            // Charge 增加蓄力等級
            self.ChargeLevel += GameConstants.BASE_CHARGE_VALUE;
            self.IsCharged = true; // 保持向後相容性
        }
    }
} 