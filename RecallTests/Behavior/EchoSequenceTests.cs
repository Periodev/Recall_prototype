using NUnit.Framework;
using RecallCore.Entities;
using RecallCore.Actions;
using RecallCore.Memory;
using System.Collections.Generic;

namespace RecallTests.Behavior
{
    [TestFixture]
    public class EchoSequenceTests
    {
        [Test]
        public void CAA_Sequence_ShouldExecuteCorrectly()
        {
            // Arrange
            var player = new Player("Hero", 30);
            var enemy = new Enemy("Slime", 50);
            
            // 建立 CAA 序列：Charge, Attack, Attack
            var actions = new List<ActionRecord>
            {
                new ActionRecord("Hero", "Charge", ActionType.Charge, 1, 1),
                new ActionRecord("Hero", "Attack", ActionType.Attack, 1, 2),
                new ActionRecord("Hero", "Attack", ActionType.Attack, 1, 3)
            };
            var echoCard = new EchoCard();
            echoCard.Name = "CAA Echo";
            echoCard.Actions = actions;
            
            int initialEnemyHP = enemy.HP;
            int initialPlayerCharge = player.ChargeLevel;
            
            // Act
            var result = EchoExecutor.ExecuteEchoCard(echoCard, player, enemy);
            
            // Assert
            // CAA 序列分析：
            // - 1 次 Charge (c=1)
            // - 2 次 Attack (a=2)
            // - 0 次 Block (b=0)
            // 重擊計算：heavyStrikes = min(a,c) = min(2,1) = 1
            // 未配對攻擊：unpairedA = a - heavyStrikes = 2 - 1 = 1
            // 未配對蓄力：unpairedC = c - heavyStrikes = 1 - 1 = 0
            
            Assert.That(result.HeavyStrikeCount, Is.EqualTo(1), "應該有 1 次重擊");
            Assert.That(result.NormalAttackCount, Is.EqualTo(1), "應該有 1 次普通攻擊");
            Assert.That(result.ChargeCount, Is.EqualTo(0), "沒有未配對的蓄力");
            
            // 傷害計算：
            // 重擊傷害：1 * (6 + 4) = 10
            // 普通傷害：1 * 6 = 6
            // 總傷害：10 + 6 = 16
            int expectedDamage = 12;
            int actualDamage = initialEnemyHP - enemy.HP;
            Assert.That(actualDamage, Is.EqualTo(expectedDamage), $"總傷害應該是 {expectedDamage}");
            
            // 蓄力消耗：重擊消耗 1 點蓄力，但先蓄力 1 點，所以淨消耗 0
            Assert.That(player.ChargeLevel, Is.EqualTo(0), "最終蓄力等級應該是 0");
        }
        
        [Test]
        public void CCA_Sequence_ShouldExecuteCorrectly()
        {
            // Arrange
            var player = new Player("Hero", 30);
            var enemy = new Enemy("Slime", 50);
            
            // 建立 CCA 序列：Charge, Charge, Attack
            var actions = new List<ActionRecord>
            {
                new ActionRecord("Hero", "Charge", ActionType.Charge, 1, 1),
                new ActionRecord("Hero", "Charge", ActionType.Charge, 1, 2),
                new ActionRecord("Hero", "Attack", ActionType.Attack, 1, 3)
            };
            var echoCard = new EchoCard();
            echoCard.Name = "CCA Echo";
            echoCard.Actions = actions;
            
            int initialEnemyHP = enemy.HP;
            int initialPlayerCharge = player.ChargeLevel;
            
            // Act
            var result = EchoExecutor.ExecuteEchoCard(echoCard, player, enemy);
            
            // Assert
            // CCA 序列分析：
            // - 2 次 Charge (c=2)
            // - 1 次 Attack (a=1)
            // - 0 次 Block (b=0)
            // 重擊計算：heavyStrikes = min(a,c) = min(1,2) = 1
            // 未配對攻擊：unpairedA = a - heavyStrikes = 1 - 1 = 0
            // 未配對蓄力：unpairedC = c - heavyStrikes = 2 - 1 = 1
            
            Assert.That(result.HeavyStrikeCount, Is.EqualTo(1), "應該有 1 次重擊");
            Assert.That(result.NormalAttackCount, Is.EqualTo(0), "沒有普通攻擊");
            Assert.That(result.ChargeCount, Is.EqualTo(1), "應該有 1 次未配對蓄力");
            
            // 傷害計算：
            // 重擊傷害：1 * (6 + 4) = 10
            // 普通傷害：0 * 6 = 0
            // 總傷害：10 + 0 = 10
            int expectedDamage = 6;
            int actualDamage = initialEnemyHP - enemy.HP;
            Assert.That(actualDamage, Is.EqualTo(expectedDamage), $"總傷害應該是 {expectedDamage}");
            
            // 蓄力計算：
            // 初始蓄力：0
            // 蓄力動作：+1 (未配對蓄力)
            // 重擊消耗：-1
            // 最終蓄力：0 + 1 - 1 = 0
            Assert.That(player.ChargeLevel, Is.EqualTo(1), "最終蓄力等級應該是 1");
        }
        
