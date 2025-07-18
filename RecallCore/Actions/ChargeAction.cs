namespace RecallCore.Actions
{
    public class ChargeAction : IAction
    {
        public string Name => "Charge";
        public int Cost => 1;

        public void Execute(RecallCore.Entities.Player self, RecallCore.Entities.Player target)
        {
            self.AP += 2;
        }
    }
} 