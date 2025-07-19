using NUnit.Framework;
using RecallCore.AI;
using RecallCore.Actions;
using RecallCore.Entities;

namespace RecallTests.Behavior
{
    [TestFixture]
    public class FixedSequenceAIBehaviorTests
    {
        [Test]
        public void FixedSequenceAI_ShouldFollowExactSequence()
        {
            // Arrange
            var ai = new FixedSequenceAIStrategy(
                new AttackAction(),
                new BlockAction(),
                new ChargeAction()
            );

            // Act & Assert
            Assert.AreEqual("Attack", ai.ChooseAction(null, null).Name);
            Assert.AreEqual("Block", ai.ChooseAction(null, null).Name);
            Assert.AreEqual("Charge", ai.ChooseAction(null, null).Name);
        }

        [Test]
        public void FixedSequenceAI_ShouldLoopSequence()
        {
            // Arrange
            var ai = new FixedSequenceAIStrategy(
                new AttackAction(),
                new BlockAction(),
                new ChargeAction()
            );

            // Act - 執行完整序列
            ai.ChooseAction(null, null); // Attack
            ai.ChooseAction(null, null); // Block
            ai.ChooseAction(null, null); // Charge

            // Assert - 應該重新開始循環
            Assert.AreEqual("Attack", ai.ChooseAction(null, null).Name);
        }

        [Test]
        public void FixedSequenceAI_ShouldMaintainIndexCorrectly()
        {
            // Arrange
            var ai = new FixedSequenceAIStrategy(
                new AttackAction(),
                new BlockAction(),
                new ChargeAction()
            );

            // Act & Assert
            Assert.AreEqual(0, ai.GetCurrentIndex());
            Assert.AreEqual("Attack", ai.GetCurrentActionName());

            ai.ChooseAction(null, null); // Attack
            Assert.AreEqual(1, ai.GetCurrentIndex());
            Assert.AreEqual("Block", ai.GetCurrentActionName());

            ai.ChooseAction(null, null); // Block
            Assert.AreEqual(2, ai.GetCurrentIndex());
            Assert.AreEqual("Charge", ai.GetCurrentActionName());

            ai.ChooseAction(null, null); // Charge
            Assert.AreEqual(0, ai.GetCurrentIndex()); // 循環回到開始
            Assert.AreEqual("Attack", ai.GetCurrentActionName());
        }

        [Test]
        public void FixedSequenceAI_ShouldHandleSingleActionSequence()
        {
            // Arrange
            var ai = new FixedSequenceAIStrategy(new AttackAction());

            // Act & Assert
            for (int i = 0; i < 5; i++)
            {
                Assert.AreEqual("Attack", ai.ChooseAction(null, null).Name);
                Assert.AreEqual(0, ai.GetCurrentIndex()); // 永遠是 0
            }
        }

        [Test]
        public void FixedSequenceAI_ShouldProvideCorrectStrategyName()
        {
            // Arrange
            var ai = new FixedSequenceAIStrategy(
                new AttackAction(),
                new BlockAction(),
                new ChargeAction()
            );

            // Act
            string strategyName = ai.GetStrategyName();

            // Assert
            Assert.AreEqual("Fixed Sequence AI (Attack -> Block -> Charge)", strategyName);
        }

        [Test]
        public void FixedSequenceAI_ShouldThrowExceptionForEmptySequence()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                new FixedSequenceAIStrategy());
        }

        [Test]
        public void FixedSequenceAI_ShouldThrowExceptionForNullSequence()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                new FixedSequenceAIStrategy(null));
        }

        [Test]
        public void FixedSequenceAI_WithEnemy_ShouldAnnounceCorrectSequence()
        {
            // Arrange
            var ai = new FixedSequenceAIStrategy(
                new AttackAction(),
                new BlockAction(),
                new ChargeAction()
            );
            var enemy = new Enemy("Test Enemy", 20, 1, ai);

            // Act & Assert
            enemy.AnnounceAction();
            Assert.AreEqual("Attack", enemy.GetAnnouncedActionName());

            enemy.AnnounceAction();
            Assert.AreEqual("Block", enemy.GetAnnouncedActionName());

            enemy.AnnounceAction();
            Assert.AreEqual("Charge", enemy.GetAnnouncedActionName());

            enemy.AnnounceAction();
            Assert.AreEqual("Attack", enemy.GetAnnouncedActionName()); // 循環
        }
    }
} 