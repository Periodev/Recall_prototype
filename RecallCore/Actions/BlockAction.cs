using RecallCore.Entities;

namespace RecallCore.Actions
{
    public class BlockAction : IAction
    {
        public string Name => "Block";
        public int Cost => 1;

        public void Execute(Actor self, Actor target)
        {
            self.Block();
        }
    }
} 