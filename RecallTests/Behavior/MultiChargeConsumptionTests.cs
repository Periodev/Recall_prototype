using NUnit.Framework;
using RecallCore.Entities;
using RecallCore.Actions;

namespace RecallTests.Behavior
{
    [TestFixture]
    public class MultiChargeConsumptionTests
    {
        [Test]
        public void AttackAction_ShouldConsumeOneChargeLevel()
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
            Assert.IsTrue(player.IsCharged);
        }

        [Test]
        public void BlockAction_ShouldNotConsumeChargeLevel()
        {
            // Arrange
            var player = new Player("Hero", 30);
            var enemy = new Enemy("Slime", 20);
            var chargeAction = new ChargeAction();
            var blockAction = new BlockAction();
            
            // Act
            chargeAction.Execute(player, enemy); // +1 charge level
            int initialCharge = player.ChargeLevel;
            blockAction.Execute(player, enemy);
            
            // Assert
            Assert.AreEqual(initialCharge, player.ChargeLevel); // Charge level unchanged
            Assert.IsTrue(player.IsCharged);
        }

        [Test]
        public void ChargeAction_ShouldNotConsumeChargeLevel()
        {
            // Arrange
            var player = new Player("Hero", 30);
            var enemy = new Enemy("Slime", 20);
            var chargeAction = new ChargeAction();
            
            // Act
            chargeAction.Execute(player, enemy); // +1 charge level
            int initialCharge = player.ChargeLevel;
            chargeAction.Execute(player, enemy); // +1 charge level
            
            // Assert
            Assert.AreEqual(initialCharge + 1, player.ChargeLevel); // Charge level increased
            Assert.IsTrue(player.IsCharged);
        }

        [Test]
        public void AttackAction_WithZeroCharge_ShouldDealBaseDamage()
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
            Assert.IsFalse(player.IsCharged);
        }
    }
} 