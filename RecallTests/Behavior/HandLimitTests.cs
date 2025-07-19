using NUnit.Framework;
using RecallCore.Memory;
using RecallCore.Entities;

namespace RecallTests.Behavior
{
    [TestFixture]
    public class HandLimitTests
    {
        [Test]
        public void EchoDeck_ShouldRespectHandLimit()
        {
            // Arrange
            var deck = new EchoDeck();
            
            // Act - 添加超過手牌上限的卡片
            for (int i = 0; i < 7; i++)
            {
                var card = new EchoCard();
                card.Name = $"Test Card {i + 1}";
                card.Id = $"card_{i + 1}";
                deck.AddCard(card);
            }
            
            // Assert
            Assert.That(deck.GetHandSize(), Is.EqualTo(5), "手牌應該限制在 5 張");
            Assert.That(deck.IsHandFull(), Is.True, "手牌應該已滿");
        }
        
        [Test]
        public void EchoDeck_ShouldDiscardOldestCardsWhenOverLimit()
        {
            // Arrange
            var deck = new EchoDeck();
            var discardedCards = new List<EchoCard>();
            
            // 訂閱棄牌事件
            deck.OnCardDiscarded += (card) => discardedCards.Add(card);
            
            // Act - 添加 7 張卡片
            for (int i = 0; i < 7; i++)
            {
                var card = new EchoCard();
                card.Name = $"Test Card {i + 1}";
                card.Id = $"card_{i + 1}";
                deck.AddCard(card);
            }
            
            // Assert
            Assert.That(discardedCards.Count, Is.EqualTo(2), "應該棄掉 2 張卡片");
            Assert.That(discardedCards[0].Name, Is.EqualTo("Test Card 1"), "應該棄掉最舊的卡片");
            Assert.That(discardedCards[1].Name, Is.EqualTo("Test Card 2"), "應該棄掉第二舊的卡片");
        }
        
        [Test]
        public void EchoDeck_ShouldNotExceedHandLimitAfterReset()
        {
            // Arrange
            var deck = new EchoDeck();
            
            // 添加 3 張卡片並使用它們
            for (int i = 0; i < 3; i++)
            {
                var card = new EchoCard();
                card.Name = $"Test Card {i + 1}";
                card.Id = $"card_{i + 1}";
                deck.AddCard(card);
            }
            
            // 使用所有卡片
            deck.UseCardByIndex(0);
            deck.UseCardByIndex(0);
            deck.UseCardByIndex(0);
            
            // 再添加 4 張新卡片
            for (int i = 3; i < 7; i++)
            {
                var card = new EchoCard();
                card.Name = $"Test Card {i + 1}";
                card.Id = $"card_{i + 1}";
                deck.AddCard(card);
            }
            
            // Act - 重置已使用的卡片
            deck.ResetUsedCards();
            
            // Assert
            Assert.That(deck.GetHandSize(), Is.EqualTo(5), "重置後手牌應該限制在 5 張");
        }
        
        [Test]
        public void EchoDeck_ShouldDisplayHandLimitInConsole()
        {
            // Arrange
            var deck = new EchoDeck();
            
            // Act & Assert - 空手牌時應該顯示上限
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                deck.DisplayEchoDeck();
                var output = sw.ToString();
                Assert.That(output, Does.Contain("[Max: 5]"), "應該顯示手牌上限");
            }
            
            // 添加卡片後也應該顯示上限
            var card = new EchoCard();
            card.Name = "Test Card";
            card.Id = "test_card";
            deck.AddCard(card);
            
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                deck.DisplayEchoDeck();
                var output = sw.ToString();
                Assert.That(output, Does.Contain("[Max: 5]"), "應該顯示手牌上限");
            }
        }
        
        [Test]
        public void EchoDeck_ShouldHandleHandLimitGracefully()
        {
            // Arrange
            var deck = new EchoDeck();
            
            // Act - 添加剛好等於上限的卡片
            for (int i = 0; i < 5; i++)
            {
                var card = new EchoCard();
                card.Name = $"Test Card {i + 1}";
                card.Id = $"card_{i + 1}";
                deck.AddCard(card);
            }
            
            // Assert
            Assert.That(deck.GetHandSize(), Is.EqualTo(5), "手牌應該剛好是 5 張");
            Assert.That(deck.IsHandFull(), Is.True, "手牌應該已滿");
            
            // 再添加一張卡片
            var extraCard = new EchoCard();
            extraCard.Name = "Extra Card";
            extraCard.Id = "extra_card";
            deck.AddCard(extraCard);
            
            // 手牌應該仍然是 5 張，最舊的卡片被棄掉
            Assert.That(deck.GetHandSize(), Is.EqualTo(5), "手牌應該仍然是 5 張");
            
            var availableCards = deck.GetAvailableCards();
            Assert.That(availableCards.Any(c => c.Name == "Test Card 1"), Is.False, "最舊的卡片應該被棄掉");
            Assert.That(availableCards.Any(c => c.Name == "Extra Card"), Is.True, "新卡片應該保留");
        }
    }
} 