namespace RecallCore.Entities
{
    public class Enemy : Actor
    {
        private static readonly Random rng = new();

        public Enemy(string name, int hp) : base(name, hp) { }

        public string DecideAction()
        {
            return rng.Next(0, 2) == 0 ? "Attack" : "Idle";
        }
    }
}
