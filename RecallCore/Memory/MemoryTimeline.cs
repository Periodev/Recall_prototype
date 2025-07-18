using System.Collections.Generic;
using System.Linq;
using RecallCore.Memory;

namespace RecallCore.Memory
{
    public class MemoryTimeline
    {
        private readonly int _capacity;
        private readonly List<ActionRecord> _records = new();

        public MemoryTimeline(int capacity = 3)
        {
            _capacity = capacity;
        }

        public void Add(ActionRecord record)
        {
            _records.Add(record);
            if (_records.Count > _capacity)
                _records.RemoveAt(0);
        }

        public List<ActionRecord> GetRecentRecords()
        {
            return _records.ToList();
        }
    }
} 