using NUnit.Framework;
using RecallCore.Actions;
using RecallCore.Entities;

namespace RecallTests.Behavior
{
    [TestFixture]
    public class ChargeBehaviorTests
    {
        [Test]
        public void Charge_ShouldPersistAcrossTurns()
        {
            // Arrange
            var player = new Player("Test Player", 30);
            var enemy = new Enemy("Test Enemy", 20);
            var chargeAction = new ChargeAction();
            var attackAction = new AttackAction();

            // Act - 玩家使用 Charge
            chargeAction.Execute(player, enemy);
            Assert.IsTrue(player.IsCharged, "Player should be charged after using Charge");

            // Act - 重置 AP（模擬回合結束）
            player.ResetAP();
            Assert.IsTrue(player.IsCharged, "Charge state should persist after AP reset");

            // Act - 玩家使用 Attack（應該有雙倍傷害）
            int enemyHPBefore = enemy.HP;
            attackAction.Execute(player, enemy);
            int damageDealt = enemyHPBefore - enemy.HP;

            // Assert - 應該造成 12 點傷害（6 * 2）
            Assert.AreEqual(12, damageDealt, "Charged attack should deal double damage");
            Assert.IsFalse(player.IsCharged, "Charge state should be consumed after attack");
        }

        [Test]
        public void Charge_ShouldBeConsumedAfterAttack()
        {
            // Arrange
            var player = new Player("Test Player", 30);
            var enemy = new Enemy("Test Enemy", 20);
            var chargeAction = new ChargeAction();
            var attackAction = new AttackAction();

            // Act - 玩家使用 Charge 然後攻擊
            chargeAction.Execute(player, enemy);
            Assert.IsTrue(player.IsCharged, "Player should be charged");

            attackAction.Execute(player, enemy);
            Assert.IsFalse(player.IsCharged, "Charge should be consumed after attack");
        }

        [Test]
        public void MultipleCharges_ShouldNotStack()
        {
            // Arrange
            var player = new Player("Test Player", 30);
            var enemy = new Enemy("Test Enemy", 20);
            var chargeAction = new ChargeAction();
            var attackAction = new AttackAction();

            // Act - 玩家連續使用 Charge
            chargeAction.Execute(player, enemy);
            Assert.IsTrue(player.IsCharged, "First charge should work");

            chargeAction.Execute(player, enemy);
            Assert.IsTrue(player.IsCharged, "Second charge should not change state");

            // Act - 攻擊應該只有一次雙倍傷害
            int enemyHPBefore = enemy.HP;
            attackAction.Execute(player, enemy);
            int damageDealt = enemyHPBefore - enemy.HP;

            // Assert - 應該造成 12 點傷害（6 * 2），不是 18 點（6 * 3）
            Assert.AreEqual(12, damageDealt, "Multiple charges should not stack");
            Assert.IsFalse(player.IsCharged, "Charge should be consumed after attack");
        }

        [Test]
        public void Enemy_ChargeShouldPersistAcrossTurns()
        {
            // Arrange
            var enemy = new Enemy("Test Enemy", 20);
            var player = new Player("Test Player", 30);
            var chargeAction = new ChargeAction();
            var attackAction = new AttackAction();

            // Act - 敵人使用 Charge
            chargeAction.Execute(enemy, player);
            Assert.IsTrue(enemy.IsCharged, "Enemy should be charged after using Charge");

            // Act - 重置 AP（模擬回合結束）
            enemy.ResetAP();
            Assert.IsTrue(enemy.IsCharged, "Enemy charge state should persist after AP reset");

            // Act - 敵人使用 Attack（應該有雙倍傷害）
            int playerHPBefore = player.HP;
            attackAction.Execute(enemy, player);
            int damageDealt = playerHPBefore - player.HP;

            // Assert - 應該造成 12 點傷害（6 * 2）
            Assert.AreEqual(12, damageDealt, "Enemy charged attack should deal double damage");
            Assert.IsFalse(enemy.IsCharged, "Enemy charge state should be consumed after attack");
        }
    }
} 