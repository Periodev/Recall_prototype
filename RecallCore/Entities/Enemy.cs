using RecallCore.Actions;
using RecallCore.AI;

namespace RecallCore.Entities
{
    public class Enemy : Actor, IAgent
    {
        public IAction? AnnouncedAction { get; private set; }
        public IAIStrategy aiStrategy { get; private set; }

        public Enemy(string name, int hp, int ap = 1, IAIStrategy? strategy = null) : base(name, hp, ap) 
        {
            aiStrategy = strategy ?? new BasicAIStrategy();
        }

        public void AnnounceAction()
        {
            AnnouncedAction = ChooseAction(); // 使用 AI 策略選擇
        }

        public IAction ChooseAction()
        {
            // 委託給 AI 策略
            return aiStrategy.ChooseAction(this, null);
        }

        public void TakeTurn()
        {
            // 這個方法需要目標參數，所以保持原有的 ExecuteAnnouncedAction
            // 在實際使用時會呼叫 ExecuteAnnouncedAction(target)
        }

        public void EndTurn()
        {
            ResetAP();
        }

        public bool IsDead()
        {
            return HP <= 0;
        }

        public void ExecuteAnnouncedAction(Actor target)
        {
            if (AnnouncedAction != null && ActionPoints >= AnnouncedAction.Cost)
            {
                AnnouncedAction.Execute(this, target);
                ActionPoints -= AnnouncedAction.Cost;
                AnnouncedAction = null; // 清除預告
            }
        }

        public string GetAnnouncedActionName()
        {
            return AnnouncedAction?.Name ?? "None";
        }
    }
}
