using System;
using System.Collections.Generic;
using System.Linq;
using RecallCore.Entities;

namespace RecallCore.Memory
{
    public class RecallSystem
    {
        private TimelineManager timelineManager;
        private EchoDeck echoDeck;
        private EchoCardFactory echoFactory;
        private int echoUsageThisTurn = 0; // 追蹤本回合 Echo 使用次數
        
        public RecallSystem(TimelineManager timelineManager, EchoDeck echoDeck)
        {
            this.timelineManager = timelineManager;
            this.echoDeck = echoDeck;
            this.echoFactory = new EchoCardFactory();
        }
        
        public void ResetTurnEchoUsage()
        {
            echoUsageThisTurn = 0;
        }
        
        public bool HandleRecallCommand(string input, int currentTurn, Player player)
        {
            var command = RecallCommand.Parse(input);
            if (command == null)
            {
                return false;
            }
            
            // 檢查 AP 是否足夠
            if (player.ActionPoints < 1)
            {
                Console.WriteLine("AP insufficient, cannot Recall");
                return true;
            }
            
            var visibleActions = timelineManager.GetVisibleActions();
            if (!command.IsValid(visibleActions.Count))
            {
                Console.WriteLine("Invalid Recall range");
                return true;
            }
            
            var actions = timelineManager.GetActionsInRange(command.StartStep, command.EndStep);
            if (actions.Count == 0)
            {
                Console.WriteLine("Invalid Recall range");
                return true;
            }
            
            // 消耗 1 AP
            player.ActionPoints -= 1;
            
            // 顯示 Recall 資訊
            var actionString = string.Join("", actions.Select(a => GetActionSymbol(a.ActionType)));
            Console.WriteLine($"Recall: {actionString}");
            
            // 創建 Echo 卡片
            var echoCard = echoFactory.CreateCard(actions, currentTurn);
            echoDeck.AddCard(echoCard);
            
            // 簡潔的創建訊息
            Console.WriteLine($"Created Echo: {echoCard.Name}");
            
            return true;
        }
        
        public bool HandleEchoCommand(string input, Player player, Enemy enemy)
        {
            var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            
            if (parts.Length == 1 && parts[0].ToUpper() == "ECHO")
            {
                // 顯示 Echo 牌組
                echoDeck.DisplayEchoDeck();
                return true;
            }
            
            if (parts.Length >= 2 && parts[0].ToUpper() == "ECHO")
            {
                // 檢查回合限制
                if (echoUsageThisTurn >= GameConstants.MAX_ECHO_PER_TURN)
                {
                    Console.WriteLine($"Echo usage limit reached for this turn ({GameConstants.MAX_ECHO_PER_TURN})");
                    return true;
                }
                
                // 使用 Echo 卡片
                if (int.TryParse(parts[1], out int cardIndex))
                {
                    UseEchoCard(cardIndex - 1, player, enemy); // 轉換為 0-based 索引
                    return true;
                }
            }
            
            return false;
        }
        
        private void UseEchoCard(int cardIndex, Player player, Enemy enemy)
        {
            var availableCards = echoDeck.GetAvailableCards();
            if (cardIndex >= 0 && cardIndex < availableCards.Count)
            {
                var card = availableCards[cardIndex];
                Console.WriteLine($"Using Echo: {card.Name}");
                
                try
                {
                    // 使用新的 EchoExecutor 執行 Echo 卡
                    var result = EchoExecutor.ExecuteEchoCard(card, player, enemy);
                    
                    // 顯示執行結果
                    if (result.HasAnyEffect)
                    {
                        Console.WriteLine($"Echo Effects: {result.GetSummary()}");
                    }
                    else
                    {
                        Console.WriteLine("Echo executed (no effects)");
                    }
                    
                    // 標記為已使用
                    echoDeck.UseCard(card.Id);
                    echoUsageThisTurn++;
                    
                    Console.WriteLine($"Echo used (Parallel execution) - {echoUsageThisTurn}/{GameConstants.MAX_ECHO_PER_TURN} this turn");
                }
                catch (InvalidOperationException ex)
                {
                    Console.WriteLine($"Echo execution failed: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Invalid Echo card number");
            }
        }
        
        private string GetActionSymbol(ActionType actionType)
        {
            return actionType switch
            {
                ActionType.Attack => "A",
                ActionType.Block => "B", 
                ActionType.Charge => "C",
                ActionType.Recall => "R",
                ActionType.Echo => "E",
                _ => "?"
            };
        }
    }
} 