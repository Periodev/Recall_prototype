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
            Assert.AreEqual(15, target.HP); // 預設傷害 5
        }

        [Test]
        public void BlockAction_Execute_ShouldSetIsBlocking()
        {
            var actor = new Player("Hero", 30);
            var target = new Enemy("Slime", 20);
            var action = new BlockAction();
            actor.IsBlocking = false;
            action.Execute(actor, target);
            Assert.IsTrue(actor.IsBlocking);
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
            
            // 先 Charge
            attacker.IsCharged = true;
            
            // 再 Attack
            action.Execute(attacker, target);
            
            Assert.AreEqual(10, target.HP); // 原本 20 - 強化傷害 10 (5*2)
            Assert.IsFalse(attacker.IsCharged); // 使用後應該重置
        }

        [Test]
        public void TakeDamage_WithBlocking_ShouldReduceDamageBy3()
        {
            var actor = new Player("Hero", 30);
            actor.IsBlocking = true;
            
            // 測試傷害 5，Block 後應該扣 2 HP (5-3=2)
            actor.TakeDamage(5);
            Assert.AreEqual(28, actor.HP);
            Assert.IsFalse(actor.IsBlocking); // Block 使用後應該重置
        }

        [Test]
        public void TakeDamage_WithBlocking_ShouldBlockCompletely_WhenDamageLessThan3()
        {
            var actor = new Player("Hero", 30);
            actor.IsBlocking = true;
            
            // 測試傷害 2，Block 後應該扣 0 HP (2-3=0)
            actor.TakeDamage(2);
            Assert.AreEqual(30, actor.HP);
            Assert.IsFalse(actor.IsBlocking);
        }

        [Test]
        public void TakeDamage_WithBlocking_ShouldBlockCompletely_WhenDamageEquals3()
        {
            var actor = new Player("Hero", 30);
            actor.IsBlocking = true;
            
            // 測試傷害 3，Block 後應該扣 0 HP (3-3=0)
            actor.TakeDamage(3);
            Assert.AreEqual(30, actor.HP);
            Assert.IsFalse(actor.IsBlocking);
        }
    }
} 