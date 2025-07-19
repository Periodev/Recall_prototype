using System;
using System.Collections.Generic;
using System.Linq;

namespace RecallCore.Memory
{
    public class EchoDeck
    {
        private List<EchoCard> allCards = new();
        private List<EchoCard> availableCards = new();
        private List<EchoCard> usedCards = new();
        
        // 事件
        public event Action<EchoCard> OnCardAdded;
        public event Action<EchoCard> OnCardUsed;
        
        public void AddCard(EchoCard card)
        {
            allCards.Add(card);
            availableCards.Add(card);
            OnCardAdded?.Invoke(card);
        }
        
        public List<EchoCard> GetAvailableCards()
        {
            return availableCards.Where(c => !c.IsUsed).ToList();
        }
        
        public List<EchoCard> GetAllCards()
        {
            return allCards;
        }
        
        public void UseCard(string cardId)
        {
            var card = allCards.FirstOrDefault(c => c.Id == cardId);
            if (card != null)
            {
                card.IsUsed = true;
                usedCards.Add(card);
                availableCards.Remove(card);
                OnCardUsed?.Invoke(card);
            }
        }
        
        public void UseCardByIndex(int index)
        {
            var available = GetAvailableCards();
            if (index >= 0 && index < available.Count)
            {
                UseCard(available[index].Id);
            }
        }
        
        public void RemoveCard(string cardId)
        {
            var card = allCards.FirstOrDefault(c => c.Id == cardId);
            if (card != null)
            {
                allCards.Remove(card);
                availableCards.Remove(card);
                usedCards.Remove(card);
            }
        }
        
        public void Clear()
        {
            allCards.Clear();
            availableCards.Clear();
            usedCards.Clear();
        }
        
        public void ResetUsedCards()
        {
            foreach (var card in usedCards)
            {
                card.IsUsed = false;
                availableCards.Add(card);
            }
            usedCards.Clear();
        }
        
        public void DisplayEchoDeck()
        {
            var available = GetAvailableCards();
            
            if (available.Count == 0)
            {
                Console.WriteLine("Echo deck: (empty)");
                return;
            }
            
            var echoList = string.Join(" ", available.Select((card, index) => 
                $"{index + 1}.{card.Name}"));
            
            Console.WriteLine($"Echo deck: {echoList}");
        }
    }
} 