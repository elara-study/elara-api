namespace Elara.Application.Common.Gamification
{
    /// <summary>
    /// Single source of truth for student level (derived from <see cref="TotalXP"/> only).
    /// </summary>
    public static class StudentGamification
    {
        public const int XpPerLevel = 100;

        public static int CalculateLevel(int totalXp) => (totalXp / XpPerLevel) + 1;

        public static int GetXpThresholdForNextLevel(int level) => level * XpPerLevel;

        public static int GetRemainingXpToNextLevel(int totalXp, out int currentLevel, out int nextLevel, out int xpTarget)
        {
            currentLevel = CalculateLevel(totalXp);
            nextLevel = currentLevel + 1;
            xpTarget = GetXpThresholdForNextLevel(currentLevel);
            return Math.Max(0, xpTarget - totalXp);
        }
    }
}
