namespace RecallCore.Entities
{
    public class BlockAction : IAction
    {
        public string Name => "Block";
        public int Cost => 1;

        public void Execute(Player self, Player target)
        {
            self.IsBlocking = true;
        }
    }
} 