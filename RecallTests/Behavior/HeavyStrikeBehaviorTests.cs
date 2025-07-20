using NUnit.Framework;
using RecallCore.Entities;
using RecallCore.Actions;
using RecallCore.Memory;

namespace RecallTests.Behavior
{
    [TestFixture]
    public class HeavyStrikeBehaviorTests
    {
        [Test]
        public void ChargeAction_ShouldIncreaseChargeLevel()
        {
            // Arrange
            var player = new Player("Hero", 30);
            var enemy = new Enemy("Slime", 20);
            var chargeAction = new ChargeAction();
            
            // Act
            chargeAction.Execute(player, enemy);
            
            // Assert
            Assert.AreEqual(1, player.ChargeLevel);
            Assert.IsTrue(player.IsCharged);
        }

        [Test]
        public void MultipleChargeActions_ShouldAccumulateChargeLevel()
        {
            // Arrange
            var player = new Player("Hero", 30);
            var enemy = new Enemy("Slime", 20);
            var chargeAction = new ChargeAction();
            
            // Act
            chargeAction.Execute(player, enemy);
            chargeAction.Execute(player, enemy);
            chargeAction.Execute(player, enemy);
            
            // Assert
            Assert.AreEqual(3, player.ChargeLevel);
        }

        [Test]
        public void AttackWithChargeLevel_ShouldDealBonusDamage()
        {
            // Arrange
            var player = new Player("Hero", 30);
            var enemy = new Enemy("Slime", 20);
            var chargeAction = new ChargeAction();
            var attackAction = new AttackAction();
            
            // Act
            chargeAction.Execute(player, enemy); // +1 charge level
            chargeAction.Execute(player, enemy); // +1 charge level
            int initialHP = enemy.HP;
            attackAction.Execute(player, enemy);
            
            // Assert
            int expectedDamage = 6 + 4; // Base damage + 1 charge level * 4 bonus
            Assert.AreEqual(initialHP - expectedDamage, enemy.HP);
            Assert.AreEqual(1, player.ChargeLevel); // Only 1 charge level consumed
            Assert.IsTrue(player.IsCharged); // Still has charge level
        }

        [Test]
        public void AttackWithoutChargeLevel_ShouldDealBaseDamage()
        {
            // Arrange
            var player = new Player("Hero", 30);
            var enemy = new Enemy("Slime", 20);
            var attackAction = new AttackAction();
            
            // Act
            int initialHP = enemy.HP;
            attackAction.Execute(player, enemy);
            
            // Assert
            Assert.AreEqual(initialHP - 6, enemy.HP); // Base damage only
            Assert.AreEqual(0, player.ChargeLevel);
        }

        [Test]
        public void ChargeLevel_ShouldPersistAcrossTurns()
        {
            // Arrange
            var player = new Player("Hero", 30);
            var enemy = new Enemy("Slime", 20);
            var chargeAction = new ChargeAction();
            
            // Act
            chargeAction.Execute(player, enemy);
            player.EndTurn(); // Should not reset charge level
            
            // Assert
            Assert.AreEqual(1, player.ChargeLevel);
        }

        [Test]
        public void EchoCard_WithHeavyStrikes_ShouldCalculateCorrectly()
        {
            // Arrange
            var player = new Player("Hero", 30);
            var enemy = new Enemy("Slime", 20);
            var card = new EchoCard
            {
                Actions = new List<ActionRecord>
                {
                    new ActionRecord("Player", "Attack", ActionType.Attack, 1, 1),
                    new ActionRecord("Player", "Charge", ActionType.Charge, 1, 2),
                    new ActionRecord("Player", "Attack", ActionType.Attack, 1, 3),
                    new ActionRecord("Player", "Charge", ActionType.Charge, 1, 4)
                }
            };
            
            // Act
            int initialHP = enemy.HP;
            var result = EchoExecutor.ExecuteEchoCard(card, player, enemy);
            
            // Assert
            // 2 attacks, 2 charges = 2 heavy strikes + 0 unpaired attacks + 0 unpaired charges
            int expectedHeavyDamage = 2 * (6 + 4); // 2 heavy strikes * (base + bonus)
            Assert.AreEqual(expectedHeavyDamage, result.HeavyStrikeDamage);
            Assert.AreEqual(2, result.HeavyStrikeCount);
            Assert.AreEqual(0, result.NormalDamage);
            Assert.AreEqual(0, result.ChargeGain);
            Assert.AreEqual(initialHP - expectedHeavyDamage, enemy.HP);
        }

