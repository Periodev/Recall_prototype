namespace RecallCore.Entities
{
    public class ChargeAction : IAction
    {
        public string Name => "Charge";
        public int Cost => 1;

        public void Execute(Player self, Player target)
        {
            self.AP += 2;
        }
    }
} 