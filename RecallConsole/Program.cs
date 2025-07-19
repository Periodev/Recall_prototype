using System;
using System.IO;
using RecallCore.Actions;
using RecallCore.Entities;
using RecallCore.AI;
using RecallCore.Memory;

namespace RecallConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            string logFile = "battle.log";
            using StreamWriter logWriter = new StreamWriter(logFile, false) { AutoFlush = true };

            // 初始化系統
            var timelineManager = new TimelineManager();
            var echoDeck = new EchoDeck();
            var recallSystem = new RecallSystem(timelineManager, echoDeck);

            Player player = new Player("Hero", 10);
            Enemy enemy = new Enemy("Pattern Slime", 15, 3, null); // 不使用 AI

            int step = 1;
            int turn = 1;
            int maxIterations = 1000; // 防禦性程式設計：最大迭代次數
            int currentIteration = 0;
            Console.WriteLine("=== Recall Console Battle (Manual Mode) ===");
            logWriter.WriteLine("=== Recall Console Battle (Manual Mode) ===");
            ShowHelp();

            while (player.HP > 0 && enemy.HP > 0 && currentIteration < maxIterations)
            {
                currentIteration++;
                
                // 防禦性程式設計：狀態檢查
                if (player.HP < 0 || enemy.HP < 0 || player.ActionPoints < 0 || enemy.ActionPoints < 0)
                {
                    Console.WriteLine("Error: Invalid game state detected!");
                    logWriter.WriteLine("Error: Invalid game state detected!");
                    break;
                }
                
                // 玩家行動階段 - 每回合2步
                for (int playerStep = 0; playerStep < 2; playerStep++)
                {
                    // 檢查遊戲結束
                    if (player.HP <= 0 || enemy.HP <= 0) break;
                    
                    // 顯示遊戲狀態
                    DisplayGameState(turn, step, player, enemy, timelineManager);
                    
                    // 獲取玩家輸入
                    var input = GetPlayerInput();
                    
                    // 處理特殊命令
                    if (HandleSpecialCommands(input, recallSystem, turn, timelineManager, player))
                    {
                        playerStep--; // 重新執行這一步
                        continue;
                    }
                    
                    // 處理玩家行動
                    if (HandlePlayerAction(input, player, enemy, timelineManager, turn, step))
                    {
                        step++;
                    }
                    else
                    {
                        // 行動失敗，但不重新執行這一步，直接跳過
                        // 這樣可以避免 soft lock
                        Console.WriteLine("Action failed, skipping this step");
                    }
                }
                
                // 檢查遊戲結束
                if (player.HP <= 0 || enemy.HP <= 0) break;
                
                // 敵人行動階段
                DisplayGameState(turn, step, player, enemy, timelineManager);
                Console.WriteLine("Enemy action (A/B/C):");
                var enemyInput = GetPlayerInput();
                
                if (HandleEnemyAction(enemyInput, enemy, player, timelineManager, turn, step))
                {
                    step++;
                }
                
                // 回合結束，重置 AP
                player.ResetAP();
                enemy.ResetAP();
                turn++;
            }

            // 防禦性程式設計：檢查是否因為超時而結束
            if (currentIteration >= maxIterations)
            {
                Console.WriteLine("Error: Game loop timeout (possible infinite loop)!");
                logWriter.WriteLine("Error: Game loop timeout (possible infinite loop)!");
            }

            string result = player.HP <= 0 ? "Enemy Victory!" : "Player Victory!";
            Console.WriteLine($"=== Battle End: {result} ===");
            logWriter.WriteLine($"=== Battle End: {result} ===");
        }
        
        static void DisplayGameState(int turn, int step, Player player, Enemy enemy, TimelineManager timelineManager)
        {
            // 格式：R回合-S步驟 | P:HP/AP | E:HP/AP | Timeline (只顯示玩家行動)
            var timeline = timelineManager.GetTimelineString();
            Console.WriteLine($"R{turn}-S{step} | P:{player.HP}/{player.ActionPoints} | E:{enemy.HP}/{enemy.ActionPoints} | {timeline}");
            Console.Write("> ");
        }
        
        static string GetPlayerInput()
        {
            return Console.ReadLine()?.Trim() ?? "";
        }
        
        static bool HandleSpecialCommands(string input, RecallSystem recallSystem, int currentTurn, TimelineManager timelineManager, Player player)
        {
            var upperInput = input.ToUpper();
            
            // Recall 命令
            if (upperInput.StartsWith("R "))
            {
                return recallSystem.HandleRecallCommand(input, currentTurn, player);
            }
            
            // Echo 命令
            if (upperInput.StartsWith("ECHO"))
            {
                return recallSystem.HandleEchoCommand(input);
            }
            
            // Help 命令
            if (upperInput == "HELP")
            {
                ShowHelp();
                return true;
            }
            
            // Skip 命令 - 跳過當前行動
            if (upperInput == "SKIP")
            {
                Console.WriteLine("Skipping current action");
                return true;
            }
            
            // Pass 命令 - 跳過當前行動
            if (upperInput == "PASS")
            {
                Console.WriteLine("Passing current action");
                return true;
            }
            
            return false;
        }
        
        static bool HandlePlayerAction(string input, Player player, Enemy enemy, TimelineManager timelineManager, int turn, int step)
        {
            // 防禦性程式設計：檢查初始狀態
            if (player.HP <= 0 || enemy.HP <= 0)
            {
                Console.WriteLine("Error: Cannot execute action - game should be over");
                return false;
            }
            
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
                    
                case "B":
                case "BLOCK":
                    action = new BlockAction();
                    actionType = ActionType.Block;
                    break;
                    
                case "C":
                case "CHARGE":
                    action = new ChargeAction();
                    actionType = ActionType.Charge;
                    break;
                    
                default:
                    Console.WriteLine("Invalid command, type HELP to see available commands");
                    return false;
            }
            
            if (player.ActionPoints >= action.Cost)
            {
                action.Execute(player, enemy);
                player.ActionPoints -= action.Cost;
                
                // 防禦性程式設計：檢查執行後狀態
                if (player.HP < 0 || enemy.HP < 0)
                {
                    Console.WriteLine("Error: Action resulted in invalid HP state");
                    return false;
                }
                
                // 記錄玩家行動到 Timeline
                var actionRecord = new ActionRecord(
                    player.Name, action.Name, actionType, turn, step);
                timelineManager.AddAction(actionRecord);
                
                Console.WriteLine($"Player executed {action.Name}");
                return true;
            }
            else
            {
                Console.WriteLine($"AP insufficient, cannot execute {action.Name}");
                return false;
            }
        }
        
        static bool HandleEnemyAction(string input, Enemy enemy, Player player, TimelineManager timelineManager, int turn, int step)
        {
            // 防禦性程式設計：檢查初始狀態
            if (player.HP <= 0 || enemy.HP <= 0)
            {
                Console.WriteLine("Error: Cannot execute enemy action - game should be over");
                return false;
            }
            
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
                    
                case "B":
                case "BLOCK":
                    action = new BlockAction();
                    actionType = ActionType.Block;
                    break;
                    
                case "C":
                case "CHARGE":
                    action = new ChargeAction();
                    actionType = ActionType.Charge;
                    break;
                    
                default:
                    Console.WriteLine("Invalid enemy action, please type A/B/C");
                    return false;
            }
            
            if (enemy.ActionPoints >= action.Cost)
            {
                action.Execute(enemy, player);
                enemy.ActionPoints -= action.Cost;
                
                // 防禦性程式設計：檢查執行後狀態
                if (player.HP < 0 || enemy.HP < 0)
                {
                    Console.WriteLine("Error: Enemy action resulted in invalid HP state");
                    return false;
                }
                
                // 注意：敵人行動不記錄到 Timeline，只有玩家行動才記錄
                // var actionRecord = new ActionRecord(
                //     enemy.Name, action.Name, actionType, turn, step);
                // timelineManager.AddAction(actionRecord);
                
                Console.WriteLine($"Enemy executed {action.Name}");
                return true;
            }
            else
            {
                Console.WriteLine($"Enemy AP insufficient, cannot execute {action.Name}");
                return false;
            }
        }
        
        static ActionType GetActionType(string actionName)
        {
            return actionName.ToUpper() switch
            {
                "ATTACK" => ActionType.Attack,
                "BLOCK" => ActionType.Block,
                "CHARGE" => ActionType.Charge,
                _ => ActionType.Attack
            };
        }
        
        static void ShowHelp()
        {
            Console.WriteLine("\n=== Available Commands ===");
            Console.WriteLine("A/ATTACK     - Attack");
            Console.WriteLine("B/BLOCK      - Block");
            Console.WriteLine("C/CHARGE     - Charge");
            Console.WriteLine("R <start> <end> - Recall specified actions (costs 1 AP)");
            Console.WriteLine("ECHO         - View Echo deck");
            Console.WriteLine("ECHO <num>   - Use specified Echo card");
            Console.WriteLine("SKIP         - Skip current action");
            Console.WriteLine("PASS         - Pass current action");
            Console.WriteLine("HELP         - Show this help");
            Console.WriteLine("\n=== Manual Mode Instructions ===");
            Console.WriteLine("1. Player Action Phase: Execute 2 actions (A/B/C) per turn");
            Console.WriteLine("2. Enemy Action Phase: Execute 1 enemy action (A/B/C)");
            Console.WriteLine("3. Repeat until game ends");
            Console.WriteLine("\n=== Timeline Instructions ===");
            Console.WriteLine("- Only player actions are recorded");
            Console.WriteLine("- Format: 1.A 2.B 3.C (numbered.action)");
            Console.WriteLine("- Available R command to recall history (costs 1 AP)");
        }
    }
}
