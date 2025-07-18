namespace RecallCore.Entities
{
    public interface IAction
    {
        string Name { get; }
        int Cost { get; }
        void Execute(Player self, Player target);
    }
} 