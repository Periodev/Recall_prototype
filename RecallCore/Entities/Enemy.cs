using RecallCore.Actions;
using RecallCore.AI;

namespace RecallCore.Entities
{
    public class Enemy : Actor, IAgent
    {
        public IAction? AnnouncedAction { get; private set; }
        public IAIStrategy aiStrategy { get; private set; }

        public Enemy(string name, int hp, int ap = GameConstants.ENEMY_INITIAL_AP, IAIStrategy? strategy = null) : base(name, hp, ap) 
        {
            aiStrategy = strategy ?? new BasicAIStrategy();
        }

        public override bool CanAct()
        {
            return HP > 0 && ActionPoints > 0;
        }

        public override int GetMaxHP()
        {
            // 假設初始 HP 就是最大 HP
            return HP;
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

        public override void EndTurn()
        {
            base.EndTurn(); // 呼叫基類的 EndTurn() 來清空護盾
        }

        public bool IsDead()
        {
            return HP <= 0;
        }

        public void ExecuteAnnouncedAction(Actor target)
        {
            var action = AnnouncedAction;
            if (action == null)
                return; // 或記錄警告

            if (ActionPoints < action.Cost)
                return; // 或記錄警告

            action.Execute(this, target);
            ActionPoints -= action.Cost;
            AnnouncedAction = null;
        }

        public string GetAnnouncedActionName()
        {
            return AnnouncedAction?.Name ?? "None";
        }
    }
}
