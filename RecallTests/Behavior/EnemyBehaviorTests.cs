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
        public void DecideAction_ShouldReturnValidActionName()
        {
            var enemy = new Enemy("Slime", 20);
            for (int i = 0; i < 20; i++)
            {
                string action = enemy.DecideAction();
                Assert.That(new[] { "Attack", "Block", "Charge" }, Does.Contain(action));
            }
        }
    }
}
