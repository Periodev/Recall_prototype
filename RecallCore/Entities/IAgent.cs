using RecallCore.Actions;

namespace RecallCore.Entities
{
    public interface IAgent
    {
        void TakeTurn();
        IAction ChooseAction();
        void EndTurn();
        bool IsDead();
    }
} 