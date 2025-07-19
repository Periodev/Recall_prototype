using RecallCore.Entities;

namespace RecallCore.Entities
{
    public class Player : Actor
    {
        public Player(string name, int hp) : base(name, hp, GameConstants.PLAYER_INITIAL_AP) { }

        public override bool CanAct()
        {
            return HP > 0 && ActionPoints > 0;
        }

        public override int GetMaxHP()
        {
            // 假設初始 HP 就是最大 HP
            return HP;
        }
    }
}
