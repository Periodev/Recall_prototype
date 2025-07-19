using RecallCore.Actions;
using RecallCore.Entities;

namespace RecallCore.AI
{
    public class FixedSequenceAIStrategy : IAIStrategy
    {
        private readonly IAction[] actionSequence;
        private int currentIndex = 0;

        public FixedSequenceAIStrategy(params IAction[] actions)
        {
            if (actions == null || actions.Length == 0)
            {
                throw new ArgumentException("Action sequence cannot be null or empty");
            }
            actionSequence = actions;
        }

        public IAction ChooseAction(Actor self, Actor target)
        {
            var action = actionSequence[currentIndex];
            currentIndex = (currentIndex + 1) % actionSequence.Length;
            return action;
        }

        public string GetStrategyName()
        {
            return $"Fixed Sequence AI ({string.Join(" -> ", actionSequence.Select(a => a.Name))})";
        }

        // 提供當前序列位置資訊（用於測試和調試）
        public int GetCurrentIndex() => currentIndex;
        public int GetSequenceLength() => actionSequence.Length;
        public string GetCurrentActionName() => actionSequence[currentIndex].Name;
    }
} 