        [Test]
        public void EchoCard_WithUnpairedActions_ShouldCalculateCorrectly()
        {
            // Arrange
            var player = new Player("Hero", 30);
            var enemy = new Enemy("Slime", 20);
            var card = new EchoCard
            {
                Actions = new List<ActionRecord>
                {
                    new ActionRecord("Player", "Attack", ActionType.Attack, 1, 1),
                    new ActionRecord("Player", "Attack", ActionType.Attack, 1, 2),
                    new ActionRecord("Player", "Charge", ActionType.Charge, 1, 3),
                    new ActionRecord("Player", "Block", ActionType.Block, 1, 4)
                }
            };
            
            // Act
            int initialHP = enemy.HP;
            int initialShield = player.CurrentShield;
            var result = EchoExecutor.ExecuteEchoCard(card, player, enemy);
            
            // Assert
            // 2 attacks, 1 charge = 1 heavy strike + 1 unpaired attack + 0 unpaired charges + 1 block
            int expectedHeavyDamage = 1 * (6 + 4); // 1 heavy strike
            int expectedNormalDamage = 1 * 6; // 1 unpaired attack
            int expectedShieldGain = 1 * 3; // 1 block
            
            Assert.AreEqual(expectedHeavyDamage, result.HeavyStrikeDamage);
            Assert.AreEqual(1, result.HeavyStrikeCount);
            Assert.AreEqual(expectedNormalDamage, result.NormalDamage);
            Assert.AreEqual(1, result.NormalAttackCount);
            Assert.AreEqual(expectedShieldGain, result.ShieldGain);
            Assert.AreEqual(1, result.BlockCount);
            Assert.AreEqual(0, result.ChargeGain);
            
            Assert.AreEqual(initialHP - expectedHeavyDamage - expectedNormalDamage, enemy.HP);
            Assert.AreEqual(initialShield + expectedShieldGain, player.CurrentShield);
        }

        [Test]
        public void EchoCard_WithOnlyBlocks_ShouldNotRequireTarget()
        {
            // Arrange
            var player = new Player("Hero", 30);
            var card = new EchoCard
            {
                Actions = new List<ActionRecord>
                {
                    new ActionRecord("Player", "Block", ActionType.Block, 1, 1),
                    new ActionRecord("Player", "Block", ActionType.Block, 1, 2)
                }
            };
            
            // Act
            int initialShield = player.CurrentShield;
            var result = EchoExecutor.ExecuteEchoCard(card, player, null); // No target needed
            
            // Assert
            Assert.AreEqual(6, result.ShieldGain); // 2 blocks * 3 shield
            Assert.AreEqual(2, result.BlockCount);
            Assert.AreEqual(0, result.HeavyStrikeDamage);
            Assert.AreEqual(0, result.NormalDamage);
            Assert.AreEqual(0, result.ChargeGain);
            Assert.AreEqual(initialShield + 6, player.CurrentShield);
        }

        [Test]
        public void EchoCard_WithAttacksButNoTarget_ShouldThrowException()
        {
            // Arrange
            var player = new Player("Hero", 30);
            var card = new EchoCard
            {
                Actions = new List<ActionRecord>
                {
                    new ActionRecord("Player", "Attack", ActionType.Attack, 1, 1)
                }
            };
            
            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => 
                EchoExecutor.ExecuteEchoCard(card, player, null));
        }
    }
} 