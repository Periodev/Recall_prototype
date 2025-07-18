namespace RecallCore.Actions
{
    public class AttackAction : IAction
    {
        public string Name => "Attack";
        public int Cost => 2;

        public void Execute(RecallCore.Entities.Player self, RecallCore.Entities.Player target)
        {
            int damage = 5;
            target.HP -= damage;
        }
    }
} 