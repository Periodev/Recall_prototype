using RecallCore.Entities;

namespace RecallCore.Actions
{
    public class AttackAction : IAction
    {
        public string Name => "Attack";
        public int Cost => 1;

        public void Execute(Actor self, Actor target)
        {
            int damage = 6;
            
            // 如果處於 Charged 狀態，傷害翻倍
            if (self.IsCharged)
            {
                damage *= 2;
                self.IsCharged = false; // 使用後重置
            }
            
            // 檢查目標是否正在 Blocking
            if (target.IsBlocking)
            {
                // 如果目標正在 Blocking，減少傷害
                damage = Math.Max(0, damage - 3);
                target.IsBlocking = false; // 使用後重置
            }
            
            target.TakeDamage(damage);
        }
    }
} 