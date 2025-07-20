using NUnit.Framework;
using RecallCore.Entities;
using RecallCore.Actions;
using RecallCore.Memory;
using System.Collections.Generic;

namespace RecallTests.Behavior
{
    [TestFixture]
    public class EchoMultiChargeTests
    {
        [Test]
        public void EchoWithSingleAttack_ShouldConsumeOneChargeLevel()
        {
            // Arrange
            var player = new Player("Hero", 30);
            var enemy = new Enemy("Slime", 20);
            
            // 玩家先蓄力 3 次
            var chargeAction = new ChargeAction();
            chargeAction.Execute(player, enemy);
            chargeAction.Execute(player, enemy);
            chargeAction.Execute(player, enemy);
            
            // 建立包含 1 次攻擊和 1 次蓄力的 Echo 卡
            var actions = new List<ActionRecord>
            {
                new ActionRecord("Hero", "Attack", ActionType.Attack, 1, 1),
                new ActionRecord("Hero", "Charge", ActionType.Charge, 1, 2)
            };
            var echoCard = new EchoCard();
            echoCard.Name = "Test Echo";
            echoCard.Actions = actions;
            
            int initialChargeLevel = player.ChargeLevel;
            
            // Act
            var result = EchoExecutor.ExecuteEchoCard(echoCard, player, enemy);
            
            // Assert
            Assert.AreEqual(1, result.HeavyStrikeCount);
            Assert.AreEqual(10, result.HeavyStrikeDamage); // 6 + 4 = 10
            Assert.AreEqual(initialChargeLevel, player.ChargeLevel); // 蓄力等級不變 (重擊消耗了 Echo 卡內的蓄力)
        }

        [Test]
        public void EchoWithMultipleAttacks_ShouldConsumeMultipleChargeLevels()
        {
            // Arrange
            var player = new Player("Hero", 30);
            var enemy = new Enemy("Slime", 20);
            
            // 玩家先蓄力 5 次
            var chargeAction = new ChargeAction();
            for (int i = 0; i < 5; i++)
            {
                chargeAction.Execute(player, enemy);
            }
            
            // 建立包含 3 次攻擊和 3 次蓄力的 Echo 卡
            var actions = new List<ActionRecord>
            {
                new ActionRecord("Hero", "Attack", ActionType.Attack, 1, 1),
                new ActionRecord("Hero", "Charge", ActionType.Charge, 1, 2),
                new ActionRecord("Hero", "Attack", ActionType.Attack, 1, 3),
                new ActionRecord("Hero", "Charge", ActionType.Charge, 1, 4),
                new ActionRecord("Hero", "Attack", ActionType.Attack, 1, 5),
                new ActionRecord("Hero", "Charge", ActionType.Charge, 1, 6)
            };
            var echoCard = new EchoCard();
            echoCard.Name = "Test Echo";
            echoCard.Actions = actions;
            
            int initialChargeLevel = player.ChargeLevel;
            
            // Act
            var result = EchoExecutor.ExecuteEchoCard(echoCard, player, enemy);
            
            // Assert
            Assert.AreEqual(3, result.HeavyStrikeCount);
            Assert.AreEqual(30, result.HeavyStrikeDamage); // 3 * (6 + 4) = 30
            Assert.AreEqual(initialChargeLevel, player.ChargeLevel); // 蓄力等級不變 (3 重擊消耗 3 蓄力)
        }

        [Test]
        public void EchoWithInsufficientCharge_ShouldConsumeAllAvailable()
        {
            // Arrange
            var player = new Player("Hero", 30);
            var enemy = new Enemy("Slime", 20);
            
            // 玩家先蓄力 2 次
            var chargeAction = new ChargeAction();
            chargeAction.Execute(player, enemy);
            chargeAction.Execute(player, enemy);
            
            // 建立包含 4 次攻擊和 2 次蓄力的 Echo 卡
            var actions = new List<ActionRecord>
            {
                new ActionRecord("Hero", "Attack", ActionType.Attack, 1, 1),
                new ActionRecord("Hero", "Attack", ActionType.Attack, 1, 2),
                new ActionRecord("Hero", "Charge", ActionType.Charge, 1, 3),
                new ActionRecord("Hero", "Attack", ActionType.Attack, 1, 4),
                new ActionRecord("Hero", "Charge", ActionType.Charge, 1, 5),
                new ActionRecord("Hero", "Attack", ActionType.Attack, 1, 6)
            };
            var echoCard = new EchoCard();
            echoCard.Name = "Test Echo";
            echoCard.Actions = actions;
            
            int initialChargeLevel = player.ChargeLevel;
            
            // Act
            var result = EchoExecutor.ExecuteEchoCard(echoCard, player, enemy);
            
            // Assert
            Assert.AreEqual(2, result.HeavyStrikeCount); // 只能配對 2 次
            Assert.AreEqual(20, result.HeavyStrikeDamage); // 2 * (6 + 4) = 20
            Assert.AreEqual(2, result.NormalAttackCount); // 2 次普通攻擊
            Assert.AreEqual(12, result.NormalDamage); // 2 * 6 = 12
            Assert.AreEqual(initialChargeLevel, player.ChargeLevel); // 蓄力等級不變 (2 重擊消耗 2 蓄力)
        }

