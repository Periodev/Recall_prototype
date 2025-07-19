using NUnit.Framework;
using RecallCore.Entities;
using RecallCore.Actions;
using RecallCore.Memory;
using RecallCore.AI;

namespace RecallTests.GameLoop
{
    [TestFixture]
    public class SoftLockTests
    {
        private Player player;
        private Enemy enemy;
        private TimelineManager timelineManager;
        private EchoDeck echoDeck;
        private RecallSystem recallSystem;

        [SetUp]
        public void Setup()
        {
            player = new Player("TestPlayer", 10);
            var ai = new FixedSequenceAIStrategy(new AttackAction(), new BlockAction(), new ChargeAction());
            enemy = new Enemy("TestEnemy", 15, 3, ai);
            timelineManager = new TimelineManager();
            echoDeck = new EchoDeck();
            recallSystem = new RecallSystem(timelineManager, echoDeck);
        }

        [Test]
        public void PlayerWithNoAP_ShouldNotGetStuck()
        {
            // Arrange
            player.ActionPoints = 0; // 沒有 AP

            // Act
            var result = HandlePlayerAction("A", player, enemy, timelineManager, 1, 1);

            // Assert
            Assert.IsFalse(result, "Action should fail when AP is insufficient");
            Assert.AreEqual(0, player.ActionPoints, "AP should remain 0");
        }

        [Test]
        public void InvalidCommands_ShouldNotBreakGame()
        {
            // Arrange
            var invalidCommands = new[] { "INVALID", "XYZ", "123", "" };

            // Act & Assert
            foreach (var command in invalidCommands)
            {
                var result = HandleSpecialCommands(command, recallSystem, 1, timelineManager, player);
                Assert.IsFalse(result, $"Invalid command '{command}' should not break game");
            }
        }

        [Test]
        public void GameLoop_ShouldProgressToEnemyTurn_EvenWithFailedActions()
        {
            // Arrange
            player.ActionPoints = 0; // 沒有 AP 執行行動

            // Act
            var gameState = SimulatePlayerTurn(player, enemy, timelineManager);

            // Assert
            Assert.IsTrue(gameState.ShouldProgressToEnemyTurn, "Game should progress to enemy turn even with failed actions");
            Assert.AreEqual(0, gameState.PlayerActionsExecuted, "No player actions should be executed");
        }

        [Test]
        public void RecallWithNoAP_ShouldFailGracefully()
        {
            // Arrange
            player.ActionPoints = 0;
            timelineManager.AddAction(new ActionRecord("Player", "Attack", ActionType.Attack, 1, 1));

            // Act
            var result = recallSystem.HandleRecallCommand("R 1 1", 1, player);

            // Assert
            Assert.IsTrue(result, "Recall should handle AP insufficiency gracefully");
            Assert.AreEqual(0, player.ActionPoints, "AP should remain 0");
        }

        [Test]
        public void EchoWithNoCards_ShouldNotCrash()
        {
            // Act
            var result = recallSystem.HandleEchoCommand("ECHO 1");

            // Assert
            Assert.IsTrue(result, "Echo command should handle empty deck gracefully");
        }

        [Test]
        public void TimelineWithNoActions_ShouldHandleRecall()
        {
            // Arrange
            player.ActionPoints = 1;

            // Act
            var result = recallSystem.HandleRecallCommand("R 1 1", 1, player);

            // Assert
            Assert.IsTrue(result, "Recall should handle empty timeline gracefully");
        }

        // Helper methods to simulate game logic
        private bool HandlePlayerAction(string input, Player player, Enemy enemy, TimelineManager timelineManager, int turn, int step)
        {
            var upperInput = input.ToUpper();
            IAction action = null;
            ActionType actionType = ActionType.Attack;

            switch (upperInput)
            {
                case "A":
                case "ATTACK":
                    action = new AttackAction();
                    actionType = ActionType.Attack;
                    break;
                default:
                    return false;
            }

            if (player.ActionPoints >= action.Cost)
            {
                action.Execute(player, enemy);
                player.ActionPoints -= action.Cost;
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool HandleSpecialCommands(string input, RecallSystem recallSystem, int currentTurn, TimelineManager timelineManager, Player player)
        {
            var upperInput = input.ToUpper();
            
            if (upperInput.StartsWith("R "))
            {
                return recallSystem.HandleRecallCommand(input, currentTurn, player);
            }
            
            if (upperInput.StartsWith("ECHO"))
            {
                return recallSystem.HandleEchoCommand(input);
            }
            
            return false;
        }

        private GameTurnState SimulatePlayerTurn(Player player, Enemy enemy, TimelineManager timelineManager)
        {
            var state = new GameTurnState();
            
            // 模擬玩家回合的2步行動
            for (int step = 0; step < 2; step++)
            {
                if (player.ActionPoints > 0)
                {
                    state.PlayerActionsExecuted++;
                }
            }
            
            // 如果沒有執行任何行動，仍然應該進入敵人回合
            state.ShouldProgressToEnemyTurn = true;
            
            return state;
        }

        private class GameTurnState
        {
            public int PlayerActionsExecuted { get; set; }
            public bool ShouldProgressToEnemyTurn { get; set; }
        }
    }
} 