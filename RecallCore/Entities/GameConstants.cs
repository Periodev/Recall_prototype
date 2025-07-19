namespace RecallCore.Entities
{
    public static class GameConstants
    {
        public const int PLAYER_INITIAL_AP = 2;
        public const int ENEMY_INITIAL_AP = 1;
        
        // 護盾機制常數
        public const int BLOCK_SHIELD_VALUE = 3; // 每次 Block 提供的護盾值
        
        // 重擊機制常數
        public const int BASE_ATTACK_DAMAGE = 6; // 基礎攻擊傷害
        public const int BASE_SHIELD_VALUE = 3; // 基礎護盾值
        public const int BASE_CHARGE_VALUE = 1; // 基礎蓄力值
        public const int HEAVY_STRIKE_BONUS = 4; // 每次重擊的額外傷害
        public const int MAX_ECHO_PER_TURN = 2; // 每回合最多使用 Echo 次數
    }
} 