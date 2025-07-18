using RecallCore.Game;
using RecallCore.Entities;
using RecallCore.Memory;
using RecallCore.Actions;

class Program
{
    static void Main()
    {
        // IAction 測試範例
        var player = new Player("Hero", 30);
        var enemy = new Player("Slime", 15);
        var attack = new AttackAction();
        Console.WriteLine($"[Before] Player HP: {player.HP}, Enemy HP: {enemy.HP}");
        attack.Execute(player, enemy);
        Console.WriteLine($"[After] Player HP: {player.HP}, Enemy HP: {enemy.HP}");

        // 原本的遊戲流程
        var game = new GameLoop();
        var timeline = new MemoryTimeline(3);
        int turn = 1;
        while (player.HP > 0 && enemy.HP > 0)
        {
            // 玩家行動紀錄
            if (action != null)
                timeline.Add(new ActionRecord(player.Name, action.Name, turn));
            // 敵人行動紀錄
            timeline.Add(new ActionRecord(enemy.Name, enemyAction.Name, turn));

            // 顯示最近 3 回合行動紀錄
            Console.WriteLine("=== 最近行動紀錄 ===");
            foreach (var record in timeline.GetRecentRecords())
            {
                Console.WriteLine($"[Turn {record.Turn}] {record.ActorName} 使用 {record.ActionName}");
            }

            turn++;
        }
    }
}
