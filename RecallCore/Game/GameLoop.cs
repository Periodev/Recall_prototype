using RecallCore.Entities;
using RecallCore.Memory;

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
                Console.WriteLine($"\n[Player HP: {player.HP}] [Enemy HP: {enemy.HP}]");
                Console.WriteLine("Choose action: (A)ttack / (B)lock / (C)harge / (E)cho");
                string input = Console.ReadLine()?.ToUpper() ?? "";

                string chosenAction = ParseAction(input);
                timeline.Record(chosenAction);
                ExecutePlayerAction(chosenAction);

                if (enemy.HP > 0)
                {
                    string enemyAction = enemy.DecideAction();
                    Console.WriteLine($"Enemy uses {enemyAction}!");
                    ExecuteEnemyAction(enemyAction);
                }
            }

            Console.WriteLine(player.HP <= 0 ? "You Lose!" : "You Win!");
        }

        private string ParseAction(string input)
        {
            return input switch
            {
                "A" => "Attack",
                "B" => "Block",
                "C" => "Charge",
                "E" => timeline.ReplayLast() ?? "Attack",
                _ => "Attack"
            };
        }

        private void ExecutePlayerAction(string action)
        {
            switch (action)
            {
                case "Attack":
                    enemy.TakeDamage(3);
                    break;
                case "Charge":
                    player.AddEnergy(1);
                    break;
                case "Block":
                    player.Block();
                    break;
            }
            Console.WriteLine($"You performed {action}!");
        }

        private void ExecuteEnemyAction(string action)
        {
            if (action == "Attack")
                player.TakeDamage(2);
        }

        private bool IsGameOver() => player.HP <= 0 || enemy.HP <= 0;
    }
}
