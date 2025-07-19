using RecallCore.Actions;
using RecallCore.Entities;

namespace RecallCore.AI
{
    public class BasicAIStrategy : IAIStrategy
    {
        private static readonly Random rng = new();
        private const int LOW_HP_THRESHOLD = 10;
        private const int CRITICAL_HP_THRESHOLD = 5;

        public IAction ChooseAction(Actor self, Actor target)
        {
            // 智能 AI 決策邏輯
            if (self.HP < CRITICAL_HP_THRESHOLD) return new BlockAction();
            if (self.HP < LOW_HP_THRESHOLD && self.ActionPoints >= 1) return new BlockAction();
            
            // 考慮 Charge + Attack 策略
            if (self.ActionPoints >= 1)
            {
                // 如果已經 Charged，優先使用 Attack 來發揮強化效果
                if (self.IsCharged) return new AttackAction();
                
                // 如果 HP 健康且 AP 足夠，可以考慮 Charge 來強化下一個 Attack
                if (self.HP >= LOW_HP_THRESHOLD + 5) // 非常健康時
                {
                    // 隨機選擇 Charge 或 Attack
                    return rng.Next(2) == 0 ? new ChargeAction() : new AttackAction();
                }
                return new AttackAction();
            }
            
            return new BlockAction(); // 預設防禦
        }

        public string GetStrategyName()
        {
            return "Basic AI";
        }
    }
} 