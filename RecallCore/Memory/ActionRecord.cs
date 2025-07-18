namespace RecallCore.Memory
{
    public class ActionRecord
    {
        public string ActorName { get; }
        public string ActionName { get; }
        public int Turn { get; }

        public ActionRecord(string actorName, string actionName, int turn)
        {
            ActorName = actorName;
            ActionName = actionName;
            Turn = turn;
        }
    }
} 