namespace RecallCore.Memory
{
    public class Timeline
    {
        private readonly Queue<string> actions;
        private readonly int capacity;

        public Timeline(int capacity)
        {
            this.capacity = capacity;
            actions = new Queue<string>();
        }

        public void Record(string action)
        {
            if (actions.Count >= capacity)
                actions.Dequeue();
            actions.Enqueue(action);
        }

        public string? ReplayLast()
        {
            return actions.Count > 0 ? actions.Last() : null;
        }
    }
}
