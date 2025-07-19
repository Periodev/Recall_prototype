using System;

namespace RecallCore.Memory
{
    public class RecallCommand
    {
        public int StartStep { get; set; }
        public int EndStep { get; set; }
        
        public RecallCommand(int startStep, int endStep)
        {
            StartStep = startStep;
            EndStep = endStep;
        }
        
        public static RecallCommand Parse(string input)
        {
            var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            
            if (parts.Length >= 3 && parts[0].ToUpper() == "R")
            {
                if (int.TryParse(parts[1], out int start) && int.TryParse(parts[2], out int end))
                {
                    // 轉換為 0-based 索引
                    return new RecallCommand(start - 1, end - 1);
                }
            }
            
            return null;
        }
        
        public bool IsValid(int maxSteps)
        {
            return StartStep >= 0 && EndStep < maxSteps && StartStep <= EndStep;
        }
    }
} 