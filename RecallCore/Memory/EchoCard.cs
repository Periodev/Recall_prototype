using System.Collections.Generic;

namespace RecallCore.Memory
{
    public class EchoCard
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<ActionRecord> Actions { get; set; }
        public EchoType Type { get; set; }
        public bool IsUsed { get; set; } = false;
        
        // 卡片屬性
        public int TurnCreated { get; set; }
        public string Description { get; set; }
        public EchoRarity Rarity { get; set; } = EchoRarity.Common;
        
        // 標記系統
        public List<EchoTag> Tags { get; set; } = new();
        public Dictionary<string, object> Metadata { get; set; } = new();
        
        public EchoCard()
        {
            Actions = new List<ActionRecord>();
            Tags = new List<EchoTag>();
            Metadata = new Dictionary<string, object>();
        }
    }
    
    public enum EchoType
    {
        Standard,
        Fused
    }
    
    public enum EchoRarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }
    
    public class EchoTag
    {
        public string Name { get; set; }
        public string Color { get; set; }
        public string Description { get; set; }
    }
} 