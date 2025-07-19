using RecallCore.Entities;

namespace RecallCore.Actions
{
    public class ChargeAction : IAction
    {
        public string Name => "Charge";
        public int Cost => 1;

        public void Execute(Actor self, Actor target)
        {
            // Charge 為下一個 Attack 提供強化效果
            self.IsCharged = true;
        }
    }
} 