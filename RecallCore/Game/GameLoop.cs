using RecallCore.Entities;
using RecallCore.Memory;
using RecallCore.Actions;

namespace RecallCore.Game
{
    public class GameLoop
    {
        private Player player = null!;
        private Enemy enemy = null!;
        private Timeline timeline = null!;

        public void Start()
        {
            player = new Player("Hero", 30);
            enemy = new Enemy("Slime", 15);
            timeline = new Timeline(5);

            Console.WriteLine("=== Recall MVP Battle ===");

            while (!IsGameOver())
            {
                player.ResetAP();
                enemy.ResetAP();
                Console.WriteLine($"\n[Player HP: {player.HP}] [Enemy HP: {enemy.HP}] [Player AP: {player.ActionPoints}] [Enemy AP: {enemy.ActionPoints}]");
                Console.WriteLine("Choose action: (A)ttack / (B)lock / (C)harge / (E)cho");
                string input = Console.ReadLine()?.ToUpper() ?? "";

                IAction chosenAction = ParseAction(input);
                timeline.Record(chosenAction.Name);
                ExecutePlayerAction(chosenAction);

                if (enemy.HP > 0)
                {
                    IAction enemyAction = EnemyChooseAction();
                    Console.WriteLine($"Enemy uses {enemyAction.Name}!");
                    ExecuteEnemyAction(enemyAction);
                }
            }

            Console.WriteLine(player.HP <= 0 ? "You Lose!" : "You Win!");
        }

        private IAction ParseAction(string input)
        {
            return input switch
            {
                "A" => new AttackAction(),
                "B" => new BlockAction(),
                "C" => new ChargeAction(),
                "E" => new AttackAction(), // 這裡可根據 timeline 實作 Echo
                _ => new AttackAction()
            };
        }

        private void ExecutePlayerAction(IAction action)
        {
            if (player.ActionPoints < action.Cost)
            {
                Console.WriteLine($"AP 不足，無法執行 {action.Name}！");
                return;
            }
            action.Execute(player, enemy);
            player.ActionPoints -= action.Cost;
            Console.WriteLine($"You performed {action.Name}!");
        }

        private void ExecuteEnemyAction(IAction action)
        {
            if (enemy.ActionPoints < action.Cost)
            {
                Console.WriteLine($"敵人 AP 不足，什麼都沒做。");
                return;
            }
            action.Execute(enemy, player);
            enemy.ActionPoints -= action.Cost;
        }

        private IAction EnemyChooseAction()
        {
            var rng = new Random();
            int choice = rng.Next(1, 4);
            return choice switch
            {
                1 => new AttackAction(),
                2 => new BlockAction(),
                3 => new ChargeAction(),
                _ => new AttackAction()
            };
        }

        private bool IsGameOver() => player.HP <= 0 || enemy.HP <= 0;
    }
}
