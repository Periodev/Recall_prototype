using NUnit.Framework;
using RecallCore.Entities;
using RecallCore.Actions;

namespace RecallTests.Behavior
{
    [TestFixture]
    public class ActionBehaviorTests
    {
        [Test]
        public void AttackAction_Execute_ShouldDealCorrectDamage()
        {
            var attacker = new Player("Hero", 30);
            var target = new Enemy("Slime", 20);
            var action = new AttackAction();
            action.Execute(attacker, target);
            Assert.AreEqual(14, target.HP); // 預設傷害 6 (20 - 6 = 14)
        }

        [Test]
        public void BlockAction_Execute_ShouldAddShield()
        {
            var actor = new Player("Hero", 30);
            var target = new Enemy("Slime", 20);
            var action = new BlockAction();
            action.Execute(actor, target);
            Assert.AreEqual(3, actor.CurrentShield); // Block 提供 3 點護盾
        }

        [Test]
        public void ChargeAction_Execute_ShouldSetIsCharged()
        {
            var actor = new Player("Hero", 30);
            var target = new Enemy("Slime", 20);
            var action = new ChargeAction();
            actor.IsCharged = false;
            action.Execute(actor, target);
            Assert.IsTrue(actor.IsCharged);
        }

        [Test]
        public void AttackAction_Execute_ShouldDealDoubleDamage_WhenCharged()
        {
            var attacker = new Player("Hero", 30);
            var target = new Enemy("Slime", 20);
            var action = new AttackAction();
            
            // 先 Charge - 使用正確的方法設置蓄力
            attacker.AddCharge(1);
            
            // 再 Attack
            action.Execute(attacker, target);
            
            Assert.AreEqual(10, target.HP); // 原本 20 - 強化傷害 10 (6 + 4 = 10)
            Assert.IsFalse(attacker.IsCharged); // 使用後應該重置
        }

        [Test]
        public void TakeDamage_WithShield_ShouldReduceDamageByShieldValue()
        {
            var actor = new Player("Hero", 30);
            actor.AddShield(3); // 添加 3 點護盾
            
            // 測試傷害 5，護盾後應該扣 2 HP (5-3=2)
            actor.TakeDamage(5);
            Assert.AreEqual(28, actor.HP);
            Assert.AreEqual(0, actor.CurrentShield); // 護盾應該被消耗完
        }

        [Test]
        public void TakeDamage_WithShield_ShouldBlockCompletely_WhenDamageLessThanShield()
        {
            var actor = new Player("Hero", 30);
            actor.AddShield(3); // 添加 3 點護盾
            
            // 測試傷害 2，護盾後應該扣 0 HP (2-3=0)
            actor.TakeDamage(2);
            Assert.AreEqual(30, actor.HP);
            Assert.AreEqual(1, actor.CurrentShield); // 護盾剩餘 1 點
        }

        [Test]
        public void TakeDamage_WithShield_ShouldBlockCompletely_WhenDamageEqualsShield()
        {
            var actor = new Player("Hero", 30);
            actor.AddShield(3); // 添加 3 點護盾
            
            // 測試傷害 3，護盾後應該扣 0 HP (3-3=0)
            actor.TakeDamage(3);
            Assert.AreEqual(30, actor.HP);
            Assert.AreEqual(0, actor.CurrentShield); // 護盾完全消耗
        }
    }
} 