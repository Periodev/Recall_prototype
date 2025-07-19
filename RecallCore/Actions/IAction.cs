using RecallCore.Entities;

namespace RecallCore.Actions
{
    public interface IAction
    {
        string Name { get; }
        int Cost { get; }
        int ChargeConsumption { get; } // 新增：每次消耗的蓄力等級
        
        void Execute(Actor self, Actor target);
    }
} 