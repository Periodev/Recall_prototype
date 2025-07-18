using RecallCore.Entities;

namespace RecallCore.Actions
{
    public interface IAction
    {
        string Name { get; }
        int Cost { get; }
        void Execute(Actor self, Actor target);
    }
} 