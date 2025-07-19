using System;
using System.Collections.Generic;
using System.Linq;
using RecallCore.Entities;
using RecallCore.Actions;

namespace RecallCore.Memory
{
    public class EchoExecutor
    {
        public static EchoExecutionResult ExecuteEchoCard(EchoCard card, Actor user, Actor? target = null)
        {
            var result = new EchoExecutionResult();
            
            // 統計 a, b, c
            int attackCount = 0, blockCount = 0, chargeCount = 0;
            
            foreach (var action in card.Actions)
            {
                switch (action.ActionType)
                {
                    case ActionType.Attack:
                        attackCount++;
                        break;
                    case ActionType.Block:
                        blockCount++;
                        break;
                    case ActionType.Charge:
                        chargeCount++;
                        break;
                }
            }
            
            // 目標選擇：如果有攻擊，需要選擇目標
            if (attackCount > 0 && target == null)
            {
                // 在 Console 版本中，目標通常是敵人
                // 這裡可以擴展為更複雜的目標選擇邏輯
                throw new InvalidOperationException("Target required for Echo with attacks");
            }
            
            // 記錄初始狀態
            int initialUserHP = user.HP;
            int initialTargetHP = target?.HP ?? 0;
            int initialShield = user.CurrentShield;
            int initialCharge = user.ChargeLevel;
            
            // 計算重擊與剩餘
            int heavyStrikes = Math.Min(attackCount, chargeCount);
            int unpairedA = attackCount - heavyStrikes;
            int unpairedC = chargeCount - heavyStrikes;
            
            // 使用動作系統來執行效果
            var attackAction = new AttackAction();
            var blockAction = new BlockAction();
            var chargeAction = new ChargeAction();
            
            // 執行重擊（配對的攻擊和蓄力）
            for (int i = 0; i < heavyStrikes; i++)
            {
                if (target != null)
                {
                    attackAction.Execute(user, target);
                }
            }
            
            // 執行普通攻擊（未配對的攻擊）
            for (int i = 0; i < unpairedA; i++)
            {
                if (target != null)
                {
                    attackAction.Execute(user, target);
                }
            }
            
            // 執行防禦（所有 Block 動作）
            for (int i = 0; i < blockCount; i++)
            {
                blockAction.Execute(user, target);
            }
            
            // 執行蓄力（未配對的蓄力）
            for (int i = 0; i < unpairedC; i++)
            {
                chargeAction.Execute(user, target);
            }
            
            // 計算結果
            if (target != null)
            {
                result.HeavyStrikeDamage = heavyStrikes * (GameConstants.BASE_ATTACK_DAMAGE + GameConstants.HEAVY_STRIKE_BONUS);
                result.HeavyStrikeCount = heavyStrikes;
                result.NormalDamage = unpairedA * GameConstants.BASE_ATTACK_DAMAGE;
                result.NormalAttackCount = unpairedA;
            }
            
            result.ShieldGain = user.CurrentShield - initialShield;
            result.BlockCount = blockCount;
            result.ChargeGain = user.ChargeLevel - initialCharge;
            result.ChargeCount = unpairedC;
            
            return result;
        }
    }
    
    public class EchoExecutionResult
    {
        public int HeavyStrikeDamage { get; set; } = 0;
        public int HeavyStrikeCount { get; set; } = 0;
        public int NormalDamage { get; set; } = 0;
        public int NormalAttackCount { get; set; } = 0;
        public int ShieldGain { get; set; } = 0;
        public int BlockCount { get; set; } = 0;
        public int ChargeGain { get; set; } = 0;
        public int ChargeCount { get; set; } = 0;
        
        public bool HasAnyEffect => HeavyStrikeDamage > 0 || NormalDamage > 0 || ShieldGain > 0 || ChargeGain > 0;
        
        public string GetSummary()
        {
            var effects = new List<string>();
            
            if (HeavyStrikeDamage > 0)
                effects.Add($"+{HeavyStrikeDamage} 重擊傷害 ({HeavyStrikeCount}次)");
            
            if (NormalDamage > 0)
                effects.Add($"+{NormalDamage} 普通傷害 ({NormalAttackCount}次)");
            
            if (ShieldGain > 0)
                effects.Add($"+{ShieldGain} 護盾 ({BlockCount}次)");
            
            if (ChargeGain > 0)
                effects.Add($"+{ChargeGain} 蓄力 ({ChargeCount}次)");
            
            return string.Join(" | ", effects);
        }
    }
} 