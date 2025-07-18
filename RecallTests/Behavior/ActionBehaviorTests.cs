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
        public void ChargeAction_Execute_ShouldIncreaseAP()
        {
            var actor = new Player("Hero", 30);
            var target = new Enemy("Slime", 20);
            var action = new ChargeAction();
            actor.ActionPoints = 0;
            action.Execute(actor, target);
            Assert.AreEqual(2, actor.ActionPoints); // 預設增加 2
        }
    }
} 