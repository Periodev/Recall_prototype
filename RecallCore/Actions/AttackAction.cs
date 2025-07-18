using RecallCore.Entities;

namespace RecallCore.Actions
{
    public class AttackAction : IAction
    {
        public string Name => "Attack";
        public int Cost => 2;

        public void Execute(Actor self, Actor target)
        {
            int damage = 5;
            target.TakeDamage(damage);
        }
    }
} 