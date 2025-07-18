namespace RecallCore.Actions
{
    public interface IAction
    {
        string Name { get; }
        int Cost { get; }
        void Execute(RecallCore.Entities.Player self, RecallCore.Entities.Player target);
    }
} 