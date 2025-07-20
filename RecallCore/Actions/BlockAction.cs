using RecallCore.Entities;

namespace RecallCore.Actions
{
    public class BlockAction : IAction
    {
        public string Name => "Block";
        public int Cost => 1;
        public int ChargeConsumption => 0; // Block 不消耗蓄力等級
        
        public void Execute(Actor self, Actor target)
        {
            self.AddShield(GameConstants.BLOCK_SHIELD_VALUE);
        }
    }
} 