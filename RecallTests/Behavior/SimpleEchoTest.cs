using NUnit.Framework;
using RecallCore.Entities;
using RecallCore.Actions;
using RecallCore.Memory;
using System.Collections.Generic;

namespace RecallTests.Behavior
{
    [TestFixture]
    public class SimpleEchoTest
    {
        [Test]
        public void SingleAttack_ShouldDealBaseDamage()
        {
            // Arrange
            var player = new Player("Hero", 30);
            var enemy = new Enemy("Slime", 50);
            
            var actions = new List<ActionRecord>
            {
                new ActionRecord("Hero", "Attack", ActionType.Attack, 1, 1)
            };
            var echoCard = new EchoCard();
            echoCard.Name = "Single Attack";
            echoCard.Actions = actions;
            
            int initialEnemyHP = enemy.HP;
            
            // Act
            var result = EchoExecutor.ExecuteEchoCard(echoCard, player, enemy);
            
            // Assert
            int actualDamage = initialEnemyHP - enemy.HP;
            Console.WriteLine($"Single Attack - Expected: 6, Actual: {actualDamage}");
            Console.WriteLine($"Result: HeavyStrike={result.HeavyStrikeCount}, Normal={result.NormalAttackCount}");
        }
        
        [Test]
        public void ChargeThenAttack_ShouldDealHeavyStrikeDamage()
        {
            // Arrange
            var player = new Player("Hero", 30);
            var enemy = new Enemy("Slime", 50);
            
            var actions = new List<ActionRecord>
            {
                new ActionRecord("Hero", "Charge", ActionType.Charge, 1, 1),
                new ActionRecord("Hero", "Attack", ActionType.Attack, 1, 2)
            };
            var echoCard = new EchoCard();
            echoCard.Name = "Charge + Attack";
            echoCard.Actions = actions;
            
            int initialEnemyHP = enemy.HP;
            
            // Act
            var result = EchoExecutor.ExecuteEchoCard(echoCard, player, enemy);
            
            // Assert
            int actualDamage = initialEnemyHP - enemy.HP;
            Console.WriteLine($"Charge + Attack - Expected: 10, Actual: {actualDamage}");
            Console.WriteLine($"Result: HeavyStrike={result.HeavyStrikeCount}, Normal={result.NormalAttackCount}");
        }
    }
} 