using System;
using System.Collections.Generic;
using System.Linq;

namespace RecallCore.Memory
{
    public class TimelineManager
    {
        private List<ActionRecord> fullTimeline = new();
        private List<ActionRecord> visibleWindow = new();
        private int windowSize = 6;
        
        // 事件
        public event Action<ActionRecord> OnActionAdded;
        public event Action<ActionRecord> OnActionRemoved;
        public event Action OnWindowUpdated;
        
        public void AddAction(ActionRecord action)
        {
            fullTimeline.Add(action);
            UpdateVisibleWindow();
            OnActionAdded?.Invoke(action);
        }
        
        public void RemoveAction(int index)
        {
            if (index >= 0 && index < fullTimeline.Count)
            {
                var removedAction = fullTimeline[index];
                fullTimeline.RemoveAt(index);
                UpdateVisibleWindow();
                OnActionRemoved?.Invoke(removedAction);
            }
        }
        
        public void ModifyAction(int index, ActionRecord newAction)
        {
            if (index >= 0 && index < fullTimeline.Count)
            {
                var oldAction = fullTimeline[index];
                fullTimeline[index] = newAction;
                UpdateVisibleWindow();
            }
        }
        
        private void UpdateVisibleWindow()
        {
            var startIndex = Math.Max(0, fullTimeline.Count - windowSize);
            visibleWindow = fullTimeline.Skip(startIndex).ToList();
            OnWindowUpdated?.Invoke();
        }
        
        public List<ActionRecord> GetVisibleActions()
        {
            return visibleWindow.ToList();
        }
        
        public List<ActionRecord> GetFullTimeline()
        {
            return fullTimeline.ToList();
        }
        
        public List<ActionRecord> GetActionsInRange(int start, int end)
        {
            // 基於視窗索引
            if (start >= 0 && end < visibleWindow.Count && start <= end)
            {
                return visibleWindow.GetRange(start, end - start + 1);
            }
            return new List<ActionRecord>();
        }
        
        public void DisplayTimeline()
        {
            if (visibleWindow.Count == 0)
            {
                Console.WriteLine("Timeline: (empty)");
                return;
            }
            
            var timeline = string.Join(" ", visibleWindow.Select((action, index) => 
                $"{index + 1}.{GetActionSymbol(action.ActionType)}"));
            
            Console.WriteLine($"Timeline: {timeline}");
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
        
        public string GetTimelineString()
        {
            if (visibleWindow.Count == 0)
                return "(empty)";
                
            return string.Join(" ", visibleWindow.Select((action, index) => 
                $"{index + 1}.{GetActionSymbol(action.ActionType)}"));
        }
    }
} 