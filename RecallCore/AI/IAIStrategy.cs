using RecallCore.Actions;
using RecallCore.Entities;

namespace RecallCore.AI
{
    public interface IAIStrategy
    {
        IAction ChooseAction(Actor self, Actor target);
        string GetStrategyName();
    }
} 