namespace RecallCore.Actions
{
    public class BlockAction : IAction
    {
        public string Name => "Block";
        public int Cost => 1;

        public void Execute(RecallCore.Entities.Player self, RecallCore.Entities.Player target)
        {
            self.IsBlocking = true;
        }
    }
} 