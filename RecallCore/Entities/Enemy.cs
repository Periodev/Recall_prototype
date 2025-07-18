namespace RecallCore.Entities
{
    public class Enemy : Actor
    {
        private static readonly Random rng = new();

        public Enemy(string name, int hp) : base(name, hp) { }

        public string DecideAction()
        {
            var actions = new[] { "Attack", "Block", "Charge" };
            return actions[rng.Next(actions.Length)];
        }
    }
}
