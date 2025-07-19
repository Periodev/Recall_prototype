using RecallCore.Actions;
using RecallCore.Entities;

namespace RecallCore.AI
{
    public class RandomAIStrategy : IAIStrategy
    {
        private static readonly Random rng = new();

        public IAction ChooseAction(Actor self, Actor target)
        {
            // 完全隨機選擇行動
            var actions = new IAction[] { new AttackAction(), new BlockAction(), new ChargeAction() };
            return actions[rng.Next(actions.Length)];
        }

        public string GetStrategyName()
        {
            return "Random AI";
        }
    }
} 