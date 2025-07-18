namespace RecallCore.Entities
{
    public class AttackAction : IAction
    {
        public string Name => "Attack";
        public int Cost => 2;

        public void Execute(Player self, Player target)
        {
            int damage = 5;
            target.HP -= damage;
        }
    }
} 