        [Test]
        public void EchoWithMixedActions_ShouldHandleCorrectly()
        {
            // Arrange
            var player = new Player("Hero", 30);
            var enemy = new Enemy("Slime", 20);
            
            // 玩家先蓄力 2 次
            var chargeAction = new ChargeAction();
            chargeAction.Execute(player, enemy);
            chargeAction.Execute(player, enemy);
            
            // 建立包含混合動作的 Echo 卡：攻擊、防禦、攻擊、蓄力
            var actions = new List<ActionRecord>
            {
                new ActionRecord("Hero", "Attack", ActionType.Attack, 1, 1),
                new ActionRecord("Hero", "Block", ActionType.Block, 1, 2),
                new ActionRecord("Hero", "Attack", ActionType.Attack, 1, 3),
                new ActionRecord("Hero", "Charge", ActionType.Charge, 1, 4)
            };
            var echoCard = new EchoCard();
            echoCard.Name = "Test Echo";
            echoCard.Actions = actions;
            
            int initialChargeLevel = player.ChargeLevel;
            
            // Act
            var result = EchoExecutor.ExecuteEchoCard(echoCard, player, enemy);
            
            // Assert
            Assert.AreEqual(1, result.HeavyStrikeCount); // 1 次重擊 (min(2,1))
            Assert.AreEqual(10, result.HeavyStrikeDamage); // 1 * (6 + 4) = 10
            Assert.AreEqual(1, result.NormalAttackCount); // 1 次普通攻擊
            Assert.AreEqual(6, result.NormalDamage); // 1 * 6 = 6
            Assert.AreEqual(3, result.ShieldGain); // 1 * 3 = 3
            Assert.AreEqual(1, result.BlockCount);
            Assert.AreEqual(0, result.ChargeGain); // 0 次未配對蓄力
            Assert.AreEqual(0, result.ChargeCount);
            Assert.AreEqual(initialChargeLevel, player.ChargeLevel); // 蓄力等級不變
        }

        [Test]
        public void EchoWithNoCharge_ShouldDealOnlyBaseDamage()
        {
            // Arrange
            var player = new Player("Hero", 30);
            var enemy = new Enemy("Slime", 20);
            
            // 玩家沒有蓄力
            Assert.AreEqual(0, player.ChargeLevel);
            
            // 建立包含 2 次攻擊的 Echo 卡
            var actions = new List<ActionRecord>
            {
                new ActionRecord("Hero", "Attack", ActionType.Attack, 1, 1),
                new ActionRecord("Hero", "Attack", ActionType.Attack, 1, 2)
            };
            var echoCard = new EchoCard();
            echoCard.Name = "Test Echo";
            echoCard.Actions = actions;
            
            // Act
            var result = EchoExecutor.ExecuteEchoCard(echoCard, player, enemy);
            
            // Assert
            Assert.AreEqual(0, result.HeavyStrikeCount);
            Assert.AreEqual(0, result.HeavyStrikeDamage);
            Assert.AreEqual(2, result.NormalAttackCount);
            Assert.AreEqual(12, result.NormalDamage); // 2 * 6 = 12
            Assert.AreEqual(0, player.ChargeLevel);
        }

        [Test]
        public void EchoWithOnlyChargeActions_ShouldNotConsumeCharge()
        {
            // Arrange
            var player = new Player("Hero", 30);
            var enemy = new Enemy("Slime", 20);
            
            // 玩家沒有蓄力
            Assert.AreEqual(0, player.ChargeLevel);
            
            // 建立只包含蓄力的 Echo 卡
            var actions = new List<ActionRecord>
            {
                new ActionRecord("Hero", "Charge", ActionType.Charge, 1, 1),
                new ActionRecord("Hero", "Charge", ActionType.Charge, 1, 2),
                new ActionRecord("Hero", "Charge", ActionType.Charge, 1, 3)
            };
            var echoCard = new EchoCard();
            echoCard.Name = "Test Echo";
            echoCard.Actions = actions;
            
            // Act
            var result = EchoExecutor.ExecuteEchoCard(echoCard, player, enemy);
            
            // Assert
            Assert.AreEqual(0, result.HeavyStrikeCount);
            Assert.AreEqual(0, result.HeavyStrikeDamage);
            Assert.AreEqual(0, result.NormalAttackCount);
            Assert.AreEqual(0, result.NormalDamage);
            Assert.AreEqual(3, result.ChargeGain); // 3 * 1 = 3
            Assert.AreEqual(3, result.ChargeCount);
            Assert.AreEqual(3, player.ChargeLevel);
        }
    }
} 