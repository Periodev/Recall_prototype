using NUnit.Framework;
using RecallCore.Entities;
using RecallCore.Actions;

namespace RecallTests.Behavior
{
    [TestFixture]
    public class ShieldBehaviorTests
    {
        [Test]
        public void BlockAction_ShouldAddShield()
        {
            // Arrange
            var player = new Player("Hero", 30);
            var enemy = new Enemy("Slime", 20);
            var blockAction = new BlockAction();
            
            // Act
            blockAction.Execute(player, enemy);
            
            // Assert
            Assert.AreEqual(3, player.CurrentShield);
        }

        [Test]
        public void TakeDamage_WithShield_ShouldAbsorbDamage()
        {
            // Arrange
            var player = new Player("Hero", 30);
            player.AddShield(5); // 手動添加護盾
            
            // Act
            player.TakeDamage(3);
            
            // Assert
            Assert.AreEqual(2, player.CurrentShield); // 5 - 3 = 2
            Assert.AreEqual(30, player.HP); // HP 不變
        }

        [Test]
        public void TakeDamage_WithShield_ShouldAbsorbAllDamage_WhenShieldSufficient()
        {
            // Arrange
            var player = new Player("Hero", 30);
            player.AddShield(5);
            
            // Act
            player.TakeDamage(5);
            
            // Assert
            Assert.AreEqual(0, player.CurrentShield);
            Assert.AreEqual(30, player.HP); // HP 不變
        }

        [Test]
        public void TakeDamage_WithShield_ShouldAbsorbPartialDamage_WhenShieldInsufficient()
        {
            // Arrange
            var player = new Player("Hero", 30);
            player.AddShield(3);
            
            // Act
            player.TakeDamage(7);
            
            // Assert
            Assert.AreEqual(0, player.CurrentShield);
            Assert.AreEqual(26, player.HP); // 30 - (7-3) = 26
        }

        [Test]
        public void TakeDamage_WithoutShield_ShouldReduceHP()
        {
            // Arrange
            var player = new Player("Hero", 30);
            
            // Act
            player.TakeDamage(5);
            
            // Assert
            Assert.AreEqual(0, player.CurrentShield);
            Assert.AreEqual(25, player.HP);
        }

        [Test]
        public void EndTurn_ShouldClearShield()
        {
            // Arrange
            var player = new Player("Hero", 30);
            player.AddShield(5);
            
            // Act
            player.EndTurn();
            
            // Assert
            Assert.AreEqual(0, player.CurrentShield);
            Assert.AreEqual(2, player.ActionPoints); // AP 重置
        }

        [Test]
        public void MultipleBlockActions_ShouldAccumulateShield()
        {
            // Arrange
            var player = new Player("Hero", 30);
            var enemy = new Enemy("Slime", 20);
            var blockAction = new BlockAction();
            
            // Act
            blockAction.Execute(player, enemy); // 第一次 Block
            blockAction.Execute(player, enemy); // 第二次 Block
            
            // Assert
            Assert.AreEqual(6, player.CurrentShield); // 3 + 3 = 6
        }

        [Test]
        public void AttackAction_WithShield_ShouldReduceShieldFirst()
        {
            // Arrange
            var player = new Player("Hero", 30);
            var enemy = new Enemy("Slime", 20);
            player.AddShield(4);
            var attackAction = new AttackAction();
            
            // Act
            attackAction.Execute(enemy, player); // 敵人攻擊玩家
            
            // Assert
            Assert.AreEqual(0, player.CurrentShield); // 4 - 6 = 0 (護盾不足)
            Assert.AreEqual(28, player.HP); // 30 - (6-4) = 28
        }

        [Test]
        public void ChargedAttack_WithShield_ShouldReduceShieldFirst()
        {
            // Arrange
            var player = new Player("Hero", 30);
            var enemy = new Enemy("Slime", 20);
            player.AddShield(8);
            enemy.IsCharged = true; // 敵人處於 Charged 狀態
            var attackAction = new AttackAction();
            
            // Act
            attackAction.Execute(enemy, player); // 敵人強化攻擊玩家
            
            // Assert
            Assert.AreEqual(0, player.CurrentShield); // 8 - 12 = 0 (護盾不足)
            Assert.AreEqual(26, player.HP); // 30 - (12-8) = 26
            Assert.IsFalse(enemy.IsCharged); // Charged 狀態應該重置
        }

        [Test]
        public void Shield_ShouldNotGoBelowZero()
        {
            // Arrange
            var player = new Player("Hero", 30);
            player.AddShield(3);
            
            // Act
            player.TakeDamage(10);
            
            // Assert
            Assert.AreEqual(0, player.CurrentShield);
            Assert.AreEqual(23, player.HP); // 30 - (10-3) = 23
        }

        [Test]
        public void ClearShield_ShouldSetShieldToZero()
        {
            // Arrange
            var player = new Player("Hero", 30);
            player.AddShield(5);
            
            // Act
            player.ClearShield();
            
            // Assert
            Assert.AreEqual(0, player.CurrentShield);
        }

        [Test]
        public void AddShield_ShouldAccumulateCorrectly()
        {
            // Arrange
            var player = new Player("Hero", 30);
            
            // Act
            player.AddShield(3);
            player.AddShield(2);
            player.AddShield(1);
            
            // Assert
            Assert.AreEqual(6, player.CurrentShield);
        }

        [Test]
        public void AddShield_ShouldIgnoreNegativeValues()
        {
            // Arrange
            var player = new Player("Hero", 30);
            player.AddShield(5);
            
            // Act
            player.AddShield(-3); // 嘗試添加負數護盾
            
            // Assert
            Assert.AreEqual(5, player.CurrentShield); // 護盾值不變
        }

        [Test]
        public void TakeDamage_ShouldIgnoreZeroDamage()
        {
            // Arrange
            var player = new Player("Hero", 30);
            player.AddShield(3);
            
            // Act
            player.TakeDamage(0); // 嘗試造成零傷害
            
            // Assert
            Assert.AreEqual(3, player.CurrentShield); // 護盾不變
            Assert.AreEqual(30, player.HP); // HP 不變
        }

        [Test]
        public void TakeDamage_ShouldIgnoreNegativeDamage()
        {
            // Arrange
            var player = new Player("Hero", 30);
            player.AddShield(3);
            
            // Act
            player.TakeDamage(-5); // 嘗試造成負數傷害
            
            // Assert
            Assert.AreEqual(3, player.CurrentShield); // 護盾不變
            Assert.AreEqual(30, player.HP); // HP 不變
        }

        [Test]
        public void AddShield_ShouldHandleZeroValue()
        {
            // Arrange
            var player = new Player("Hero", 30);
            
            // Act
            player.AddShield(0); // 添加零護盾
            
            // Assert
            Assert.AreEqual(0, player.CurrentShield); // 護盾值為 0
        }

        [Test]
        public void AddShield_WithNegativeValue_ShouldNotDecrease()
        {
            // Arrange
            var player = new Player("Hero", 30);
            player.AddShield(5);
            
            // Act
            player.AddShield(-3); // 嘗試加入負數護盾
            
            // Assert
            Assert.AreEqual(5, player.CurrentShield); // 應該保持不變
        }

        [Test]  
        public void TakeDamage_WithZeroDamage_ShouldNotChange()
        {
            // Arrange
            var player = new Player("Hero", 30);
            player.AddShield(5);
            
            // Act
            player.TakeDamage(0);
            
            // Assert
            Assert.AreEqual(5, player.CurrentShield);
            Assert.AreEqual(30, player.HP);
        }
    }
} 