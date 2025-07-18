using NUnit.Framework;
using RecallCore.Entities;

namespace RecallTests.Contract
{
    [TestFixture]
    public class PlayerContractTests
    {
        [Test]
        public void ResetAP_ShouldSetActionPointsTo2()
        {
            var player = new Player("Hero", 30);
            player.ActionPoints = 0;
            player.ResetAP();
            Assert.AreEqual(2, player.ActionPoints);
        }
    }
} 