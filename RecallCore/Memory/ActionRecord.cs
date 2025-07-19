using RecallCore.Actions;

namespace RecallCore.Memory
{
    public class ActionRecord
    {
        public string ActorName { get; }
        public string ActionName { get; }
        public ActionType ActionType { get; }
        public int Turn { get; }
        public int Step { get; }

        public ActionRecord(string actorName, string actionName, ActionType actionType, int turn, int step)
        {
            ActorName = actorName;
            ActionName = actionName;
            ActionType = actionType;
            Turn = turn;
            Step = step;
        }
    }
    
    public enum ActionType
    {
        Attack,
        Block,
        Charge,
        Recall,
        Echo
    }
} 