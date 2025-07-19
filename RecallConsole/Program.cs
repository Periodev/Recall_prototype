using System;
using System.IO;
using RecallCore.Actions;
using RecallCore.Entities;
using RecallCore.AI;

namespace RecallConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            string logFile = "battle.log";
            using StreamWriter logWriter = new StreamWriter(logFile, false) { AutoFlush = true };

            Player player = new Player("Hero", 30);
            
            // 使用固定序列 AI 敵人
            var fixedSequenceAI = new FixedSequenceAIStrategy(
                new AttackAction(),
                new BlockAction(),
                new ChargeAction()
            );
            Enemy enemy = new Enemy("Pattern Slime", 15, 1, fixedSequenceAI);

            IAction[] playerActions = new IAction[] {
                new AttackAction(), new BlockAction(), new ChargeAction()
            };
            int playerActionIndex = 0;
            Random rng = new Random();

            int step = 1;
            int turn = 1;
            Console.WriteLine("=== Recall Console Battle (Fixed Sequence AI) ===");
            logWriter.WriteLine("=== Recall Console Battle (Fixed Sequence AI) ===");

            while (player.HP > 0 && enemy.HP > 0)
            {
                // Turn header and initial state (not a step)
                Console.WriteLine($"\n=== Turn {turn} ===");
                string playerStatus = $"[Player] HP: {player.HP}, AP: {player.ActionPoints}";
                string enemyStatus = $"[Enemy] HP: {enemy.HP}, AP: {enemy.ActionPoints}";
                
                if (player.IsCharged) playerStatus += " (Charged)";
                if (enemy.IsCharged) enemyStatus += " (Charged)";
                
                Console.WriteLine($"{playerStatus} | {enemyStatus}");
                logWriter.WriteLine($"=== Turn {turn} ===");
                logWriter.WriteLine($"{playerStatus} | {enemyStatus}");

                // Step 1: Enemy announce
                enemy.AnnounceAction();
                Console.WriteLine($"[Step {step}] Enemy announces: {enemy.GetAnnouncedActionName()}");
                logWriter.WriteLine($"[Step {step}] Enemy announces: {enemy.GetAnnouncedActionName()}");
                
                // If enemy announced Block, set blocking state immediately
                if (enemy.GetAnnouncedActionName() == "Block")
                {
                    enemy.Block(); // 設置 Block 狀態
                }
                
                step++;

                // Check announcement type
                bool isBlockAnnouncement = enemy.GetAnnouncedActionName() == "Block";

                // Step 2: Player first action
                IAction playerAction1 = playerActions[playerActionIndex];
                if (player.ActionPoints >= playerAction1.Cost)
                {
                    playerAction1.Execute(player, enemy);
                    player.ActionPoints -= playerAction1.Cost;
                    Console.WriteLine($"[Step {step}] Player uses {playerAction1.Name}");
                    logWriter.WriteLine($"[Step {step}] Player uses {playerAction1.Name}");
                    
                    string stepPlayerStatus = $"[Player] HP: {player.HP}, AP: {player.ActionPoints}";
                    string stepEnemyStatus = $"[Enemy] HP: {enemy.HP}, AP: {enemy.ActionPoints}";
                    
                    if (player.IsCharged) stepPlayerStatus += " (Charged)";
                    if (enemy.IsCharged) stepEnemyStatus += " (Charged)";
                    
                    Console.WriteLine($"[Step {step}] State: {stepPlayerStatus} | {stepEnemyStatus}");
                    logWriter.WriteLine($"[Step {step}] State: {stepPlayerStatus} | {stepEnemyStatus}");
                    step++;
                }
                else
                {
                    Console.WriteLine($"[Step {step}] Player AP insufficient for {playerAction1.Name}");
                    logWriter.WriteLine($"[Step {step}] Player AP insufficient for {playerAction1.Name}");
                    
                    string stepPlayerStatus = $"[Player] HP: {player.HP}, AP: {player.ActionPoints}";
                    string stepEnemyStatus = $"[Enemy] HP: {enemy.HP}, AP: {enemy.ActionPoints}";
                    
                    if (player.IsCharged) stepPlayerStatus += " (Charged)";
                    if (enemy.IsCharged) stepEnemyStatus += " (Charged)";
                    
                    Console.WriteLine($"[Step {step}] State: {stepPlayerStatus} | {stepEnemyStatus}");
                    logWriter.WriteLine($"[Step {step}] State: {stepPlayerStatus} | {stepEnemyStatus}");
                    step++;
                }
                playerActionIndex = (playerActionIndex + 1) % playerActions.Length;

                if (enemy.HP <= 0) break;

                // Step 3: Player second action
                IAction playerAction2 = playerActions[playerActionIndex];
                if (player.ActionPoints >= playerAction2.Cost)
                {
                    playerAction2.Execute(player, enemy);
                    player.ActionPoints -= playerAction2.Cost;
                    Console.WriteLine($"[Step {step}] Player uses {playerAction2.Name}");
                    logWriter.WriteLine($"[Step {step}] Player uses {playerAction2.Name}");
                    
                    string stepPlayerStatus = $"[Player] HP: {player.HP}, AP: {player.ActionPoints}";
                    string stepEnemyStatus = $"[Enemy] HP: {enemy.HP}, AP: {enemy.ActionPoints}";
                    
                    if (player.IsCharged) stepPlayerStatus += " (Charged)";
                    if (enemy.IsCharged) stepEnemyStatus += " (Charged)";
                    
                    Console.WriteLine($"[Step {step}] State: {stepPlayerStatus} | {stepEnemyStatus}");
                    logWriter.WriteLine($"[Step {step}] State: {stepPlayerStatus} | {stepEnemyStatus}");
                    step++;
                }
                else
                {
                    Console.WriteLine($"[Step {step}] Player AP insufficient for {playerAction2.Name}");
                    logWriter.WriteLine($"[Step {step}] Player AP insufficient for {playerAction2.Name}");
                    
                    string stepPlayerStatus = $"[Player] HP: {player.HP}, AP: {player.ActionPoints}";
                    string stepEnemyStatus = $"[Enemy] HP: {enemy.HP}, AP: {enemy.ActionPoints}";
                    
                    if (player.IsCharged) stepPlayerStatus += " (Charged)";
                    if (enemy.IsCharged) stepEnemyStatus += " (Charged)";
                    
                    Console.WriteLine($"[Step {step}] State: {stepPlayerStatus} | {stepEnemyStatus}");
                    logWriter.WriteLine($"[Step {step}] State: {stepPlayerStatus} | {stepEnemyStatus}");
                    step++;
                }
                playerActionIndex = (playerActionIndex + 1) % playerActions.Length;

                if (enemy.HP <= 0) break;

                // Step 4: Enemy action (execute announced action)
                if (!isBlockAnnouncement) // Block already executed
                {
                    string announcedActionName = enemy.GetAnnouncedActionName();
                    enemy.ExecuteAnnouncedAction(player);
                    Console.WriteLine($"[Step {step}] Enemy executes announced {announcedActionName}");
                    logWriter.WriteLine($"[Step {step}] Enemy executes announced {announcedActionName}");
                    
                    string stepPlayerStatus = $"[Player] HP: {player.HP}, AP: {player.ActionPoints}";
                    string stepEnemyStatus = $"[Enemy] HP: {enemy.HP}, AP: {enemy.ActionPoints}";
                    
                    if (player.IsCharged) stepPlayerStatus += " (Charged)";
                    if (enemy.IsCharged) stepEnemyStatus += " (Charged)";
                    
                    Console.WriteLine($"[Step {step}] State: {stepPlayerStatus} | {stepEnemyStatus}");
                    logWriter.WriteLine($"[Step {step}] State: {stepPlayerStatus} | {stepEnemyStatus}");
                    step++;
                }

                // Turn end, reset AP
                player.ResetAP();
                enemy.ResetAP();
                turn++;
            }

            string result = player.HP <= 0 ? "Enemy Victory!" : "Player Victory!";
            Console.WriteLine($"=== Battle End: {result} ===");
            logWriter.WriteLine($"=== Battle End: {result} ===");
        }
    }
}
