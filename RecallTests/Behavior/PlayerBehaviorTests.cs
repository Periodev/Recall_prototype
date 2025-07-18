using NUnit.Framework;
using RecallCore.Entities;

namespace RecallTests.Behavior
{
    [TestFixture]
    public class PlayerBehaviorTests
    {
        [Test]
        public void TakeDamage_ShouldReduceHP()
        {
            var player = new Player("Hero", 30);
            player.TakeDamage(5);
            Assert.AreEqual(25, player.HP);
        }

        [Test]
        public void TakeDamage_ShouldNotGoBelowZero()
        {
            var player = new Player("Hero", 10);
            player.TakeDamage(20);
            Assert.AreEqual(0, player.HP);
        }
    }
}
