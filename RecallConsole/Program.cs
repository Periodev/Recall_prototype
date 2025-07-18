using System;
using System.IO;
using RecallCore.Actions;
using RecallCore.Entities;

namespace RecallConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            string logFile = "battle.log";
            using StreamWriter logWriter = new StreamWriter(logFile, false);

            Player player = new Player("Hero", 30);
            Enemy enemy = new Enemy("Slime", 20);

            IAction[] playerActions = new IAction[] {
                new AttackAction(), new BlockAction(), new ChargeAction()
            };
            int playerActionIndex = 0;
            Random rng = new Random();

            int turn = 1;
            Console.WriteLine("=== Recall Console Auto-Battle ===");
            logWriter.WriteLine("=== Recall Console Auto-Battle ===");

            while (player.HP > 0 && enemy.HP > 0)
            {
                Console.WriteLine($"[Turn {turn}] [玩家] HP: {player.HP}, AP: {player.ActionPoints} | [敵人] HP: {enemy.HP}, AP: {enemy.ActionPoints}");
                logWriter.WriteLine($"[Turn {turn}] [玩家] HP: {player.HP}, AP: {player.ActionPoints} | [敵人] HP: {enemy.HP}, AP: {enemy.ActionPoints}");

                // 玩家行動
                IAction playerAction = playerActions[playerActionIndex];
                if (player.ActionPoints >= playerAction.Cost)
                {
                    playerAction.Execute(player, enemy);
                    Console.WriteLine($"玩家使用 {playerAction.Name}");
                    logWriter.WriteLine($"玩家使用 {playerAction.Name}");
                }
                else
                {
                    Console.WriteLine($"玩家 AP 不足，無法執行 {playerAction.Name}");
                    logWriter.WriteLine($"玩家 AP 不足，無法執行 {playerAction.Name}");
                }
                playerActionIndex = (playerActionIndex + 1) % playerActions.Length;

                if (enemy.HP <= 0) break;

                // 敵人行動
                IAction enemyAction = rng.Next(3) switch
                {
                    0 => new AttackAction(),
                    1 => new BlockAction(),
                    _ => new ChargeAction()
                };
                if (enemy.ActionPoints >= enemyAction.Cost)
                {
                    enemyAction.Execute(enemy, player);
                    Console.WriteLine($"敵人使用 {enemyAction.Name}");
                    logWriter.WriteLine($"敵人使用 {enemyAction.Name}");
                }
                else
                {
                    Console.WriteLine($"敵人 AP 不足，無法執行 {enemyAction.Name}");
                    logWriter.WriteLine($"敵人 AP 不足，無法執行 {enemyAction.Name}");
                }

                // 回合結束，AP 自動恢復
                player.ActionPoints = 2;
                enemy.ActionPoints = 2;
                turn++;
            }

            string result = player.HP <= 0 ? "敵人勝利！" : "玩家勝利！";
            Console.WriteLine($"=== 戰鬥結束：{result} ===");
            logWriter.WriteLine($"=== 戰鬥結束：{result} ===");
        }
    }
}
