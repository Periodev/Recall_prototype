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
            
            // 計算重擊與剩餘
            int heavyStrikes = Math.Min(attackCount, chargeCount);
            int unpairedA = attackCount - heavyStrikes;
            int unpairedC = chargeCount - heavyStrikes;
            
            // 目標選擇：如果有攻擊，需要選擇目標
            if (attackCount > 0 && target == null)
            {
                // 在 Console 版本中，目標通常是敵人
                // 這裡可以擴展為更複雜的目標選擇邏輯
                throw new InvalidOperationException("Target required for Echo with attacks");
            }
            
            // 一次性觸發效果
            if (heavyStrikes > 0 && target != null)
            {
                int heavyDamage = heavyStrikes * (GameConstants.BASE_ATTACK_DAMAGE + GameConstants.HEAVY_STRIKE_BONUS);
                target.TakeDamage(heavyDamage);
                result.HeavyStrikeDamage = heavyDamage;
                result.HeavyStrikeCount = heavyStrikes;
            }
            
            if (unpairedA > 0 && target != null)
            {
                int normalDamage = unpairedA * GameConstants.BASE_ATTACK_DAMAGE;
                target.TakeDamage(normalDamage);
                result.NormalDamage = normalDamage;
                result.NormalAttackCount = unpairedA;
            }
            
            if (blockCount > 0)
            {
                int shieldGain = blockCount * GameConstants.BASE_SHIELD_VALUE;
                user.AddShield(shieldGain);
                result.ShieldGain = shieldGain;
                result.BlockCount = blockCount;
            }
            
            if (unpairedC > 0)
            {
                int chargeGain = unpairedC * GameConstants.BASE_CHARGE_VALUE;
                user.ChargeLevel += chargeGain;
                result.ChargeGain = chargeGain;
                result.ChargeCount = unpairedC;
            }
            
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