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
            
            // 移除舊的 Block 減傷邏輯，現在使用護盾機制
            // 護盾減傷在 target.TakeDamage() 中處理
            
            target.TakeDamage(damage);
        }
    }
} 