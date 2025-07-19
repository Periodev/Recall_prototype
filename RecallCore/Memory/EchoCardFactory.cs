using System;
using System.Collections.Generic;
using System.Linq;

namespace RecallCore.Memory
{
    public class EchoCardFactory
    {
        private int nextCardId = 1;
        
        public EchoCard CreateCard(List<ActionRecord> actions, int turnCreated)
        {
            var card = new EchoCard
            {
                Id = GenerateCardId(),
                Name = GenerateCardName(actions),
                Actions = new List<ActionRecord>(actions),
                Type = EchoType.Standard,
                TurnCreated = turnCreated,
                Description = GenerateDescription(actions),
                Rarity = DetermineRarity(actions)
            };
            
            AddAutomaticTags(card, actions);
            return card;
        }
        
        private string GenerateCardId()
        {
            return $"ECHO_{nextCardId++}";
        }
        
        private string GenerateCardName(List<ActionRecord> actions)
        {
            // 只顯示行動符號的基本字串
            return string.Join("", actions.Select(a => GetActionSymbol(a.ActionType)));
        }
        
        private string GenerateDescription(List<ActionRecord> actions)
        {
            var actionNames = actions.Select(a => a.ActionType.ToString()).ToArray();
            return string.Join(" + ", actionNames);
        }
        
        private EchoRarity DetermineRarity(List<ActionRecord> actions)
        {
            if (actions.Count >= 4) return EchoRarity.Rare;
            if (actions.Count >= 3) return EchoRarity.Uncommon;
            return EchoRarity.Common;
        }
        
        private void AddAutomaticTags(EchoCard card, List<ActionRecord> actions)
        {
            // 根據行動組合自動添加標記
            if (actions.Count >= 3)
            {
                card.Tags.Add(new EchoTag { Name = "Combo", Color = "Blue", Description = "連擊組合" });
            }
            
            if (actions.Any(a => a.ActionType == ActionType.Charge))
            {
                card.Tags.Add(new EchoTag { Name = "Charge", Color = "Yellow", Description = "包含充能" });
            }
            
            if (actions.Any(a => a.ActionType == ActionType.Block))
            {
                card.Tags.Add(new EchoTag { Name = "Defense", Color = "Green", Description = "包含防禦" });
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