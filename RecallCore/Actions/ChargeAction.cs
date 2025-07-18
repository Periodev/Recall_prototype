using RecallCore.Entities;

namespace RecallCore.Actions
{
    public class ChargeAction : IAction
    {
        public string Name => "Charge";
        public int Cost => 1;

        public void Execute(Actor self, Actor target)
        {
            self.ActionPoints += 2;
        }
    }
} 