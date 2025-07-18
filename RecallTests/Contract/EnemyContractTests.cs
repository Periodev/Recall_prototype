using NUnit.Framework;
using RecallCore.Entities;

namespace RecallTests.Contract
{
    [TestFixture]
    public class EnemyContractTests
    {
        [Test]
        public void ResetAP_ShouldSetActionPointsTo2()
        {
            var enemy = new Enemy("Slime", 20);
            enemy.ActionPoints = 0;
            enemy.ResetAP();
            Assert.AreEqual(2, enemy.ActionPoints);
        }
    }
} 