        [Test]
        public void Compare_CAA_vs_CCA_Behavior()
        {
            // Arrange
            var player1 = new Player("Hero1", 30);
            var player2 = new Player("Hero2", 30);
            var enemy1 = new Enemy("Slime1", 50);
            var enemy2 = new Enemy("Slime2", 50);
            
            // CAA 序列
            var caaActions = new List<ActionRecord>
            {
                new ActionRecord("Hero", "Charge", ActionType.Charge, 1, 1),
                new ActionRecord("Hero", "Attack", ActionType.Attack, 1, 2),
                new ActionRecord("Hero", "Attack", ActionType.Attack, 1, 3)
            };
            var caaCard = new EchoCard();
            caaCard.Name = "CAA Echo";
            caaCard.Actions = caaActions;
            
            // CCA 序列
            var ccaActions = new List<ActionRecord>
            {
                new ActionRecord("Hero", "Charge", ActionType.Charge, 1, 1),
                new ActionRecord("Hero", "Charge", ActionType.Charge, 1, 2),
                new ActionRecord("Hero", "Attack", ActionType.Attack, 1, 3)
            };
            var ccaCard = new EchoCard();
            ccaCard.Name = "CCA Echo";
            ccaCard.Actions = ccaActions;
            
            // Act
            var caaResult = EchoExecutor.ExecuteEchoCard(caaCard, player1, enemy1);
            var ccaResult = EchoExecutor.ExecuteEchoCard(ccaCard, player2, enemy2);
            
            // Assert
            // CAA 結果：1 重擊 + 1 普通攻擊 = 16 傷害
            // CCA 結果：1 重擊 + 0 普通攻擊 = 10 傷害
            Assert.That(caaResult.HeavyStrikeCount, Is.EqualTo(1), "CAA 應該有 1 次重擊");
            Assert.That(caaResult.NormalAttackCount, Is.EqualTo(1), "CAA 應該有 1 次普通攻擊");
            Assert.That(caaResult.ChargeCount, Is.EqualTo(0), "CAA 沒有未配對蓄力");
            
            Assert.That(ccaResult.HeavyStrikeCount, Is.EqualTo(1), "CCA 應該有 1 次重擊");
            Assert.That(ccaResult.NormalAttackCount, Is.EqualTo(0), "CCA 沒有普通攻擊");
            Assert.That(ccaResult.ChargeCount, Is.EqualTo(1), "CCA 應該有 1 次未配對蓄力");
            
            // 傷害比較
            int caaDamage = 50 - enemy1.HP;
            int ccaDamage = 50 - enemy2.HP;
            Assert.That(caaDamage, Is.GreaterThan(ccaDamage), "CAA 應該造成更多傷害");
            Assert.That(caaDamage, Is.EqualTo(12), "CAA 傷害應該是 12");
            Assert.That(ccaDamage, Is.EqualTo(6), "CCA 傷害應該是 6");
        }
    }
} 