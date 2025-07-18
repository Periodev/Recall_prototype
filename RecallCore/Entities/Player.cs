namespace RecallCore.Entities
{
    public class Player : Actor
    {
        public bool IsBlocking { get; set; } = false;
        public int AP => Energy;
        public Player(string name, int hp) : base(name, hp) { }
    }
}
