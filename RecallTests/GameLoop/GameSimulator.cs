using System;
using System.Collections.Generic;
using RecallCore.Entities;
using RecallCore.Actions;
using RecallCore.Memory;
using RecallCore.AI;

namespace RecallTests.GameLoop
{
    public class GameSimulator
    {
        private Player player;
        private Enemy enemy;
        private TimelineManager timelineManager;
        private EchoDeck echoDeck;
        private RecallSystem recallSystem;
        private int maxIterations = 1000;
        private int currentIteration = 0;

        public GameSimulator()
        {
            player = new Player("SimPlayer", 10);
            var ai = new FixedSequenceAIStrategy(new AttackAction(), new BlockAction(), new ChargeAction());
            enemy = new Enemy("SimEnemy", 15, 3, ai);
            timelineManager = new TimelineManager();
            echoDeck = new EchoDeck();
            recallSystem = new RecallSystem(timelineManager, echoDeck);
        }

        public GameResult PlayFullGame()
        {
            var result = new GameResult();
            int turn = 1;
            int step = 1;

            try
            {
                while (player.HP > 0 && enemy.HP > 0 && currentIteration < maxIterations)
                {
                    currentIteration++;

                    // 模擬玩家回合
                    if (!SimulatePlayerTurn(turn, step))
                    {
                        result.IsHanging = true;
                        result.HangingReason = "Player turn hanging";
                        break;
                    }

                    if (player.HP <= 0 || enemy.HP <= 0) break;

                    // 模擬敵人回合
                    if (!SimulateEnemyTurn(turn, step))
                    {
                        result.IsHanging = true;
                        result.HangingReason = "Enemy turn hanging";
                        break;
                    }

                    // 回合結束
                    player.ResetAP();
                    enemy.ResetAP();
                    turn++;
                }

                result.IsCompleted = true;
                result.TotalTurns = turn - 1;
                result.TotalIterations = currentIteration;
                result.PlayerFinalHP = player.HP;
                result.EnemyFinalHP = enemy.HP;
                result.Winner = player.HP <= 0 ? "Enemy" : "Player";
            }
            catch (Exception ex)
            {
                result.IsCompleted = false;
                result.IsHanging = true;
                result.HangingReason = $"Exception: {ex.Message}";
            }

            return result;
        }

        private bool SimulatePlayerTurn(int turn, int step)
        {
            // 模擬玩家2步行動
            for (int playerStep = 0; playerStep < 2; playerStep++)
            {
                if (player.HP <= 0 || enemy.HP <= 0) break;

                // 隨機選擇玩家行動
                var action = GetRandomPlayerAction();
                if (!ExecutePlayerAction(action, turn, step))
                {
                    // 行動失敗，但繼續下一個行動
                    continue;
                }
                step++;
            }

            return true; // 玩家回合完成
        }

        private bool SimulateEnemyTurn(int turn, int step)
        {
            // 模擬敵人行動
            var enemyAction = GetRandomEnemyAction();
            if (!ExecuteEnemyAction(enemyAction, turn, step))
            {
                return false;
            }

            return true;
        }

        private string GetRandomPlayerAction()
        {
            var actions = new[] { "A", "B", "C", "R 1 1", "ECHO", "PASS" };
            var random = new Random();
            return actions[random.Next(actions.Length)];
        }

        private string GetRandomEnemyAction()
        {
            var actions = new[] { "A", "B", "C" };
            var random = new Random();
            return actions[random.Next(actions.Length)];
        }

        private bool ExecutePlayerAction(string action, int turn, int step)
        {
            var upperAction = action.ToUpper();

            // 處理特殊命令
            if (upperAction.StartsWith("R "))
            {
                return recallSystem.HandleRecallCommand(action, turn, player);
            }

            if (upperAction.StartsWith("ECHO"))
            {
                return recallSystem.HandleEchoCommand(action);
            }

            if (upperAction == "PASS")
            {
                return true;
            }

            // 處理基本行動
            IAction gameAction = null;
            ActionType actionType = ActionType.Attack;

            switch (upperAction)
            {
                case "A":
                    gameAction = new AttackAction();
                    actionType = ActionType.Attack;
                    break;
                case "B":
                    gameAction = new BlockAction();
                    actionType = ActionType.Block;
                    break;
                case "C":
                    gameAction = new ChargeAction();
                    actionType = ActionType.Charge;
                    break;
                default:
                    return false;
            }

            if (player.ActionPoints >= gameAction.Cost)
            {
                gameAction.Execute(player, enemy);
                player.ActionPoints -= gameAction.Cost;

                // 記錄到 Timeline
                var actionRecord = new ActionRecord(player.Name, gameAction.Name, actionType, turn, step);
                timelineManager.AddAction(actionRecord);

                return true;
            }

            return false; // AP 不足
        }

        private bool ExecuteEnemyAction(string action, int turn, int step)
        {
            var upperAction = action.ToUpper();
            IAction gameAction = null;

            switch (upperAction)
            {
                case "A":
                    gameAction = new AttackAction();
                    break;
                case "B":
                    gameAction = new BlockAction();
                    break;
                case "C":
                    gameAction = new ChargeAction();
                    break;
                default:
                    return false;
            }

            if (enemy.ActionPoints >= gameAction.Cost)
            {
                gameAction.Execute(enemy, player);
                enemy.ActionPoints -= gameAction.Cost;
                return true;
            }

            return false;
        }
    }

    public class GameResult
    {
        public bool IsCompleted { get; set; }
        public bool IsHanging { get; set; }
        public string HangingReason { get; set; }
        public int TotalTurns { get; set; }
        public int TotalIterations { get; set; }
        public int PlayerFinalHP { get; set; }
        public int EnemyFinalHP { get; set; }
        public string Winner { get; set; }

        public override string ToString()
        {
            if (IsHanging)
            {
                return $"Game Hanging: {HangingReason} (Iterations: {TotalIterations})";
            }

            return $"Game Completed: {Winner} wins in {TotalTurns} turns (Iterations: {TotalIterations})";
        }
    }
} 