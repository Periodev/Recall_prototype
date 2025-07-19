using NUnit.Framework;
using RecallCore.Entities;

namespace RecallTests.Behavior
{
    [TestFixture]
    public class EnemyBehaviorTests
    {
        [Test]
        public void TakeDamage_ShouldReduceHP()
        {
            var enemy = new Enemy("Slime", 20);
            enemy.TakeDamage(7);
            Assert.AreEqual(13, enemy.HP);
        }

        [Test]
        public void TakeDamage_ShouldNotGoBelowZero()
        {
            var enemy = new Enemy("Slime", 5);
            enemy.TakeDamage(10);
            Assert.AreEqual(0, enemy.HP);
        }

        [Test]
        public void AnnounceAction_ShouldReturnValidActionName()
        {
            var enemy = new Enemy("Slime", 20);
            for (int i = 0; i < 20; i++)
            {
                enemy.AnnounceAction();
                string action = enemy.GetAnnouncedActionName();
                Assert.That(new string[] { "Attack", "Block", "Charge" }, Does.Contain(action)); // 重新加入 Charge
            }
        }

        [Test]
        public void ChooseAction_ShouldChooseBlock_WhenHPIsLow()
        {
            var enemy = new Enemy("Slime", 5); // 低於 CRITICAL_HP_THRESHOLD (5)
            var action = enemy.ChooseAction();
            Assert.AreEqual("Block", action.Name);
        }

        [Test]
        public void ChooseAction_ShouldChooseBlock_WhenHPIsBelowLowThreshold()
        {
            var enemy = new Enemy("Slime", 8); // 低於 LOW_HP_THRESHOLD (10)
            var action = enemy.ChooseAction();
            Assert.AreEqual("Block", action.Name);
        }

        [Test]
        public void ChooseAction_ShouldChooseAttack_WhenHPIsHealthy()
        {
            var enemy = new Enemy("Slime", 15); // 高於 LOW_HP_THRESHOLD (10) 但低於 15
            var action = enemy.ChooseAction();
            Assert.AreEqual("Attack", action.Name);
        }

        [Test]
        public void ChooseAction_ShouldChooseAttackOrCharge_WhenHPIsVeryHealthy()
        {
            var enemy = new Enemy("Slime", 20); // 高於 LOW_HP_THRESHOLD + 5 (15)
            var action = enemy.ChooseAction();
            Assert.That(new string[] { "Attack", "Charge" }, Does.Contain(action.Name));
        }

        [Test]
        public void IsDead_ShouldReturnTrue_WhenHPIsZero()
        {
            var enemy = new Enemy("Slime", 20);
            enemy.TakeDamage(20);
            Assert.IsTrue(enemy.IsDead());
        }

        [Test]
        public void IsDead_ShouldReturnFalse_WhenHPIsAboveZero()
        {
            var enemy = new Enemy("Slime", 20);
            Assert.IsFalse(enemy.IsDead());
        }

        [Test]
        public void ExecuteAnnouncedAction_ShouldExecuteAndClearAnnouncement()
        {
            var enemy = new Enemy("Slime", 20);
            var target = new Player("Hero", 30);
            
            enemy.AnnounceAction();
            string announcedAction = enemy.GetAnnouncedActionName();
            Assert.That(new string[] { "Attack", "Block", "Charge" }, Does.Contain(announcedAction));
            
            enemy.ExecuteAnnouncedAction(target);
            Assert.AreEqual("None", enemy.GetAnnouncedActionName()); // 執行後應該清除預告
        }

        [Test]
        public void ExecuteAnnouncedAction_ShouldConsumeAP()
        {
            var enemy = new Enemy("Slime", 20, 1);
            var target = new Player("Hero", 30);
            
            enemy.AnnounceAction();
            int initialAP = enemy.ActionPoints;
            
            enemy.ExecuteAnnouncedAction(target);
            Assert.AreEqual(initialAP - 1, enemy.ActionPoints); // 應該消耗 1 AP
        }

        [Test]
        public void ExecuteAnnouncedAction_ShouldNotExecute_WhenInsufficientAP()
        {
            var enemy = new Enemy("Slime", 20, 0); // 0 AP
            var target = new Player("Hero", 30);
            int initialTargetHP = target.HP;
            
            enemy.AnnounceAction();
            enemy.ExecuteAnnouncedAction(target);
            
            Assert.AreEqual(initialTargetHP, target.HP); // 目標 HP 不應該改變
            Assert.AreEqual("None", enemy.GetAnnouncedActionName()); // 預告應該被清除
        }

        [Test]
        public void GetAnnouncedActionName_ShouldReturnNone_WhenNoAnnouncement()
        {
            var enemy = new Enemy("Slime", 20);
            Assert.AreEqual("None", enemy.GetAnnouncedActionName());
        }

        [Test]
        public void Enemy_WithDifferentAP_ShouldHaveCorrectInitialAP()
        {
            var enemy1 = new Enemy("Slime", 20, 1);
            var enemy2 = new Enemy("Boss", 50, 3);
            
            Assert.AreEqual(1, enemy1.ActionPoints);
            Assert.AreEqual(3, enemy2.ActionPoints);
        }

        [Test]
        public void Enemy_WithDifferentAP_ShouldResetToCorrectAP()
        {
            var enemy1 = new Enemy("Slime", 20, 1);
            var enemy2 = new Enemy("Boss", 50, 3);
            
            enemy1.ActionPoints = 0;
            enemy2.ActionPoints = 0;
            
            enemy1.ResetAP();
            enemy2.ResetAP();
            
            Assert.AreEqual(1, enemy1.ActionPoints);
            Assert.AreEqual(3, enemy2.ActionPoints);
        }
    }
}

